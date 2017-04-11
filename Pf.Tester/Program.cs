using System.Diagnostics;
using Topshelf;
using Topshelf.Hosts;

namespace Pf.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            StartTopshelf();
        }

        static void StartTopshelf()
        {
            var url = "http://localhost:8080";
            HostFactory.Run(x =>
            {
                x.Service<WebServer>(s =>
                {
                    s.ConstructUsing(name => new WebServer(name, url));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                    s.AfterStartingService(hsc =>
                    {
                        if (hsc is ConsoleRunHost)
                        {
                            Process.Start("explorer.exe", url);
                        }
                    });
                });
                x.RunAsLocalSystem();
                x.SetDescription("This is a tool for performance test.");
                x.SetDisplayName("Tsharp Performance Tester");
                x.SetServiceName("TsharpPerformanceTester");
            });
        }
    }
}
