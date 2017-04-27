using System;
using System.Threading.Tasks;
using Meceqs.TypedHandling;
using Meceqs.TypedHandling.Configuration;
using Xunit;

namespace Meceqs.Tests.TypedHandling
{
    public class HandlerCollectionTest
    {
        public class HandlerOne : IHandles<SimpleMessage, int>
        {
            public Task<int> HandleAsync(HandleContext<SimpleMessage> context)
            {
                return Task.FromResult(0);
            }
        }

        public class HandlerTwo : IHandles<SimpleMessage, int>
        {
            public Task<int> HandleAsync(HandleContext<SimpleMessage> context)
            {
                return Task.FromResult(0);
            }
        }

        [Fact]
        public void Adding_the_same_handler_twice_throws()
        {
            HandlerCollection collection = new HandlerCollection();
            collection.Add<HandlerOne>();

            Assert.Throws<InvalidOperationException>(() => collection.Add<HandlerOne>());
        }

        [Fact]
        public void Adding_a_handler_with_handles_from_other_handler_throws()
        {
            HandlerCollection collection = new HandlerCollection();
            collection.Add<HandlerOne>();

            Assert.Throws<InvalidOperationException>(() => collection.Add<HandlerTwo>());
        }
    }
}
