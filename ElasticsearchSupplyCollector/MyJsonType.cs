using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticsearchSupplyCollector
{
    public class MyJsonType
    {
        public IndexSubDocumentType Index { get; set; }
        public AliasSubDocumentType Alias { get; set; }
        public MappingsSubDocumentType Mapping { get; set; }
        public PropertyKeySubDocumentType PropertyKey { get; set; }
        public PropertyValueSubDocumentType PropertyValue { get; set; }
        public string TypeValue { get; set; }
        public FieldSubDocumentType Field { get; set; }
        public KeywordSubDocumentType Keyword { get; set; }
        public string KeywordTypeValue { get; set; }
        public double IgnoreAbove { get; set; }

    }

    public class IndexSubDocumentType
    {
        public string AliasTitle { get; set; }
    }
    public class AliasSubDocumentType
    {
        public string AliasSubDocumentProperty { get; set; }
    }
    public class MappingsSubDocumentType
    {
        public string PropertyKey{ get; set; }
    }
    public class PropertyKeySubDocumentType
    {
        public string PropertyValue { get; set; }
    }
    public class PropertyValueSubDocumentType
    {
        public string TypeKey { get; set; }
    }
    public class FieldSubDocumentType
    {
        public string Keyword { get; set; }
    }
    public class KeywordSubDocumentType
    {
        public string KeywordTypeKey { get; set; }
    }
}
