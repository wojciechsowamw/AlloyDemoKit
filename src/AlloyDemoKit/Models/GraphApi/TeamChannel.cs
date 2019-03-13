using Newtonsoft.Json;

namespace AlloyDemoKit.Models.GraphApi
{
    public class TeamChannel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("isFavoriteByDefault")]
        public object IsFavoriteByDefault { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("webUrl")]
        public string WebUrl { get; set; }
    }
}