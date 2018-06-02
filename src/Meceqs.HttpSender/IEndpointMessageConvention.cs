using System;

namespace Meceqs.HttpSender
{
    /// <summary>
    /// Allows to override the mapping between message types and endpoint URIs.
    /// </summary>
    public interface IEndpointMessageConvention
    {
        string GetRelativePath(Type messageType);
    }
}