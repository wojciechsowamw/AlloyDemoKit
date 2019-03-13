using System;
using System.Collections.Generic;
using Microsoft.Graph;
using Newtonsoft.Json;

namespace AlloyDemoKit.Models.GraphApi
{
    public class CompositeGraphObject
    {
        public User User { get; set; }
        public Organization Organization { get; set; }
        public List<DriveItem> DriveItems { get; set; }
        public List<Message> Messages { get; set; }
        public List<Event> Events { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<OnenoteSection> OnenoteSections { get; set; }
        public List<OnenotePage> OnenotePages { get; set; }
        public Planner Planner { get; set; }
        public List<PlannerPlan> PlannerPlans { get; set; }
        public List<PlannerTask> PlannerTasks { get; set; }
        public List<Group> JoinedTeams { get; set; }
        public List<User> FoundUsers { get; set; }
        public List<User> TeamMembers { get; set; }
        public List<TeamChannel> TeamChannels { get; set; }
        public List<TeamChannelTab> TeamChannelTabs { get; set; }
        public List<AppRoleAssignment> AppRoleAssignments { get; set; }
    }
    public class AppRoleAssignment
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("creationTimestamp")]
        public DateTime? CreationTimestamp { get; set; }

        [JsonProperty("appRoleId")]
        public string AppRoleId { get; set; }

        [JsonProperty("principalDisplayName")]
        public string PrincipalDisplayName { get; set; }

        [JsonProperty("principalId")]
        public string PrincipalId { get; set; }

        [JsonProperty("principalType")]
        public string PrincipalType { get; set; }

        [JsonProperty("resourceDisplayName")]
        public string ResourceDisplayName { get; set; }

        [JsonProperty("resourceId")]
        public string ResourceId { get; set; }
    }
    public class Configuration
    {
        [JsonProperty("entityId")]
        public object EntityId { get; set; }

        [JsonProperty("contentUrl")]
        public object ContentUrl { get; set; }

        [JsonProperty("removeUrl")]
        public object RemoveUrl { get; set; }

        [JsonProperty("websiteUrl")]
        public object WebsiteUrl { get; set; }

        [JsonProperty("wikiTabId")]
        public int WikiTabId { get; set; }

        [JsonProperty("wikiDefaultTab")]
        public bool WikiDefaultTab { get; set; }

        [JsonProperty("hasContent")]
        public bool HasContent { get; set; }
    }

    public class TeamChannelTab
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("teamsAppId")]
        public string TeamsAppId { get; set; }

        [JsonProperty("sortOrderIndex")]
        public string SortOrderIndex { get; set; }

        [JsonProperty("messageId")]
        public object MessageId { get; set; }

        [JsonProperty("webUrl")]
        public string WebUrl { get; set; }

        [JsonProperty("configuration")]
        public Configuration Configuration { get; set; }
    }
}