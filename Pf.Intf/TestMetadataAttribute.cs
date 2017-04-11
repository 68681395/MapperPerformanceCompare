using System;

namespace PerformanceTool
{
   

    //ӳ����Ԫ����ע��
    [AttributeUsage(AttributeTargets.Class)]
    public class TestMetadataAttribute : Attribute, ITestMetadata
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string Descrption { get; set; }
    }
}