using System;
using System.Collections.Generic;
using System.Dynamic;
using Nest;
using S2.BlackSwan.SupplyCollector.Models;
using SupplyCollectorDataLoader;

namespace ElasticsearchSupplyCollectorLoader
{
    public class ElasticsearchSupplyCollectorLoader : SupplyCollectorDataLoaderBase
    {
        private static ElasticClient Connect(string connectString)
        {
            var connectionSettings = new ConnectionSettings(new Uri(connectString));
            return new ElasticClient(connectionSettings);
        }

        public override void InitializeDatabase(DataContainer dataContainer) {
            
        }

        public override void LoadSamples(DataEntity[] dataEntities, long count) {
            var client = Connect(dataEntities[0].Container.ConnectionString);

            long rows = 0;
            var r = new Random();
            while (rows < count) {
                if (rows % 1000 == 0) {
                    Console.Write(".");
                }
                dynamic doc = new ExpandoObject();
                var docFields = doc as IDictionary<string, Object>;
                foreach (var dataEntity in dataEntities) {
                    switch (dataEntity.DataType) {
                        case DataType.String:
                            docFields[dataEntity.Name] = new Guid().ToString();
                            break;
                        case DataType.Int:
                            docFields[dataEntity.Name] = r.Next();
                            break;
                        case DataType.Double:
                            docFields[dataEntity.Name] = r.NextDouble();
                            break;
                        case DataType.Boolean:
                            docFields[dataEntity.Name] = r.Next(100) > 50;
                            break;
                        case DataType.DateTime:
                            docFields[dataEntity.Name] = DateTimeOffset
                                .FromUnixTimeMilliseconds(
                                    DateTimeOffset.Now.ToUnixTimeMilliseconds() + r.Next()).DateTime;
                            break;
                        default:
                            docFields[dataEntity.Name] = r.Next();
                            break;
                    }
                }

                client.Index((object)doc, i => i
                    .Index(dataEntities[0].Collection.Name)
                    //.Type(dataEntities[0].Collection.Name + "_Type")
                    .Id(rows)
                );

                rows++;
            }
            Console.WriteLine();
        }

        public override void LoadUnitTestData(DataContainer dataContainer) {
            var client = Connect(dataContainer.ConnectionString);

            var documents = new UnitTestDataProvider().GetPeople(200);

            documents.ForEach(d => {
                var resp = client.Index(d, idx => idx.Index("people"));

                Console.WriteLine(resp);
            });
        }
    }
}
