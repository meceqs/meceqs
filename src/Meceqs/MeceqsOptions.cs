using System.Collections.Generic;
using System.Reflection;

namespace Meceqs
{
    public class MeceqsOptions
    {
        public string ApplicationName { get; set; }

        public string HostName { get; set; }

        public List<Assembly> ContractAssemblies { get; set; } = new List<Assembly>();
    }
}