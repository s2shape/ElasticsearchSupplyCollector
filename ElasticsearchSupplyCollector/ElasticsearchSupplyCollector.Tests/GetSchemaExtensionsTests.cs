using FluentAssertions;
using Nest;
using S2.BlackSwan.SupplyCollector.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ElasticsearchSupplyCollector.Tests
{
    public class GetSchemaExtensionsTests
    {
        //private readonly List<KeyValuePair<PropertyName, IProperty>> _mapSut = new List<KeyValuePair<PropertyName, IProperty>>
        //{
        //    new KeyValuePair<PropertyName, IProperty>(new PropertyName("Addresses"), new Property())
        //};

        [Fact]
        public void GetSchema_handles_maps()
        {
            // arrange
            var container = new DataContainer();
            var collection = new DataCollection(container, "Any");

            // act
            //var schema = _mapSut.GetSchema(container, collection);

            // assert
            var expected = new List<DataEntity>()
            {
                new DataEntity("Addresses.type1.Zip", DataType.String, "S", container, collection),
                new DataEntity("Addresses.type1.Street2", DataType.String, "S", container, collection),
                new DataEntity("Addresses.type1.Street1", DataType.String, "S", container, collection),
                new DataEntity("Addresses.type1.City", DataType.String, "S", container, collection),
                new DataEntity("Addresses.type1.State", DataType.String, "S", container, collection),

                new DataEntity("Addresses.type0.Zip", DataType.String, "S", container, collection),
                new DataEntity("Addresses.type0.Street2", DataType.String, "S", container, collection),
                new DataEntity("Addresses.type0.Street1", DataType.String, "S", container, collection),
                new DataEntity("Addresses.type0.City", DataType.String, "S", container, collection),
                new DataEntity("Addresses.type0.State", DataType.String, "S", container, collection)
            };

            //schema.Should().BeEquivalentTo(expected);
        }
    }

    //private class EsProperty : IProperty
    //{
    //    public EsProperty(IDictionary<string, object> localMetadata, PropertyName name, string type)
    //    {
    //        LocalMetadata = localMetadata;
    //        Name = name;
    //        Type = type;
    //    }

    //    public IDictionary<string, object> LocalMetadata { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public PropertyName Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public string Type { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //}
}
