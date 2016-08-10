using System;
using Meceqs.Internal;
using Xunit;

namespace Meceqs.Tests
{
    public class MessageContextFactoryTest
    {
        private IMessageContextFactory GetFactory()
        {
            return new DefaultMessageContextFactory();
        }

        [Fact]
        public void Create_throws_if_parameters_missing()
        {
            // Arrange
            var factory = GetFactory();

            Assert.Throws<ArgumentNullException>(() => factory.Create(null));
        }
    }
}