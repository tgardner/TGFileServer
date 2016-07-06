﻿using Microsoft.Owin;
using TGFileServer;

[assembly: OwinStartup(typeof(Startup))]

namespace TGFileServer
{
    using System.Configuration;
    using Configuration;
    using FileSystems;
    using Microsoft.Owin.StaticFiles;
    using Microsoft.WindowsAzure.Storage;
    using Owin;

    public class Startup
    {
        private static readonly string StorageConnectionString =
            ConfigurationManager.AppSettings["StorageConnectionString"];

        private static readonly string StorageContainer = ConfigurationManager.AppSettings["StorageContainer"];

        public void Configuration(IAppBuilder app)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(StorageConnectionString);
            var fileSystem = new AzureBlobFileSystem(cloudStorageAccount, StorageContainer);

            var options = new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                EnableDefaultFiles = false,
                FileSystem = fileSystem,
                StaticFileOptions =
                {
                    ContentTypeProvider = new CustomContentTypeProvider()
                }
            };

            app.UseFileServer(options);
        }
    }
}