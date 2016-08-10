using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.Filters.DataAnnotationsValidator
{
    public class DataAnnotationsValidatorFilter
    {
        private readonly FilterDelegate _next;

        public DataAnnotationsValidatorFilter(FilterDelegate next)
        {
            Check.NotNull(next, nameof(next));

            _next = next;
        }

        public Task Invoke(FilterContext context)
        {
            // TODO !!! implement me
            throw new NotImplementedException();

            //return _next(context);
        }
    }
}