using System;
using Meceqs.Pipeline;
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
        public void Create_throws_if_parameters_missing()
        {
            // Arrange
            var factory = GetFactory();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => factory.CreateFilterContext(null));
        }
    }
}