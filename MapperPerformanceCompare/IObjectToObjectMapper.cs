using NLite;
using System;
using System.Collections.Generic;

namespace NLiteEmitCompare
{
    [Contract]
    public interface IObjectToObjectMapper
    {
        //��ʼ��ӳ����
        void Initialize();
        //ִ��ӳ��
        void Map();
    }

    //����ӳ����Ԫ����
    public interface IMapperMetadata
    {
        //Ŀ¼
        string Category { get; }
        //����
        string Name { get; }
        string Descrption { get; }
    }

    //ӳ����Ԫ����ע��
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