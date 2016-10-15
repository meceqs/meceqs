using System;

namespace Meceqs.AspNetCore.Consuming
{
    public interface IMessagePathConvention
    {
        string GetPathForMessage(Type messageType);
    }
}