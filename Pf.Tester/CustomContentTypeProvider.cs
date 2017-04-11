using Microsoft.Owin.StaticFiles.ContentTypes;

namespace Pf.Tester
{
    public class CustomContentTypeProvider : FileExtensionContentTypeProvider
    {
        public CustomContentTypeProvider()
        {
            Mappings.Add(".json", "application/json");
        }
    }
}
