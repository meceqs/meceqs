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

            dict.Get<string>(keyForGet).ShouldBe("dummy");
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

            dict[keyForGet].ShouldBe("dummy");
        }

        [Fact]
        public void Get_returns_0_for_int_if_key_not_found()
        {
            var dict = new EnvelopeProperties();
            dict.Add("some-key", 4);

            dict.Get<int>("different-key").ShouldBe(0);
        }

        [Fact]
        public void Get_returns_null_for_string_if_key_not_found()
        {
            var dict = new EnvelopeProperties();
            dict.Add("some-key", "dummy");

            dict.Get<string>("different-key").ShouldBeNull();
        }

        [Fact]
        public void Get_returns_null_for_NullableDate_if_key_not_found()
        {
            var dict = new EnvelopeProperties();
            dict.Add("some-key", DateTime.Now);

            dict.Get<DateTime?>("different-key").ShouldBeNull();
        }

        [Fact]
        public void Get_returns_default_for_Date_if_key_not_found()
        {
            var dict = new EnvelopeProperties();
            dict.Add("some-key", DateTime.Now);

            dict.Get<DateTime>("different-key").ShouldBe(default(DateTime));
        }

        [Fact]
        public void Get_converts_int_to_double()
        {
            var dict = new EnvelopeProperties();
            dict.Add("key", 4);

            dict.Get<double>("key").ShouldBe(4.0);
        }

        [Fact]
        public void Get_converts_double_to_int()
        {
            var dict = new EnvelopeProperties();
            dict.Add("key", 4.3);

            dict.Get<int>("key").ShouldBe(4);
        }

        [Fact]
        public void Get_converts_string_to_int()
        {
            var dict = new EnvelopeProperties();
            dict.Add("key", "4");

            dict.Get<int>("key").ShouldBe(4);
        }

        [Fact]
        public void Get_converts_InvariantDouble_string_to_double()
        {
            var dict = new EnvelopeProperties();
            dict.Add("key", "4.3");

            dict.Get<double>("key").ShouldBe(4.3);
        }

        [Fact]
        public void Get_converts_InvariantDateString_string_to_UtcDateTime()
        {
            var dict = new EnvelopeProperties();
            dict.Add("key", "2016-04-23T18:25:43.511Z");

            dict.Get<DateTime>("key").ShouldBe(new DateTime(2016, 4, 23, 18, 25, 43, 511, DateTimeKind.Utc));
        }

        [Fact]
        public void Get_converts_InvariantDateString_string_to_UtcDateTimeOffset()
        {
            var dict = new EnvelopeProperties();
            dict.Add("key", "2016-04-23T18:25:43.511Z");

            dict.Get<DateTimeOffset>("key")
                .ShouldBe(new DateTimeOffset(new DateTime(2016, 4, 23, 18, 25, 43, 511, DateTimeKind.Utc)));
        }

        [Fact]
        public void GetRequired_throws_if_key_is_not_present()
        {
            var dict = new EnvelopeProperties();
            dict.Add("key", "value");

            Should.Throw<ArgumentOutOfRangeException>(() => dict.GetRequired<string>("wrong-key"));
        }

        [Fact]
        public void GetRequired_succeeds_if_key_exists_with_struct_default_value()
        {
            var dict = new EnvelopeProperties();
            dict.Add("key", 0);

            dict.GetRequired<int>("key").ShouldBe(0);
        }

        [Fact]
        public void GetRequired_succeeds_if_key_exists_with_null_value()
        {
            var dict = new EnvelopeProperties();
            dict.Add("key", null);

            dict.GetRequired<string>("key").ShouldBeNull();
        }
    }
}
