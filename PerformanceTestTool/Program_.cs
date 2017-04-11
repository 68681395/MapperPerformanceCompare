using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Newtonsoft.Json;
using PerformanceTool;
using PerformanceTool.Osgi;

namespace Pf.Tester
{
   
    internal class Program_
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

        private static void _Main(string[] args)
        {
            var qts = new QueuedTaskScheduler(TaskScheduler.Default, maxConcurrencyLevel: 4);
            var options = new ParallelOptions { TaskScheduler = qts };

            Task.Factory.StartNew(() =>
            {
                Parallel.For(0, 100, options, i =>
                {
                    //…
                });
            }, CancellationToken.None, TaskCreationOptions.None, qts);

            Task.Factory.StartNew(() =>
            {
                Parallel.For(0, 100, options, i =>
                {
                    //…
                });
            }, CancellationToken.None, TaskCreationOptions.None, qts);


            using (var e = OsgiEngine.InitWinformEngine("testfolder"))
            {
                var host = new Program_();
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
                // var chartfile = Path.Combine(dir, "performance.html");
                //Process.Start(new ProcessStartInfo(chartfile));
              
                Process.Start("explorer.exe", "http://localhost:8080/api/v1/users");
                Console.ReadLine();
            }


        }
    }
}