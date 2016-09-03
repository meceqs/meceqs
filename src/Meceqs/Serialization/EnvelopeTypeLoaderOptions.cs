using System.Collections.Generic;
using System.Reflection;

namespace Meceqs.Serialization
{
    public class EnvelopeTypeLoaderOptions
    {
        public List<Assembly> ContractAssemblies { get; set; } = new List<Assembly>();
    }
}