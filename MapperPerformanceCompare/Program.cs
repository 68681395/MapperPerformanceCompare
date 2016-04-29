using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using NLite;
using System.IO;
using System.Reflection;

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

    class Program
    {
        [InjectMany]
        private Lazy<IObjectToObjectMapper, IMapperMetadata>[] Mappers;

        //初始化映射器，并做一次映射操作
        void Init()
        {
            foreach (var item in Mappers)
            {
                item.Value.Initialize();
                item.Value.Map();
            }

        }

        //进行测试
        void Run(List<Record> records)
        {
            CodeTimer.Initialize();

            foreach (var item in Mappers)
                CodeTimer.Time(item.Metadata, () => item.Value.Map(),
                    records, Enumerable.Range(1, 10).Select(x => 10000 * x).ToArray());
        }

        static void Main(string[] args)
        {




            //CsvHelper.CsvWriter wr = new CsvHelper.CsvWriter(twr);

            //wr.WriteRecord()

            ServiceRegistry.RegisteryFromAssemblyOf<Program>();

            var host = new Program();
            ServiceRegistry.Compose(host);

            host.Init();
            var records = new List<Record>();
            host.Run(records);

            var data = records.Select(x => new
            {
                name = x.Mapper.Name,
                data =
                    x.Iterations.Select(v =>
                    new long[] { v.Iteration, v.TimeElapsed }
                    ).ToArray()
            }).ToArray();


            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var s = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.None);
            var file =Path.Combine(dir  , "dataJs.js");
            using (var writer = new StreamWriter(file))
            {
                writer.Write("dataAll=");
                writer.Write(s);
                writer.Write(";");
                writer.Flush();
            }
            var chartfile = Path.Combine(dir, "performance.html");
            Process.Start(new ProcessStartInfo(chartfile));
            //Console.Read();
        }
    }


}
