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

                //TODO: Try to ping the server.

                var documents = new SampleDataProvider().GetPeople(200);

                documents.ForEach(d => {
                    var resp = esClient.Index(d, idx => idx.Index("people"));

                    Console.WriteLine(resp);
                });

                Console.WriteLine("The test data has been loaded. Please press Enter.");
                Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static ElasticClient EsClient()
        {
            var connectionSettings = new ConnectionSettings(new Uri("http://localhost:9200"));
            var elasticClient = new ElasticClient(connectionSettings);
            
            return elasticClient;
        }
    }
}
