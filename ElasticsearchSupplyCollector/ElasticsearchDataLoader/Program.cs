using Nest;
using System;

namespace ElasticsearchDataLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var esClient = EsClient();

                var documents = new SampleDataProvider().GetPeople(200);

                documents.ForEach(d => {
                    var resp = esClient.Index(d, idx => idx.Index("people"));

                    Console.WriteLine(resp);
                });

                Console.WriteLine("The test data has been loaded.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static ElasticClient EsClient()
        {
            var host = Environment.GetEnvironmentVariable("ELASTIC_HOST");
            if (String.IsNullOrEmpty(host))
                host = "localhost";

            var port = Environment.GetEnvironmentVariable("ELASTIC_PORT");
            if (String.IsNullOrEmpty(port)) {
                port = "9200";
            }

            var connectionSettings = new ConnectionSettings(new Uri($"http://{host}:{port}"));
            var elasticClient = new ElasticClient(connectionSettings);
            
            return elasticClient;
        }
    }
}
