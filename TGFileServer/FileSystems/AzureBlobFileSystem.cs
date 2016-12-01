namespace TGFileServer.FileSystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Owin.FileSystems;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    public class AzureBlobFileSystem : IFileSystem
    {
        private readonly CloudBlobContainer _container;

        public AzureBlobFileSystem(CloudStorageAccount cloudStorageAccount, string containerName)
        {
            var client = cloudStorageAccount.CreateCloudBlobClient();
            _container = CreateContainer(client, containerName, BlobContainerPublicAccessType.Blob);
        }

        public bool TryGetFileInfo(string subpath, out IFileInfo fileInfo)
        {
            if (subpath.StartsWith("/", StringComparison.Ordinal))
            {
                subpath = subpath.Substring(1);
            }

            var blob = _container.GetBlobReference(subpath);
            if (!blob.Exists())
            {
                fileInfo = null;
                return false;
            }

            blob.FetchAttributes();
            fileInfo = new BlobFileInfo(blob);
            return true;
        }

        public bool TryGetDirectoryContents(string subpath, out IEnumerable<IFileInfo> contents)
        {
            if (subpath.StartsWith("/", StringComparison.Ordinal))
            {
                subpath = subpath.Substring(1);
            }

            var directory = _container.GetDirectoryReference(subpath);
            contents = directory.ListBlobs().Select(blob => new BlobFileInfo(blob));
            return contents.Any() || string.IsNullOrEmpty(subpath);
        }

        /// <summary>
        /// Returns the media container, creating a new one if none exists.
        /// </summary>
        /// <param name="cloudBlobClient"><see cref="CloudBlobClient"/> where the container is stored.</param>
        /// <param name="containerName">The name of the container.</param>
        /// <param name="accessType"><see cref="BlobContainerPublicAccessType"/> indicating the access permissions.</param>
        /// <returns>The <see cref="CloudBlobContainer"/></returns>
        private static CloudBlobContainer CreateContainer(CloudBlobClient cloudBlobClient, string containerName, BlobContainerPublicAccessType accessType)
        {
            var container = cloudBlobClient.GetContainerReference(containerName);
            container.CreateIfNotExists();
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = accessType });
            return container;
        }
    }
}