using Nancy;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Pf.Tester
{
    public class NancyDemo : NancyModule
    {
        private StringBuilder logs = new StringBuilder();

        private void AddToLog(string s)
        {
            logs.AppendLine(s);
        }

        public string GetLog() => logs.Replace("\r\n", "<br />").ToString();

        public NancyDemo() : base("/nancy")
        {
            Before.AddItemToEndOfPipeline(async (ctx, ct) =>
          {
              this.AddToLog("Before Hook Delay\n");
              await Task.Delay(5000);

              return null;
          });

            After.AddItemToEndOfPipeline(async (ctx, ct) =>
          {
              this.AddToLog("After Hook Delay\n");
              await Task.Delay(5000);
              this.AddToLog("After Hook Complete\n");

              ctx.Response = this.GetLog();
          });

            Get["/", runAsync: true] = async (x, ct) =>
            {
                this.AddToLog("Delay 1\n");
                await Task.Delay(1000);

                this.AddToLog("Delay 2\n");
                await Task.Delay(1000);

                this.AddToLog("Executing async http client\n");
                using (var client = new HttpClient())
                {
                    var res = await client.GetAsync("http://nancyfx.org");
                    var content = await res.Content.ReadAsStringAsync();
                    this.AddToLog("Response: " + content.Split('\n')[0] + "\n");
                }
                return (Response)this.GetLog();
            };
            Get["/nancy/demo"] = parameters => new string[] { "Hello", "World" };
        }


    }
}
