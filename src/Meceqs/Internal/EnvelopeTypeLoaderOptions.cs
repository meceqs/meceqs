using System.Collections.Generic;
using System.Reflection;

namespace Meceqs.Internal
{
    public class EnvelopeTypeLoaderOptions
    {
        public List<Assembly> ContractAssemblies { get; } = new List<Assembly>();

        public void TryAddContractAssembly(params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                if (!ContractAssemblies.Contains(assembly))
                {
                    ContractAssemblies.Add(assembly);
                }
            }
        }
    }
}
