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

            ServiceRegistry.RegisteryFromAssemblyOf<Program>();

            var host = new Program();
            ServiceRegistry.Compose(host);

            host.Init();
            var records = new List<Record>();
            host.Run(records);

            var data = records.Select(x => new
            {
                name = x.Mapper.Name,
                Iterations = x.Iterations.Select(v => v.Iteration).ToArray(),
                TimeElapseds = x.Iterations.Select(v => v.TimeElapsed).ToArray(),
                CPUCycles = x.Iterations.Select(v => v.CPUCycles).ToArray(),
            }).ToArray();


            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var s = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.None);
            var file = Path.Combine(dir, "dataJs.js");
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
