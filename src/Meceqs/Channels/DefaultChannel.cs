using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.Channels
{
    public class DefaultChannel : IChannel
    {
        private readonly FilterDelegate _pipeline;

        public DefaultChannel(ChannelOptions options)
        {
            Check.NotNull(options, nameof(options));
            Check.NotNull(options.PipelineBuilder, $"{nameof(options)}.{nameof(options.PipelineBuilder)}");

            _pipeline = options.Pipeline;
        }

        public Task SendAsync(IList<FilterContext> contexts)
        {
            return SendAsync<VoidType>(contexts);
        }

        public async Task<TResult> SendAsync<TResult>(IList<FilterContext> contexts)
        {
            // TODO @cweiss does this make sense?

            if (contexts.Count == 0)
            {
                return await Task.FromResult(default(TResult));
            }
            else if (contexts.Count == 1)
            {
                return await SendAsync<TResult>(contexts[0]);
            }
            else
            {
                if (typeof(TResult) != typeof(VoidType))
                    throw new InvalidOperationException("SendAsync with many contexts can only be used with return-type 'VoidType'");

                foreach (var context in contexts)
                {
                    await SendAsync<TResult>(context);
                }

                return await Task.FromResult(default(TResult));
            }
        }

        private async Task<TResult> SendAsync<TResult>(FilterContext context)
        {
            Check.NotNull(context, nameof(context));

            context.ExpectedResultType = typeof(TResult);

            await _pipeline(context);

            return (TResult)context.Result;
        }
    }
}