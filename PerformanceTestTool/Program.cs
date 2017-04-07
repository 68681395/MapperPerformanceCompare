using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using PerformanceTool.Osgi;
using PerformanceTool.SharpHttpServer;
using System.Net;
using System;
using System.Threading.Tasks.Schedulers;
using System.Threading.Tasks;
using System.Threading;

namespace PerformanceTool
{
    public class Server : HttpServer
    {
        public Server(int port)
            : base(port)
        {
            Get["/"] = _ => "Hello world!";

            Get["/api/v1/users"] = GetUsers;
            Get["dataJs.js"] = req =>
            {

                //  HttpListenerContext.
                return "";
            };
            ServeStatic(new DirectoryInfo

                ("/"), "/");
        }

        private string GetUsers(HttpListenerRequest arg)
        {
            return JsonConvert.SerializeObject(new[]
            {
                new { Id = 1, Username = "peter" },
                new { Id = 1, Username = "jack" },
                new { Id = 1, Username = "john" },
            });
        }
    }
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
                // var chartfile = Path.Combine(dir, "performance.html");
                //Process.Start(new ProcessStartInfo(chartfile));
                var server = new Server(8080);
                server.Run();
                Process.Start("explorer.exe", "http://localhost:8080/api/v1/users");
                Console.ReadLine();
            }


        }
    }
}