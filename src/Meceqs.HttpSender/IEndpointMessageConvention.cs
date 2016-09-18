using System;

namespace Meceqs.HttpSender
{
    public interface IEndpointMessageConvention
    {
        EndpointMessage GetEndpointMessage(Type messageType);
    }
}