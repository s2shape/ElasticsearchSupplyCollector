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

        private readonly DataContainer _container = new DataContainer
        {
            ConnectionString = "http://localhost:9200"
        };

        private readonly DataCollection _collection;

        private readonly List<string> KNOWN_INDEXES = new List<string>{ "people" };

        public ElasticsearchSupplyCollectorTests()
        {
            _sut = new ElasticsearchSupplyCollector();
            _collection = new DataCollection(_container, "people");
        }

        [Fact]
        public void GetSchema_contains_nested_object_leaf_properties()
        {
            var (collections, entities) = _sut.GetSchema(_container);
                       
            var expected = new List<DataEntity>()
            {
                new DataEntity("addresses.type1.zip", DataType.String, "text", _container, _collection),
                new DataEntity("addresses.type1.street2", DataType.String, "text", _container, _collection),
                new DataEntity("addresses.type1.street1", DataType.String, "text", _container, _collection),
                new DataEntity("addresses.type1.city", DataType.String, "text", _container, _collection),
                new DataEntity("addresses.type1.state", DataType.String, "text", _container, _collection),
                                
                new DataEntity("addresses.type0.zip", DataType.String, "text", _container, _collection),
                new DataEntity("addresses.type0.street2", DataType.String, "text", _container, _collection),
                new DataEntity("addresses.type0.street1", DataType.String, "text", _container, _collection),
                new DataEntity("addresses.type0.city", DataType.String, "text", _container, _collection),
                new DataEntity("addresses.type0.state", DataType.String, "text", _container, _collection)
            };
            var expectedNames = expected.Select(x => x.Name).ToList();

            entities
                .Where(e => expectedNames.Contains(e.Name))
                .Should().BeEquivalentTo(expected);
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
