using FluentAssertions;
using S2.BlackSwan.SupplyCollector.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ElasticsearchSupplyCollector.Tests
{
    public class ElasticsearchSupplyCollectorTests
    {
        private readonly ElasticsearchSupplyCollector _sut;

        private DataContainer _container = new DataContainer
        {
            ConnectionString = "http://localhost:9200"
        };

        private readonly List<string> KNOWN_INDEXES = new List<string>{ "people" };

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

        [Fact]
        public void TestConnection()
        {
            // act
            var result = _sut.TestConnection(_container);

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void GetDataCollectionMetrics()
        {
            // act
            var result = _sut.GetDataCollectionMetrics(_container);

            // assert
            result.Select(m => m.Name).Should().BeEquivalentTo(KNOWN_INDEXES);
        }
    }
}
