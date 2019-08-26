using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace ElasticsearchSupplyCollector.Tests
{
    public class ElasticsearchSupplyCollectorTests
    {
        private readonly ElasticsearchSupplyCollector _sut;

        public ElasticsearchSupplyCollectorTests()
        {
            _sut = new ElasticsearchSupplyCollector();
        }

        [Fact]
        public void DataStoreTypes_returns_elasticsearch()
        {
            // act
            var result = _sut.DataStoreTypes();

            var expected = new List<string> { "Elasticsearch" };

            result.Should().BeEquivalentTo(expected);
        }
    }
}
