using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.HttpSender
{
    public class HttpSenderFilter
    {
        public HttpSenderFilter(FilterDelegate next)
        {
            // "next" is not stored because this is a terminal filter.
        }

        public Task Invoke(FilterContext context)
        {
            throw new NotImplementedException();
        }
    }
}