using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using AlloyDemoKit.Models.GraphApi;
using Microsoft.Graph;
using Newtonsoft.Json;

namespace AlloyDemoKit.Helpers

{
    public class AzureADUser
    {
        public string DisplayName { get; set; }

        public string Email { get; set; }
    }
    public class GraphApiReturnedObjectModel
    {
        [JsonProperty("@odata.context")]
        public string Context;

        [JsonProperty("@odata.count")]
        public int Count;

        [JsonProperty("@odata.nextLink")]
        public string NextLink;

        [JsonProperty("value")]
        public object Value;
    }

    //public static class GraphUser
    //{
    //    public static ClaimsPrincipal User => HttpContext.Current.User as ClaimsPrincipal;
    //}
    public class GraphService
    {
        private readonly MemoryCache memoryCache;
        private readonly GraphServiceClient graphClientForUser;
        private readonly GraphServiceClient graphClientForApplication;
        private readonly IGraphAuthProvider authProvider;
        //private readonly IHttpContextAccessor httpContextAccessor;

        public GraphService(MemoryCache memoryCache, IGraphSdkHelper graphSdkHelper, IGraphAuthProvider authProvider)
        {
            this.memoryCache = memoryCache;
            this.graphClientForUser = graphSdkHelper.GetAuthenticatedClientForUser();
            //this.graphClientForApplication = graphSdkHelper.GetAuthenticatedClientForApplication();
            this.authProvider = authProvider;
            //this.httpContextAccessor = httpContextAccessor;
        }

