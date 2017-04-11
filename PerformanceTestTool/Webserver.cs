using System;
using Microsoft.Owin.Hosting;
using System.Diagnostics;

namespace Pf.Tester
{
    public class WebServer
    {
        private string name;
        private string _url;
        private IDisposable _webapp;

        public WebServer(string name, string url)
        {
            this.name = name;
            _url = url;
        }

        public void Start()
        {
            _webapp = WebApp.Start<Startup>(_url);
        }

        public void Stop()
        {
            _webapp?.Dispose();
        }
    }
}
