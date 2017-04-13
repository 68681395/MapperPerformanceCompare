﻿using System.Diagnostics;
using Topshelf;
using Topshelf.Hosts;
using Topshelf.Logging;

namespace Pf.Tester
{
    class Program
    {
        static int Main(string[] args)
        {
            return (int)StartTopshelf();
        }

        static TopshelfExitCode StartTopshelf()
        {
            var url = "http://localhost:8080";
            return HostFactory.Run(x =>
             {
                 x.Service<WebServer>(s =>
                 {
                     s.ConstructUsing(name => new WebServer(name, url));
                     s.WhenStarted(
                         (tc, hsc) =>
                         {
                             tc.Start();
                             HostLogger.Current.Get("WebServer").Info($"WebServer start on {url}");
                             var host = hsc as ConsoleRunHost;
                             if (host != null)
                             {
                                 Process.Start("explorer.exe", url);
                             }
                             return true;
                         });
                     s.WhenStopped(tc => tc.Stop());
                 });
                 x.RunAsLocalSystem();
                 x.SetDescription("This is a tool for performance test.");
                 x.SetDisplayName("Tsharp Performance Tester");
                 x.SetServiceName("TsharpPerformanceTester");
             });
        }
    }
}
