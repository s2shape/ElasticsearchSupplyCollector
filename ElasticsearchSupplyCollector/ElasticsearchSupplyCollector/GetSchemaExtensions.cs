using Nest;
using S2.BlackSwan.SupplyCollector.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticsearchSupplyCollector
{
    public static class GetSchemaExtensions
    {
        public static List<DataEntity> GetSchema(this KeyValuePair<PropertyName, IProperty> src,
            DataContainer container,
            DataCollection collection)
        {            
            var entities = new List<DataEntity>();

            Traverse(src.Value);

            return entities;

            void Traverse(IProperty prop, string propPath = null)
            {
                var name = propPath == null ? prop.Name.Name : $"{propPath}.{prop.Name.Name}";

                if (prop.Type == "object")
                {
                    var nestedProps = GetProps(prop).ToList();
                    foreach (var p in nestedProps)
                    {
                        Traverse(p.Value, name);
                    }
                }
                else
                {
                    entities.Add(new DataEntity(name, GetDbType(prop.Type), prop.Type, container, collection));
                }
            }
        }

        private static DataType GetDbType(string type)
        {
            switch (type)
            {
                case "text":
                    return DataType.String;
                default:
                    return DataType.Unknown;
            }
        }

        private static IProperties GetProps(IProperty prop)
        {
            return ((dynamic)prop).Properties as IProperties;
        }
    }
}