        // Load user's profile in formatted JSON.
        public async Task<CompositeGraphObject> GetUserJson(string email, string search = null)
        {
            if (email == null) return null;

            CompositeGraphObject compositeGraphObject = this.memoryCache.Get("compositeGraphObject") as CompositeGraphObject;
            if (compositeGraphObject != null)
            {
                return compositeGraphObject;
            }

            var user = HttpContext.Current.User as ClaimsPrincipal;
            string userId = user.FindFirst(Startup.ObjectIdentifierType)?.Value;
            var accessToken = await authProvider.GetUserAccessTokenAsync(userId);

            HttpClient httpClient = new HttpClient(new HttpClientHandler());
            httpClient.BaseAddress = new Uri(@"https://graph.microsoft.com/beta/");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            try
            {
                User me = await GeObjectFromGraph<User>(httpClient, "me");
                Organization organization = await GeObjectFromGraph<Organization>(httpClient, "organization");

                //List<DriveItem> driveItems = await GetListOfObjectsFromGraph<DriveItem>(httpClient, "me/drive/root/children");
                List<DriveItem> driveSharedItems =
                    await GetListOfObjectsFromGraph<DriveItem>(httpClient, "me/drive/sharedWithMe");

                //SHAREPOINT SHARED DOCUMENTS
                //List<List> siteLists = await GetListOfObjectsFromGraph<List>(httpClient, "sites/root/lists");
                //List sharedDocuments = siteLists.FirstOrDefault(sr => sr.Name.Equals("Shared Documents"));
                //List<DriveItem> sharedDocumentItems = await GetListOfObjectsFromGraph<DriveItem>(httpClient, "sites/root/lists/" + sharedDocuments.Id + "/items");

                List<Message> messages = await GetListOfObjectsFromGraph<Message>(httpClient, "me/messages");
                List<Event> events = await GetListOfObjectsFromGraph<Event>(httpClient, "me/events");
                List<Contact> contacts = await GetListOfObjectsFromGraph<Contact>(httpClient, "me/contacts");
                List<OnenoteSection> oneNoteSections =
                    await GetListOfObjectsFromGraph<OnenoteSection>(httpClient, "me/onenote/sections");
                //List<OnenotePage> onenotePages = await GetListOfObjectsFromGraph<OnenotePage>(httpClient, "me/onenote/pages");

                Planner planner = await GeObjectFromGraph<Planner>(httpClient, "me/planner/");

                List<PlannerPlan> plannerPlan =
                    await GetListOfObjectsFromGraph<PlannerPlan>(httpClient, "me/planner/plans");
                List<PlannerTask> plannerTasks =
                    await GetListOfObjectsFromGraph<PlannerTask>(httpClient, "me/planner/tasks");
                List<Group> joinedTeams = await GetListOfObjectsFromGraph<Group>(httpClient, "me/joinedTeams");

                string searchFor = search ?? "Wojciech";
                List<User> foundUsers =
                    await GetListOfObjectsFromGraph<User>(httpClient, "me/people?$search=" + searchFor);

                List<User> teamMembers =
                    await GetListOfObjectsFromGraph<User>(httpClient, "groups/" + joinedTeams[0].Id + "/members");
                List<TeamChannel> teamChannels =
                    await GetListOfObjectsFromGraph<TeamChannel>(httpClient,
                        "teams/" + joinedTeams[0].Id + "/channels");
                List<TeamChannelTab> channelTabs = await GetListOfObjectsFromGraph<TeamChannelTab>(httpClient,
                    "teams/" + joinedTeams[0].Id + "/channels/" + teamChannels[0].Id + "/tabs");
                //List<object> applications = await GetListOfObjectsFromGraph<object>(httpClient, "applications");
                List<AppRoleAssignment> appRoleAssignments =
                    await GetListOfObjectsFromGraph<AppRoleAssignment>(httpClient, "me/appRoleAssignments");

                compositeGraphObject = new CompositeGraphObject()
                {
                    Organization = organization,
                    Planner = planner,
                    TeamChannelTabs = channelTabs,
                    User = me,
                    Events = events,
                    OnenoteSections = oneNoteSections,
                    Messages = messages,
                    PlannerPlans = plannerPlan,
                    //OnenotePages = onenotePages,
                    Contacts = contacts,
                    AppRoleAssignments = appRoleAssignments,
                    PlannerTasks = plannerTasks,
                    TeamChannels = teamChannels,
                    DriveItems = driveSharedItems,
                    FoundUsers = foundUsers,
                    JoinedTeams = joinedTeams,
                    TeamMembers = teamMembers
                };

                this.memoryCache.Add(new CacheItem("compositeGraphObject", compositeGraphObject),
                    new CacheItemPolicy() { });

                return compositeGraphObject;
            }
            catch (ServiceException e)
            {
                switch (e.Error.Code)
                {
                    case "RequestResourceNotFound":
                    case "ResourceNotFound":
                    case "ErrorItemNotFound":
                    case "itemNotFound":
                        return null;
                    case "ErrorInvalidUser":
                        return null;
                    case "AuthenticationFailure":
                        return null;
                    case "TokenNotFound":
                        //await httpContext.ChallengeAsync();
                        return null;
                    default:
                        return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private async Task<T> GeObjectFromGraph<T>(HttpClient httpClient, string url)
        {
            var response = await httpClient.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();
            T t = JsonConvert.DeserializeObject<T>(json);
            return t;
        }

        private async Task<List<T>> GetListOfObjectsFromGraph<T>(HttpClient httpClient, string url)
        {
            var response = await httpClient.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();
            GraphApiReturnedObjectModel graphObject = JsonConvert.DeserializeObject<GraphApiReturnedObjectModel>(json);
            List<T> objects = JsonConvert.DeserializeObject<List<T>>(graphObject.Value.ToString());
            return objects;
        }
        

        // Load user's profile picture in base64 string.
        public async Task<string> GetPictureBase64(string email)
        {
            try
            {
                var key = "picture_" + email;
                if(!this.memoryCache.Contains(key))
                {
                    var pictureStream = await GetPictureStream(email);
                    if (pictureStream == null)
                    {
                        return null;
                    }

                    // Copy stream to MemoryStream object so that it can be converted to byte array.
                    var pictureMemoryStream = new MemoryStream();
                    await pictureStream.CopyToAsync(pictureMemoryStream);

                    // Convert stream to byte array.
                    var pictureByteArray = pictureMemoryStream.ToArray();

                    // Convert byte array to base64 string.
                    var pictureBase64 = Convert.ToBase64String(pictureByteArray);
                    var picture = "data:image/jpeg;base64," + pictureBase64;
                    return this.memoryCache.AddOrGetExisting(key, picture, new CacheItemPolicy(),null).ToString();
                }
                return "data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz4NCjwhRE9DVFlQRSBzdmcgIFBVQkxJQyAnLS8vVzNDLy9EVEQgU1ZHIDEuMS8vRU4nICAnaHR0cDovL3d3dy53My5vcmcvR3JhcGhpY3MvU1ZHLzEuMS9EVEQvc3ZnMTEuZHRkJz4NCjxzdmcgd2lkdGg9IjQwMXB4IiBoZWlnaHQ9IjQwMXB4IiBlbmFibGUtYmFja2dyb3VuZD0ibmV3IDMxMi44MDkgMCA0MDEgNDAxIiB2ZXJzaW9uPSIxLjEiIHZpZXdCb3g9IjMxMi44MDkgMCA0MDEgNDAxIiB4bWw6c3BhY2U9InByZXNlcnZlIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPg0KPGcgdHJhbnNmb3JtPSJtYXRyaXgoMS4yMjMgMCAwIDEuMjIzIC00NjcuNSAtODQzLjQ0KSI+DQoJPHJlY3QgeD0iNjAxLjQ1IiB5PSI2NTMuMDciIHdpZHRoPSI0MDEiIGhlaWdodD0iNDAxIiBmaWxsPSIjRTRFNkU3Ii8+DQoJPHBhdGggZD0ibTgwMi4zOCA5MDguMDhjLTg0LjUxNSAwLTE1My41MiA0OC4xODUtMTU3LjM4IDEwOC42MmgzMTQuNzljLTMuODctNjAuNDQtNzIuOS0xMDguNjItMTU3LjQxLTEwOC42MnoiIGZpbGw9IiNBRUI0QjciLz4NCgk8cGF0aCBkPSJtODgxLjM3IDgxOC44NmMwIDQ2Ljc0Ni0zNS4xMDYgODQuNjQxLTc4LjQxIDg0LjY0MXMtNzguNDEtMzcuODk1LTc4LjQxLTg0LjY0MSAzNS4xMDYtODQuNjQxIDc4LjQxLTg0LjY0MWM0My4zMSAwIDc4LjQxIDM3LjkgNzguNDEgODQuNjR6IiBmaWxsPSIjQUVCNEI3Ii8+DQo8L2c+DQo8L3N2Zz4NCg==";
            }
            catch (Exception e)
            {
                switch (e.Message)
                {
                    case "ResourceNotFound":
                        // If picture not found, return the default image.
                        return "data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz4NCjwhRE9DVFlQRSBzdmcgIFBVQkxJQyAnLS8vVzNDLy9EVEQgU1ZHIDEuMS8vRU4nICAnaHR0cDovL3d3dy53My5vcmcvR3JhcGhpY3MvU1ZHLzEuMS9EVEQvc3ZnMTEuZHRkJz4NCjxzdmcgd2lkdGg9IjQwMXB4IiBoZWlnaHQ9IjQwMXB4IiBlbmFibGUtYmFja2dyb3VuZD0ibmV3IDMxMi44MDkgMCA0MDEgNDAxIiB2ZXJzaW9uPSIxLjEiIHZpZXdCb3g9IjMxMi44MDkgMCA0MDEgNDAxIiB4bWw6c3BhY2U9InByZXNlcnZlIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPg0KPGcgdHJhbnNmb3JtPSJtYXRyaXgoMS4yMjMgMCAwIDEuMjIzIC00NjcuNSAtODQzLjQ0KSI+DQoJPHJlY3QgeD0iNjAxLjQ1IiB5PSI2NTMuMDciIHdpZHRoPSI0MDEiIGhlaWdodD0iNDAxIiBmaWxsPSIjRTRFNkU3Ii8+DQoJPHBhdGggZD0ibTgwMi4zOCA5MDguMDhjLTg0LjUxNSAwLTE1My41MiA0OC4xODUtMTU3LjM4IDEwOC42MmgzMTQuNzljLTMuODctNjAuNDQtNzIuOS0xMDguNjItMTU3LjQxLTEwOC42MnoiIGZpbGw9IiNBRUI0QjciLz4NCgk8cGF0aCBkPSJtODgxLjM3IDgxOC44NmMwIDQ2Ljc0Ni0zNS4xMDYgODQuNjQxLTc4LjQxIDg0LjY0MXMtNzguNDEtMzcuODk1LTc4LjQxLTg0LjY0MSAzNS4xMDYtODQuNjQxIDc4LjQxLTg0LjY0MWM0My4zMSAwIDc4LjQxIDM3LjkgNzguNDEgODQuNjR6IiBmaWxsPSIjQUVCNEI3Ii8+DQo8L2c+DQo8L3N2Zz4NCg==";
                    case "EmailIsNull":
                        return JsonConvert.SerializeObject(new { Message = "Email address cannot be null." }, Formatting.Indented);
                    default:
                        return null;
                }
            }
        }

        public async Task<Stream> GetPictureStream(string email)
        {
            if (email == null) throw new Exception("EmailIsNull");

            Stream pictureStream = null;

            try
            {
                pictureStream = await this.graphClientForUser.Me.Photo.Content.Request().GetAsync();
            }
            catch (ServiceException e)
            {
                switch (e.Error.Code)
                {
                    case "RequestResourceNotFound":
                    case "ResourceNotFound":
                    case "ErrorItemNotFound":
                    case "itemNotFound":
                    case "ErrorInvalidUser":
                        // If picture not found, return the default image.
                        throw new Exception("ResourceNotFound");
                    case "TokenNotFound":
                        //await httpContext.ChallengeAsync();
                        return null;
                    default:
                        return null;
                }
            }

            return pictureStream;
        }
    }
    public class AzureAdOptions
    {
        public bool UseMSI { get; set; }

        public string ClientId { get; set; }

        public string ClientCertificate { get; set; }

        public string Instance { get; set; }

        public string Domain { get; set; }

        public string TenantId { get; set; }

        public string CallbackPath { get; set; }

        public string BaseUrl { get; set; }

        public string Scopes { get; set; }

        public string GraphResourceId { get; set; }

        public string GraphScopes { get; set; }

        public string KeyVaultEndpoint { get; set; }
    }
}