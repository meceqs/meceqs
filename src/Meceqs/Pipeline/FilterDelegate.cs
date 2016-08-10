using System.Threading.Tasks;

namespace Meceqs.Pipeline
{
    public delegate Task FilterDelegate(FilterContext context);
}