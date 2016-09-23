using System;
using Meceqs.Pipeline;
using Shouldly;
using Xunit;

namespace Meceqs.Tests
{
    public class FilterContextFactoryTest
    {
        private IFilterContextFactory GetFactory()
        {
            return new DefaultFilterContextFactory();
        }

        [Fact]
        public void Throws_if_parameters_missing()
        {
            var factory = GetFactory();

            Should.Throw<ArgumentNullException>(() => factory.CreateFilterContext(null));
        }

        [Fact]
        public void Creates_FilterContext_with_same_generic_MessageType()
        {
            var envelope = TestObjects.Envelope<SimpleMessage>();
            var factory = GetFactory();

            var filterContext = factory.CreateFilterContext(envelope);

            filterContext.ShouldBeOfType<FilterContext<SimpleMessage>>();
        }

        [Fact]
        public void Creates_FilterContext_with_same_envelope()
        {
            var envelope = TestObjects.Envelope<SimpleMessage>();
            var factory = GetFactory();

            var filterContext = factory.CreateFilterContext(envelope);

            filterContext.Envelope.ShouldBe(envelope);
            filterContext.Message.ShouldBe(envelope.Message);
        }

        [Fact]
        public void Succeeds_multiple_times_for_same_envelope()
        {
            var envelope = TestObjects.Envelope<SimpleMessage>();
            var factory = GetFactory();

            factory.CreateFilterContext(envelope);
            factory.CreateFilterContext(envelope);
            factory.CreateFilterContext(envelope);
        }

        [Fact]
        public void Succeeds_multiple_times_for_different_envelopes()
        {
            var envelope1 = TestObjects.Envelope<SimpleMessage>();
            var envelope2 = TestObjects.Envelope<SimpleCommand>();
            var factory = GetFactory();

            factory.CreateFilterContext(envelope1);
            factory.CreateFilterContext(envelope2);
            factory.CreateFilterContext(envelope1);
            factory.CreateFilterContext(envelope2);
        }
    }
}