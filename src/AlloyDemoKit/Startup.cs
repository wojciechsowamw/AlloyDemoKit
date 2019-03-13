using EPiServer.Cms.UI.AspNetIdentity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Configuration;
using System.Globalization;
using System.Runtime.Caching;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using AlloyDemoKit.Helpers;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

[assembly: OwinStartup(typeof(AlloyDemoKit.Startup))]

namespace AlloyDemoKit
{
    public class Startup
    {
        public static string ACCESS_TOKEN = null;
        public const string ObjectIdentifierType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        private static readonly string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        public static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static readonly string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
        private static readonly string authority = ConfigurationManager.AppSettings["ida:Authority"];
        private static string aadAuthority = String.Format(CultureInfo.InvariantCulture, aadInstance, authority);

        const string LogoutPath = "/logout";
        public const string ClientSecret = "UQMY7@AEg.pI@QbbJED4zCxkthQ^G1UG";

        public void ConfigureServices(IServiceCollection services)
        {

        }

        public void Configuration(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = clientId,
                Authority = aadAuthority,
                PostLogoutRedirectUri = postLogoutRedirectUri,
                ResponseType = OpenIdConnectResponseType.CodeIdToken,
         
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    RoleClaimType = ClaimTypes.Role
                },
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    // If there is a code in the OpenID Connect response, redeem it for an access token and refresh token, and store those away.
                    AuthorizationCodeReceived = async (context) =>
                    {
                        var code = context.Code;
                        string signedInUserID = context.AuthenticationTicket.Identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                        string[] scopes = "User.Read User.ReadBasic.All".Split(new char[] { ' ' });

                        TokenCache userTokenCache = new SessionTokenCache(signedInUserID, MemoryCache.Default);
                        var credential = new ClientCredential(clientId, ClientSecret);

                        AuthenticationContext authContext = new AuthenticationContext(aadAuthority);
                        AuthenticationResult result = authContext.AcquireTokenByAuthorizationCodeAsync(code, new Uri(postLogoutRedirectUri), credential, "https://graph.microsoft.com/").Result;
                        ACCESS_TOKEN = result.AccessToken;

                    },
                    AuthenticationFailed = context =>
                    {
                        context.HandleResponse();
                        context.Response.Write(context.Exception.Message);
                        return Task.FromResult(0);
                    }                   
                }
            });
            
            
            
            app.UseStageMarker(PipelineStage.Authenticate);
            app.Map(LogoutPath, map =>
            {
                map.Run(ctx =>
                {
                    ctx.Authentication.SignOut();
                    return Task.FromResult(0);
                });
            });






            // Add CMS integration for ASP.NET Identity
            app.AddCmsAspNetIdentity<ApplicationUser>();

            // Remove to block registration of administrators
            app.UseSetupAdminAndUsersPage(() => HttpContext.Current.Request.IsLocal);

            //app.UseCookieAuthentication(new CookieAuthenticationOptions
            //{
            //    AuthenticationType = "Application",
            //    LoginPath = new PathString("/Login"),
            //    LogoutPath = new PathString("/Logout")
            //});
            //app.SetDefaultSignInAsAuthenticationType(WsFederationAuthenticationDefaults.AuthenticationType);
            //app.UseCookieAuthentication(new CookieAuthenticationOptions
            //{
            //    AuthenticationType = WsFederationAuthenticationDefaults.AuthenticationType
            //});



            /*
            // Use cookie authentication
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(Global.LoginPath),
                Provider = new CookieAuthenticationProvider
                {
                    // If the "/util/login.aspx" has been used for login otherwise you don't need it you can remove OnApplyRedirect.
                    OnApplyRedirect = cookieApplyRedirectContext =>
                    {
                        app.CmsOnCookieApplyRedirect(cookieApplyRedirectContext, cookieApplyRedirectContext.OwinContext.Get<ApplicationSignInManager<ApplicationUser>>());
                    },

                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager<ApplicationUser>, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => manager.GenerateUserIdentityAsync(user))
                }
            });*/


           
        }

        private void HandleMultiSiteReturnUrl(RedirectToIdentityProviderNotification<Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            if (context.ProtocolMessage.RedirectUri == null)
            {
                var currentUrl = HttpContext.Current.Request.Url;
                context.ProtocolMessage.RedirectUri = new UriBuilder(
                    currentUrl.Scheme,
                    currentUrl.Host,
                    currentUrl.Port).ToString();
            }
        }

        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            context.Response.Redirect("/?errormessage=" + context.Exception.Message);
            return Task.FromResult(0);
        }

        //private void HandleMultiSiteReturnUrl(
        //    RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        //{
        //    // here you change the context.ProtocolMessage.RedirectUri to corresponding siteurl
        //    // this is a sample of how to change redirecturi in the multi-tenant environment
        //    if (context.ProtocolMessage.RedirectUri == null)
        //    {
        //        var currentUrl = HttpContext.Current.Request.Url;
        //        context.ProtocolMessage.RedirectUri = new UriBuilder(
        //            currentUrl.Scheme,
        //            currentUrl.Host,
        //            currentUrl.Port).ToString();
        //    }
        //}
    }
}
