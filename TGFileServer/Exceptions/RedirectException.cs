namespace TGFileServer.Exceptions
{
    using System;

    public class RedirectException : Exception
    {
        public Uri RedirectUri { get; private set; }

        public RedirectException(Uri redirectUri)
        {
            RedirectUri = redirectUri;
        }
    }
}