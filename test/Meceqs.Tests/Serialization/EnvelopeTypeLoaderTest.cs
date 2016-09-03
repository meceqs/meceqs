using System;
using System.Reflection;
using Meceqs.Transport;
using Microsoft.Extensions.Options;
using Xunit;

namespace Meceqs.Tests
{
    public class EnvelopeTypeLoaderTest
    {
        private IEnvelopeTypeLoader GetEnvelopeTypeLoader()
        {
            var options = new MeceqsTransportOptions();
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