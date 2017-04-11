using Nancy;

namespace Pf.Tester
{
    public class NancyDemo : NancyModule
    {
        public NancyDemo()
        {
            Get["/nancy/demo"] = parameters => new string[] { "Hello", "World" };
        }
    }
}
