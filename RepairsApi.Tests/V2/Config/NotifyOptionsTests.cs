using FluentAssertions;
using NUnit.Framework;
using RepairsApi.V2.Email;
using System.Collections.Generic;

namespace RepairsApi.Tests.V2.Config
{
    public class NotifyOptionsTests
    {
        [Test]
        public void MapsString()
        {
            var testString = "key1:value1,key2:value2,key3:value3";

            var classUnderTest = new NotifyOptions
            {
                TemplateIds = testString
            };

            var formattedIds = classUnderTest.GetTemplateIds();

            formattedIds.Should().BeEquivalentTo(new Dictionary<string, string>
            {
                { "key1", "value1"},
                { "key2", "value2"},
                { "key3", "value3"},
            });
        }
    }
}
