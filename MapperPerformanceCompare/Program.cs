using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using PerformanceTool.Osgi;

namespace PerformanceTool
{
    internal class Program
    {
        private List<KeyValuePair<ITestMetadata, ITestRunner>> TestItems;

        //初始化映射器，并做一次映射操作
        private void Init()
        {
            foreach (var item in TestItems)
            {
                item.Value.Initialize();
                item.Value.Run();
            }
        }

        //进行测试
        private void Run(List<Record> records)
        {
            CodeTimer.Initialize();

            foreach (var item in TestItems)
                CodeTimer.Time(item.Key, () => item.Value.Run(), records,
                    Enumerable.Range(1, 50).Select(x => 10000 * x).ToArray());
        }

        private static void Main(string[] args)
        {
            using (var e = OsgiEngine.InitWinformEngine("testfolder"))
            {
                var host = new Program();
                host.TestItems = LazyLoading.NewAll<ITestMetadata, ITestRunner>();

                host.Init();
                var records = new List<Record>();
                host.Run(records);

                var data =
                    records.Select(
                        x =>
                            new
                            {
                                name = x.Mapper.Name,
                                Iterations = x.Iterations.Select(v => v.Iteration).ToArray(),
                                TimeElapseds = x.Iterations.Select(v => v.TimeElapsed).ToArray(),
                                CPUCycles = x.Iterations.Select(v => v.CPUCycles).ToArray()
                            }).ToArray();


                var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                var s = JsonConvert.SerializeObject(data, Formatting.None);
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
            }

            //Console.Read();
        }
    }
}