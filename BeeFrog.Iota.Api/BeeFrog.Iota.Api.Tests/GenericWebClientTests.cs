using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BeeFrog.Iota.Api.Iri;
using BeeFrog.Iota.Api.Exceptions;
using System.Threading.Tasks;
using BeeFrog.Iota.Api.Iri.Dto;

namespace BeeFrog.Iota.Api.Tests
{
    [TestClass]
    public class GenericWebClientTests
    {
        [TestMethod]
        public async Task GetTipsRequestTest()
        {
            var client = CreateGenericClient();
            var getTipsResponse = await client.RequestAsync<GetTipsResponse>(new GetTipsRequest());

            Assert.IsNotNull(getTipsResponse);
            Assert.IsTrue(getTipsResponse.Hashes.Length > 0);
        }

        [TestMethod]
        [ExpectedException(typeof(IotaWebException))]
        public async Task BadCommandTest()
        {
            var client = CreateGenericClient();
            var getTipsResponse = await client.RequestAsync<IriResponseBase>(new IriRequestBase("bad_command"));
        }

        private GenericWebClient CreateGenericClient()
        {
            return new GenericWebClient("http://node.iotawallet.info:14265");
        }
    }
}
