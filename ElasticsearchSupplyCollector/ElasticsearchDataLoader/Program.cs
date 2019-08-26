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

                Console.WriteLine("The data has been loaded!");
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
