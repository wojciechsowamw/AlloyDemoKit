using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AlloyDemoKit.Helpers
{
    public interface IGraphAuthProvider
    {
        Task<string> GetUserAccessTokenAsync(string userId);
    }
    public class GraphAuthProvider : IGraphAuthProvider
    {
        public const string ObjectIdentifierType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        private static readonly string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        public static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static readonly string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
        private static readonly string authority = ConfigurationManager.AppSettings["ida:Authority"];
        private static string aadAuthority = String.Format(CultureInfo.InvariantCulture, aadInstance, authority);

        const string LogoutPath = "/logout";
        public const string ClientSecret = "UQMY7@AEg.pI@QbbJED4zCxkthQ^G1UG";

        private readonly MemoryCache memoryCache;
        private readonly AzureAdOptions azureOptions;
        //private readonly IConfiguration configuration;

        public GraphAuthProvider(MemoryCache memoryCache)//, IConfiguration configuration)
        {
            this.memoryCache = memoryCache;
            //this.configuration = configuration;

            var azureOptions = new AzureAdOptions();
            //configuration.Bind("AzureAd", azureOptions);

            this.azureOptions = azureOptions;
        }

        public async Task<string> GetUserAccessTokenAsync(string userId)
        {
            return Startup.ACCESS_TOKEN;

            ClientCredential credential = null;

            string appSecret = Startup.ClientSecret;
            credential = new ClientCredential(clientId, appSecret);

            //var userTokenCache = new SessionTokenCache(userId, memoryCache);
            string[] scopes = "User.Read User.ReadBasic.All".Split(new char[] { ' ' });

            AuthenticationContext ac = new AuthenticationContext(aadAuthority, new ADALTokenCache(userId));
            AuthenticationResult result = null;
            try
            {
                result = await ac.AcquireTokenSilentAsync("https://graph.microsoft.com/", clientId);
                return result.AccessToken;
            }
            catch (AdalException adalException)
            {
                throw;
            }
        }
    }
}