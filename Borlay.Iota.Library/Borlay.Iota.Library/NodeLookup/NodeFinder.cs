using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Borlay.Iota.Library.NodeLookup
{
    public class IotaDanceNodeFinder : INodeFinder
    {
        public async Task<IEnumerable<Node>> FindNodes()
        {
            try
            {
                var client = new System.Net.Http.HttpClient();
                string jsonResults = await client.GetStringAsync("https://iota.dance/data/node-stats");
                var nodes = Newtonsoft.Json.JsonConvert.DeserializeObject<Node[]>(jsonResults);
                this.CacheResults(jsonResults);
                return nodes?.OrderByDescending(n => n.health);
            }
            catch (HttpRequestException)
            {
                // Fall-back to well known nodes
                // TODO: cache the list and reload of possible.
                var nodes = new Node[] {
                    new Node() { node = "http://iotanode.party", port = "14265" },
                    new Node() { node = "http://node01.iotatoken.nl", port = "14265" },
                    new Node() { node = "http://iri1.iota.fm", port = "80" },
                    new Node() { node = "http://iri3.iota.fm", port = "80" },
                    new Node() { node = "https://nodes.iota.cafe", port = "443" },
                    //new Node() { node = "https://nodes.iota.cafe", port = "443" },

                };
                return nodes;
            }
        }

        private void CacheResults(string jsonResults)
        {
            //TODO.
        }
    }
}
