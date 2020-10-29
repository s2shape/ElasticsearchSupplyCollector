using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticsearchSupplyCollector
{
    public class MyJsonType
    {
        public string MyStringProperty { get; set; }

        public int MyIntegerProperty { get; set; }

        public MyJsonSubDocumentType MySubDocument { get; set; }

        public List<int> MyListProperty { get; set; }
    }

    public class MyJsonSubDocumentType
    {
        public string SubDocumentProperty { get; set; }
    }
}
