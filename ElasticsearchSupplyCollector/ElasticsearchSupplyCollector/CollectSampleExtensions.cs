using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace ElasticsearchSupplyCollector
{
    public static class CollectSampleExtensions
    {
        public static string CollectSample(JObject src, string name)
        {
            var currentObject = src;
            var currentName = name;
            while (true)
            {
                if (IsNestedObject(currentName))
                {
                    var namePath = currentName.Split(".");
                    string subPath = string.Join('.', namePath.Skip(1));

                    string rootName = namePath.First();

                    currentName = subPath;
                    currentObject = (JObject)currentObject[rootName];
                }
                else
                {
                    return currentObject[currentName].Value<string>();
                }
            }         
        }

        private static bool IsNestedObject(string name) => name.Contains(".");
    }
}
