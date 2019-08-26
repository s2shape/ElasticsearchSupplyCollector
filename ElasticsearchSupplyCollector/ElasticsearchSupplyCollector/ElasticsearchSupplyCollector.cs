using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;
using S2.BlackSwan.SupplyCollector;
using S2.BlackSwan.SupplyCollector.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticsearchSupplyCollector
{
    public class ElasticsearchSupplyCollector : SupplyCollectorBase
    {
        const int DEFAULT_SCHEMA_SAMPLE_SIZE = 10;

        public override List<string> CollectSample(DataEntity dataEntity, int sampleSize)
        {
            throw new NotImplementedException();
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

            var dataEntities = indexes.SelectMany(idx => GetSchema(idx, client));
            var dataCollections = indexes.Select(idx => new DataCollection(container, idx));

            return (dataCollections.ToList(), dataEntities.ToList());
        }

        private List<DataEntity> GetSchema(string index, ElasticClient client)
        {
            //var resp = client.LowLevel.Search<IElasticsearchResponse>(null);

            var response = client.Search<EsDocument>(s => s
                .Index(index)
                .From(0)
                .Size(DEFAULT_SCHEMA_SAMPLE_SIZE)
            );

            //client.GetMa

            var json = JsonConvert.SerializeObject(response.Documents);

            //response.
            //var resp = client.LowLevel.;


            throw new NotImplementedException();
        }

        public override bool TestConnection(DataContainer container)
        {
            var client = new ElasticsearchClientBuilder(container.ConnectionString).GetClient();

            var resp = client.Ping();

            return resp.IsValid;
        }

        private class EsDocument
        {
            public string FirstName { get; set; }
        }
    }
}
