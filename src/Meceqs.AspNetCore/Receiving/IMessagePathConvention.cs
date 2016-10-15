using System;

namespace Meceqs.AspNetCore.Receiving
{
    public interface IMessagePathConvention
    {
        string GetPathForMessage(Type messageType);
    }
}