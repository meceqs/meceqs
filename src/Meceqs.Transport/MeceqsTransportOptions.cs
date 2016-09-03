using System.Collections.Generic;
using System.Reflection;

namespace Meceqs.Serialization
{
    public class MeceqsSerializationOptions
    {
        public List<Assembly> ContractAssemblies { get; set; } = new List<Assembly>();
    }
}