using System.Net.Http.Headers;
using System.Security.Claims;
using System.Web;
using Microsoft.Graph;

namespace AlloyDemoKit.Helpers
{
    public interface IGraphSdkHelper
    {
        GraphServiceClient GetAuthenticatedClientForUser();
        //GraphServiceClient GetAuthenticatedClientForApplication();
    }
    public class GraphSdkHelper : IGraphSdkHelper
    {
        private readonly IGraphAuthProvider authProvider;
        private GraphServiceClient graphClient;

        public GraphSdkHelper(IGraphAuthProvider authProvider)
        {
            this.authProvider = authProvider;
        }

        // Get an authenticated Microsoft Graph Service client.
        public GraphServiceClient GetAuthenticatedClientForUser()
        {
            ClaimsPrincipal principal = HttpContext.Current.User as ClaimsPrincipal;
            string userId = principal.FindFirst(Startup.ObjectIdentifierType)?.Value;

            graphClient = new GraphServiceClient("https://graph.microsoft.com/beta", new DelegateAuthenticationProvider(
                async requestMessage =>
                {
                    // Passing tenant ID to the sample auth provider to use as a cache key
                    var accessToken = await authProvider.GetUserAccessTokenAsync(userId);

                    // Append the access token to the request
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }));

            return graphClient;
        }
    }
}