using System;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Customers.Hosts.WebApi.Infrastructure
{
    /// <summary>
    /// Makes sure, every complex type is bound from HTTP body. This way, using [FromBody] is no longer necessary.
    /// http://benfoster.io/blog/aspnet-core-customising-model-binding-conventions
    /// </summary>
    public class BindComplexTypeFromBodyConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            // IFormFile kann nicht gebindet werden --> UnsupportedMediaType
            Type formFileType = typeof(IFormFile);
            foreach (var parameter in action.Parameters)
            {
                var paramType = parameter.ParameterInfo.ParameterType;
                if (parameter.BindingInfo == null && !IsSimpleType(paramType) && paramType != formFileType)
                {
                    parameter.BindingInfo = new BindingInfo
                    {
                        BindingSource = BindingSource.Body
                    };
                }
            }
        }

        private static bool IsSimpleType(Type target)
        {
            Type type = Nullable.GetUnderlyingType(target) ?? target;

            return type.GetTypeInfo().IsPrimitive ||
                   type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(Guid) ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(TimeSpan);
        }
    }
}