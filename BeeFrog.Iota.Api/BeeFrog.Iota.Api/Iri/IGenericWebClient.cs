using System.Threading;
using System.Threading.Tasks;

namespace BeeFrog.Iota.Api.Iri
{
    /// <summary>
    /// This interface abstracts a generic version of the core api that is used internally.
    /// </summary>
    public interface IGenericWebClient
    {
        /// <summary>
        /// Gets the url.
        /// </summary>
        /// <value>
        /// The hostname.
        /// </value>
        string Url { get; }

        /// <summary>
        /// Requests the specified request asynchronously.
        /// </summary>
        /// <typeparam name="TResponse">The response type you are expecting.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>Wrapped API result with the success object or failure reason.</returns>
        Task<APIResult<TResponse>> RequestAsync<TResponse>(object request, CancellationToken cancellationToken)
            where TResponse : new();

        /// <summary>
        /// Requests the specified request asynchronously.
        /// </summary>
        /// <typeparam name="TResponse">The response type you are expecting.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>Wrapped API result with the success object or failure reason.</returns>
        Task<APIResult<TResponse>> RequestAsync<TResponse>(object request)
            where TResponse : new();
    }
}
