using System.Collections.Generic;
using System.Reflection;

namespace Meceqs.Transport
{
    public class MeceqsTransportOptions
    {
        public List<Assembly> ContractAssemblies { get; set; } = new List<Assembly>();
    }
}