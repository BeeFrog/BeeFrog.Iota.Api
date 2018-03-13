﻿using BeeFrog.Iota.Api.Iri.Dto;
using BeeFrog.Iota.Api.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BeeFrog.Iota.Api.Iri
{
    public class GenericWebClient : IGenericWebClient
    {
        /// <summary>
        /// Gets or sets the url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Sets or gets MaximumRetryCount. Default is 10. Look at RequestErrorHandler for more handling.
        /// </summary>
        public int MaximumRetryCount { get; set; }

        /// <summary>
        /// Gets or sets RequestErrorHandler
        /// </summary>
        public IRequestErrorHandler RequestErrorHandler { get; set; }

        /// <summary>
        /// Gets or sets request log delegate
        /// </summary>
        public Action<string> Log { get; set; }

        public GenericWebClient(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));

            this.Url = url;
            this.MaximumRetryCount = 10;
            this.RequestErrorHandler = new RequestErrorHandler();
        }

        public async Task<APIResult<TResponse>> RequestAsync<TResponse>(object request) where TResponse : new()
        {
            return await this.RequestAsync<TResponse>(request, CancellationToken.None);
        }

        /// <summary>
        /// Requests the specified request asynchronously
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        public async Task<APIResult<TResponse>> RequestAsync<TResponse>(object request, CancellationToken cancellationToken) where TResponse : new()
        {
            string stringData = request.ToJson();
            Log?.Invoke($"Request: {stringData}");

            Exception lastException = null;

            for (int i = 0; i < MaximumRetryCount; i++)
            {
                try
                {
                    var content = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
                    var response = await RequestContentAsync<TResponse>(content, cancellationToken);
                    return new APIResult<TResponse>(response);
                }
                catch(OperationCanceledException oex)
                {
                    return new APIResult<TResponse>(oex, "Operation Cancelled");
                }
                catch(Exception e)
                {
                    lastException = e;
                    var handled = await RequestErrorHandler?.HandleError(request, e, i + 1, cancellationToken);

                    
                    if(cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }

            return new APIResult<TResponse>(lastException, $"Max retry count reached with message '{lastException?.Message}'");
        }

        private async Task<TResponse> RequestContentAsync<TResponse>(HttpContent content, CancellationToken cancellationToken) where TResponse : new()
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(30);
                client.DefaultRequestHeaders.Add("User-Agent", "borlay.iota.netcore");
                client.DefaultRequestHeaders.Add("X-IOTA-API-Version", "1");

                try
                {
                    var postResponse = await client.PostAsync(Url, content, cancellationToken);
                    var stringResult = await postResponse.Content.ReadAsStringAsync();

                    Log?.Invoke($"Request success: {postResponse.IsSuccessStatusCode}");
                    Log?.Invoke($"Response: {stringResult}");

                    if (postResponse.IsSuccessStatusCode)
                    {
                        var objectResult = JsonConvert.DeserializeObject<TResponse>(stringResult);
                        return objectResult;
                    }
                    else
                    {
                        var objectResult = JsonConvert.DeserializeObject<IriErrorResponse>(stringResult);
                        throw new IotaWebException(objectResult.Error);
                    }
                }
                catch (HttpRequestException ex)
                {
                    throw new IotaWebException(ex.Message, ex);
                }
            }
        }
    }
}
