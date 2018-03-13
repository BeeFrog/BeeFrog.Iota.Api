using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeeFrog.Iota.Api.NodeLookup
{
    public interface INodeFinder
    {
        /// <summary>
        /// Gets a list of nodes
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Node>> FindNodes();

        /// <summary>
        /// Finds the best performing node and returns it's url.
        /// </summary>
        /// <returns></returns>
        Task<string> GetBestNode();
    }
}