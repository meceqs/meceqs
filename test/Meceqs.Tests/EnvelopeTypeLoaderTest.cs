using System;
using System.Reflection;
using Xunit;

namespace Meceqs.Tests
{
    public class EnvelopeTypeLoaderTest
    {
        private IEnvelopeTypeLoader GetEnvelopeTypeLoader()
        {
            var typeLoader = new DefaultEnvelopeTypeLoader();
            typeLoader.AddContractAssemblies(Assembly.GetExecutingAssembly());

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