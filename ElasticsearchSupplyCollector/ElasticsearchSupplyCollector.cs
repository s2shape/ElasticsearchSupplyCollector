using System;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using S2.BlackSwan.SupplyCollector;
using S2.BlackSwan.SupplyCollector.Models;
using System.Collections.Generic;
using System.Linq;
using Elasticsearch.Net;

namespace ElasticsearchSupplyCollector
{
    public class ElasticsearchSupplyCollector : SupplyCollectorBase
    {
        public override List<string> CollectSample(DataEntity dataEntity, int sampleSize)
        {
            var client = new ElasticsearchClientBuilder(dataEntity.Container.ConnectionString).GetClient();

            string jsonQuery = BuildQuery(dataEntity, sampleSize);

            string index = dataEntity.Collection.Name;
            var searchResponse = client.LowLevel.Search<SearchResponse<dynamic>>(index, jsonQuery);

            var samples = searchResponse.Documents
                .Select(d => JsonConvert.DeserializeObject(d.ToString()))
                .OfType<JToken>()
                .SelectMany(x => x.CollectSample(dataEntity.Name))
                .ToList();

            return samples;
        }

        private static string BuildQuery(DataEntity dataEntity, int sampleSize)
        {
            var rawQuery = new
            {
                size = sampleSize,
                _source = new[] { dataEntity.Name },
                // this a native way to take random data from the ES
                query = new
                {
                    function_score = new
                    {
                        query = new
                        {
                            match_all = new { }
                        },
                        functions = new[]
                                    {
                            new { random_score = new { } }
                        }
                    }
                }
            };

            var jsonQuery = JsonConvert.SerializeObject(rawQuery);
            return jsonQuery;
        }

        public override List<string> DataStoreTypes()
        {
            return new List<string>() { "Elasticsearch" };
        }

        public override List<DataCollectionMetrics> GetDataCollectionMetrics(DataContainer container)
        {
            var client = new ElasticsearchClientBuilder(container.ConnectionString).GetClient();

            var indices = client.Cat.Indices();

            var metrics = indices.Records
                .Where(idx => idx.Index != ".kibana") // this index has been created by Kibana (GUI for Elasticsearch) automatically.
                .Select(idx =>
            {
                long.TryParse(idx.DocsCount, out var docsCount);
                decimal.TryParse(idx.StoreSize.Replace("kb", ""), out var storeSize);

                return new DataCollectionMetrics
                {
                    Name = idx.Index,
                    RowCount = docsCount,
                    TotalSpaceKB = storeSize,
                    UsedSpaceKB = storeSize
                };
            }).ToList();

            return metrics;
        }
        public override (List<DataCollection>, List<DataEntity>) GetSchema(DataContainer container)
        
        {
            var indexes = GetDataCollectionMetrics(container)
                .Select(m => m.Name)
                .ToList();

            var client = new ElasticsearchClientBuilder(container.ConnectionString).GetClient();

            var mappingsList = new List<string>();

            foreach(var index in indexes)
            {
                var response = client.LowLevel.Indices.GetMapping<StringResponse>(index);
                var jsonMapping = response.Body;
                mappingsList.Add(jsonMapping);
            }

            var jsonMappings = mappingsList.ToArray();

            List<DataEntity> dataEntities = new List<DataEntity>();
            foreach (var mappings in jsonMappings)
            {
             dataEntities.AddRange(indexes.SelectMany(idx => GetSchema(idx, mappings, container)));
            }

            var dataCollections = indexes.Select(idx => new DataCollection(container, idx));

            return (dataCollections.ToList(), dataEntities.ToList());
        }

        private List<DataEntity> GetSchema(string index,
            string mappings,
            DataContainer container)
        {
            var entityNames = EntitiesFromJson(index, mappings);

            var collection = new DataCollection(container, index);

            var dataEntities = new List<DataEntity>();
            foreach (var name in entityNames)
            {
                dataEntities.Add(
                new DataEntity(name, DataType.String, "string", container, collection));
            }

            return dataEntities;
        }
        private List<string> EntitiesFromJson(string indexName, string json)
        {
            var jsonParser = JObject.Parse(json);
            var subObject = jsonParser.SelectToken($"$.{indexName}.mappings.properties");
            var jsonString = subObject.ToString();
            var responseDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
            return responseDictionary.Keys.ToList();
        }
        public override bool TestConnection(DataContainer container)
        {
            var client = new ElasticsearchClientBuilder(container.ConnectionString).GetClient();

            var resp = client.Ping();

            return resp.IsValid;
        }
    }
}
