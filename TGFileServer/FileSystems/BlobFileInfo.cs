namespace TGFileServer.FileSystems
{
    using System;
    using System.IO;
    using Exceptions;
    using Microsoft.Owin.FileSystems;
    using Microsoft.WindowsAzure.Storage.Blob;

    public class BlobFileInfo : IFileInfo
    {
        private readonly IListBlobItem _blob;

        public BlobFileInfo(IListBlobItem blob)
        {
            if (blob == null)
            {
                throw new ArgumentNullException(nameof(blob));
            }

            _blob = blob;
            PhysicalPath = null;

            var directory = blob as CloudBlobDirectory;
            if (directory != null)
            {
                IsDirectory = true;

                var name = directory.Prefix.TrimEnd('/');
                if (!string.IsNullOrEmpty(directory.Parent.Prefix))
                {
                    name = name.Replace(directory.Parent.Prefix, "");
                }
                Name = name;
            }
            else
            {
                var cloudBlob = (CloudBlob) blob;

                Length = cloudBlob.Properties.Length;
                Name = cloudBlob.Name;
                if (cloudBlob.Parent != null)
                {
                    Name = Name.Remove(0, cloudBlob.Parent.Prefix.Length);
                }

                if (cloudBlob.Properties.LastModified.HasValue)
                {
                    LastModified = cloudBlob.Properties.LastModified.Value.DateTime;
                }
            }
        }

        public Stream CreateReadStream()
        {
            if (IsDirectory) return null;

            var cloudBlob = (CloudBlob)_blob;
            throw new RedirectException(cloudBlob.Uri);
        }

        public long Length { get; }
        public string PhysicalPath { get; }
        public string Name { get; }
        public DateTime LastModified { get; }
        public bool IsDirectory { get; }
    }
}