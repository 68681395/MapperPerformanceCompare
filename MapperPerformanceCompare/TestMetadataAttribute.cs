using System;

namespace PerformanceTool
{
   

    //映射器元数据注解
    [AttributeUsage(AttributeTargets.Class)]
    public class TestMetadataAttribute : Attribute, ITestMetadata
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string Descrption { get; set; }
    }
}