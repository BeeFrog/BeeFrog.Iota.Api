using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeeFrog.Iota.Api.NodeLookup
{
    public interface INodeFinder
    {
        Task<IEnumerable<Node>> FindNodes();
    }
}