using System;
using System.Reflection;
using Meceqs.Serialization;
using Microsoft.Extensions.Options;
using Xunit;

namespace Meceqs.Tests
{
    public class EnvelopeTypeLoaderTest
    {
        private IEnvelopeTypeLoader GetEnvelopeTypeLoader()
        {
            var options = new MeceqsSerializationOptions();
            options.ContractAssemblies.Add(Assembly.GetExecutingAssembly());
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