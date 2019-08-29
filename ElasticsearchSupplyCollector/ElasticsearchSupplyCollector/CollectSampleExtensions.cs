using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace ElasticsearchSupplyCollector
{
    public static class CollectSampleExtensions
    {
        public static List<string> CollectSample(this JToken src, string name)
        {
            if (src == null)
                return new List<string>();

            JToken currentObject = src;
            var currentName = name;

            var results = new List<string>();
            while (true)
            {
                if (currentObject == null)
                    break;

                if (currentObject.Type == JTokenType.Object)
                {
                    var namePath = currentName.Split(".");
                    string subPath = string.Join('.', namePath.Skip(1));

                    string rootName = namePath.First();

                    currentName = subPath;
                    currentObject = currentObject[rootName];
                }
                else if(currentObject.Type == JTokenType.Array)
                {
                    var array = (JArray)currentObject;

                    var values = array.SelectMany(x => CollectSample(x, currentName)).ToList();
                    results.AddRange(values);

                    break;
                }
                else
                {
                    var val = currentObject.Value<string>();
                    results.Add(val);
                    break;
                }
            }

            return results;
        }
    }
}
