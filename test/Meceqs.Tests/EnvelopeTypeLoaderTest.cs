using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Meceqs.Tests
{
    public class EnvelopeTypeLoaderTest
    {
        private IEnvelopeTypeLoader GetEnvelopeTypeLoader()
        {
            var assemblies = new List<Assembly>
            {
                Assembly.GetExecutingAssembly()
            };

            return new DefaultEnvelopeTypeLoader(assemblies);
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