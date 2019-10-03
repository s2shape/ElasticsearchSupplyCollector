using System;
using FluentAssertions;
using S2.BlackSwan.SupplyCollector.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ElasticsearchSupplyCollectorTests
{
    public class ElasticsearchSupplyCollectorTests : IClassFixture<LaunchSettingsFixture>
    {
        private readonly ElasticsearchSupplyCollector.ElasticsearchSupplyCollector _sut;
        private readonly DataContainer _container;
        private readonly DataCollection _collection;
        private LaunchSettingsFixture _fixture;

        private readonly List<string> KNOWN_INDEXES = new List<string> { "people" };

        public ElasticsearchSupplyCollectorTests(LaunchSettingsFixture fixture) {
            _fixture = fixture;

            var host = Environment.GetEnvironmentVariable("ELASTIC_HOST");
            var port = Environment.GetEnvironmentVariable("ELASTIC_PORT");

            _sut = new ElasticsearchSupplyCollector.ElasticsearchSupplyCollector();
            _container = new DataContainer
            {
                ConnectionString = $"http://{host}:{port}"
            };
            _collection = new DataCollection(_container, "people");
        }

        [Fact]
        public void CollectSample_nested_property()
        {
            // arrange
            var zipEntity = new DataEntity("addresses.type1.zip", DataType.String, "text", _container, _collection);
            const int sampleSize = 10;

            // act
            var result = _sut.CollectSample(zipEntity, sampleSize);

            // assert
            result.Should().HaveCountLessOrEqualTo(sampleSize);
            result.Should().OnlyContain(x => x == "Zip1");
        }

        [Fact]
        public void CollectSample_list_of_objects()
        {
            // arrange
            var countryCode = new DataEntity("phoneNumbers.countryCode", DataType.String, "text", _container, _collection);
            var knowCountryCodes = new[] { "CountryCode1", "CountryCode2", "CountryCode0" };
            const int sampleSize = 10;

            // act
            var result = _sut.CollectSample(countryCode, sampleSize);

            // assert
            result.Should().HaveCountLessOrEqualTo(30);
            result.Should().OnlyContain(x => knowCountryCodes.Contains(x));
        }

        [Fact]
        public void CollectSample_simple_list()
        {
            // arrange
            var simpleInts = new DataEntity("simpleInts", DataType.Long, "long", _container, _collection);
            var knowInts = new[] { "1", "2", "3" };

            // act
            var result = _sut.CollectSample(simpleInts, 10);

            // assert
            result.Should().HaveCountLessOrEqualTo(30);
            result.Should().OnlyContain(x => knowInts.Contains(x));
        }

        [Fact]
        public void GetSchema_returns_collections()
        {
            var knowCollections = new List<DataCollection> { new DataCollection(_container, "people") };

            var (collections, _) = _sut.GetSchema(_container);

            collections.Should().BeEquivalentTo(knowCollections);
        }

        [Fact]
        public void GetSchema_nested_object_leaf_properties()
        {
            // act
            var (_, entities) = _sut.GetSchema(_container);

            // assert
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
        public void GetSchema_list_of_objects()
        {
            // act
            var (_, entities) = _sut.GetSchema(_container);

            // assert
            var expected = new List<DataEntity>()
            {
                new DataEntity("phoneNumbers.countryCode", DataType.String, "text", _container, _collection),
                new DataEntity("phoneNumbers.number", DataType.String, "text", _container, _collection),
                new DataEntity("phoneNumbers.type", DataType.String, "text", _container, _collection)
            };
            var expectedNames = expected.Select(x => x.Name).ToList();

            var result = entities
                .Where(e => expectedNames.Contains(e.Name))
                .ToList();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetSchema_sipmle_list()
        {
            // act
            var (_, entities) = _sut.GetSchema(_container);

            // assert
            var expected = new List<DataEntity>()
            {
                new DataEntity("simpleStrings", DataType.String, "text", _container, _collection),
                new DataEntity("simpleInts", DataType.Long, "long", _container, _collection)
            };
            var expectedNames = expected.Select(x => x.Name).ToList();

            var result = entities
                .Where(e => expectedNames.Contains(e.Name))
                .ToList();

            result.Should().BeEquivalentTo(expected);
        }
        
        [Fact]
        public void DataStoreTypes_returns_elasticsearch()
        {
            // act
            var result = _sut.DataStoreTypes();

            // assert
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
