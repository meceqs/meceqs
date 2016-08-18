using System;
using Shouldly;
using Xunit;

namespace Meceqs.Tests
{
    public class EnvelopePropertiesTest
    {
        [Theory]
        [InlineData("somekey", "Somekey")]
        [InlineData("somekey", "SomeKey")]
        [InlineData("somekey", "someKey")]
        [InlineData("someKey", "somekey")]
        [InlineData("someKey", "Somekey")]
        [InlineData("SomeKey", "Somekey")]
        [InlineData("SomeKey", "somekey")]
        public void Get_succeeds_if_case_is_different(string keyInDict, string keyForGet)
        {
            var dict = new EnvelopeProperties();
            dict.Add(keyInDict, "dummy");

            var result = dict.Get<string>(keyForGet);

            result.ShouldBe("dummy");
        }

        [Theory]
        [InlineData("somekey", "Somekey")]
        [InlineData("somekey", "SomeKey")]
        [InlineData("somekey", "someKey")]
        [InlineData("someKey", "somekey")]
        [InlineData("someKey", "Somekey")]
        [InlineData("SomeKey", "Somekey")]
        [InlineData("SomeKey", "somekey")]
        public void ItemAccessor_succeeds_if_case_is_different(string keyInDict, string keyForGet)
        {
            var dict = new EnvelopeProperties();
            dict.Add(keyInDict, "dummy");

            var result = dict[keyForGet];

            result.ShouldBe("dummy");
        }

        [Fact]
        public void Get_returns_0_for_int_if_key_not_found()
        {
            var dict = new EnvelopeProperties();
            dict.Add("some-key", 4);

            var result = dict.Get<int>("different-key");

            result.ShouldBe(0);
        }

        [Fact]
        public void Get_returns_null_for_string_if_key_not_found()
        {
            var dict = new EnvelopeProperties();
            dict.Add("some-key", "dummy");

            var result = dict.Get<string>("different-key");

            result.ShouldBe(null);
        }

        [Fact]
        public void Get_returns_null_for_NullableDate_if_key_not_found()
        {
            var dict = new EnvelopeProperties();
            dict.Add("some-key", DateTime.Now);

            var result = dict.Get<DateTime?>("different-key");

            result.ShouldBe(null);
        }

        [Fact]
        public void Get_returns_default_for_Date_if_key_not_found()
        {
            var dict = new EnvelopeProperties();
            dict.Add("some-key", DateTime.Now);

            var result = dict.Get<DateTime>("different-key");

            result.ShouldBe(default(DateTime));
        }

        [Fact]
        public void Get_converts_int_to_double()
        {
            var dict = new EnvelopeProperties();
            dict.Add("key", 4);

            var result = dict.Get<double>("key");

            result.ShouldBe(4);
        }

        [Fact]
        public void Get_converts_double_to_int()
        {
            var dict = new EnvelopeProperties();
            dict.Add("key", 4.3);

            var result = dict.Get<int>("key");

            result.ShouldBe(4);
        }

        [Fact]
        public void Get_converts_string_to_int()
        {
            var dict = new EnvelopeProperties();
            dict.Add("key", "4");

            var result = dict.Get<int>("key");

            result.ShouldBe(4);
        }

        [Fact]
        public void Get_converts_InvariantDouble_string_to_double()
        {
            var dict = new EnvelopeProperties();
            dict.Add("key", "4.3");

            var result = dict.Get<double>("key");

            result.ShouldBe(4.3);
        }

        [Fact]
        public void Get_converts_InvariantDateString_string_to_DateTime()
        {
            var dict = new EnvelopeProperties();
            dict.Add("key", "2016-04-23T18:25:43.511Z");

            var result = dict.Get<DateTime>("key");

            result.ShouldBe(new DateTime(2016, 4, 23, 18, 25, 43, 511));
        }

        [Fact]
        public void Get_converts_InvariantDateString_string_to_DateTimeOffset()
        {
            var dict = new EnvelopeProperties();
            dict.Add("key", "2016-04-23T18:25:43.511Z");

            var result = dict.Get<DateTimeOffset>("key");

            result.ShouldBe(new DateTimeOffset(new DateTime(2016, 4, 23, 18, 25, 43, 511, DateTimeKind.Utc)));
        }
    }
}