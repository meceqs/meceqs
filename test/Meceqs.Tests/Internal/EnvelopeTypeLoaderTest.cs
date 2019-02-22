using System;
using System.Reflection;
using Meceqs.Internal;
using Microsoft.Extensions.Options;
using Xunit;

namespace Meceqs.Tests.Internal
{
    public class EnvelopeTypeLoaderTest
    {
        private IEnvelopeTypeLoader GetEnvelopeTypeLoader()
        {
            var options = new EnvelopeTypeLoaderOptions();
            options.ContractAssemblies.Add(GetType().GetTypeInfo().Assembly);
            var typeLoader = new DefaultEnvelopeTypeLoader(Options.Create(options));

            return typeLoader;
        }

        [Fact]
        public void SucceedsForTypeFromSameAssembly()
        {
            // Arrange
            var typeLoader = GetEnvelopeTypeLoader();
            string messageType = typeof(SimpleMessage).FullName;

            // Act
            Type envelopeType = typeLoader.LoadEnvelopeType(messageType);

            // Assert
            Assert.Equal(typeof(Envelope<SimpleMessage>), envelopeType);
        }
    }
}
