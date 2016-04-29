using NLite;
using System;
using System.Collections.Generic;

namespace NLiteEmitCompare
{
    [Contract]
    public interface IObjectToObjectMapper
    {
        //初始化映射器
        void Initialize();
        //执行映射
        void Map();
    }

    //测试映射器元数据
    public interface IMapperMetadata
    {
        //目录
        string Category { get; }
        //名称
        string Name { get; }
        string Descrption { get; }
    }

    //映射器元数据注解
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [MetadataAttributeAttribute]
    public class MapperAttribute : ComponentAttribute
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string Descrption { get; set; }
    }


    public class Record
    {
        public IMapperMetadata Mapper { get; set; }

        public List<IterationRecord> Iterations { get; set; }
    }

    public class IterationRecord
    {
        public int Iteration { get; set; }

        public long TimeElapsed { get; set; }
        public long CPUCycles { get; set; }
    }

}