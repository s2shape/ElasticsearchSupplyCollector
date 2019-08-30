using Nest;
using System;

namespace ElasticsearchSupplyCollector
{
    class ElasticsearchClientBuilder
    {
        private string _connectionString;

        public ElasticsearchClientBuilder(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ElasticClient GetClient()
        {
            var connectionSettings = new ConnectionSettings(new Uri(_connectionString));
            var elasticClient = new ElasticClient(connectionSettings);
            
            return elasticClient;
        }
    }
}
