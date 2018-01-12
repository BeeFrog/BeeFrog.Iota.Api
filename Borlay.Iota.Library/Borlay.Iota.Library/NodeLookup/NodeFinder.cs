using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Borlay.Iota.Library.NodeLookup
{
    public class IotaDanceNodeFinder : INodeFinder
    {
        public async Task<IEnumerable<Node>> FindNodes()
        {
            var client = new System.Net.Http.HttpClient();
            string jsonResults = await client.GetStringAsync("https://iota.dance/data/node-stats");
            var nodes = Newtonsoft.Json.JsonConvert.DeserializeObject<Node[]>(jsonResults);

            return nodes?.OrderByDescending(n => n.health);
        }
    }
}
