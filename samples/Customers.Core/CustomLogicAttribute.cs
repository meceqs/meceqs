using System;

namespace CustomerContext.Core
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CustomLogicAttribute : Attribute
    {
    }
}