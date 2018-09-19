using System;

namespace Customers.Core
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CustomLogicAttribute : Attribute
    {
    }
}
