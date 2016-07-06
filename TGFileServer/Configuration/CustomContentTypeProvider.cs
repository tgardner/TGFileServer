namespace TGFileServer.Configuration
{
    using Microsoft.Owin.StaticFiles.ContentTypes;

    public class CustomContentTypeProvider : FileExtensionContentTypeProvider
    {
        public CustomContentTypeProvider()
        {
            Mappings.Add(".json", "application/json");
            Mappings.Add(".exe", "application/x-msdownload");
        }
    }
}