namespace TGFileServer.Middleware
{
    using System.Threading.Tasks;
    using Exceptions;
    using Microsoft.Owin;

    public class RedirectMiddleware : OwinMiddleware
    {
        public RedirectMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            try
            {
                await Next.Invoke(context);
            }
            catch (RedirectException e)
            {
                context.Response.Redirect(e.RedirectUri.AbsoluteUri);
            }
        }
    }
}