using System.Collections.Generic;
using System.Threading.Tasks;

namespace Borlay.Iota.Library.NodeLookup
{
    public interface INodeFinder
    {
        Task<IEnumerable<Node>> FindNodes();
    }
}