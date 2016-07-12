using System;
using System.Reflection;

namespace Meceqs
{
    public interface IEnvelopeTypeLoader
    {
        void AddContractAssemblies(params Assembly[] assemblies);

        Type LoadEnvelopeType(string messageType);
    }
}