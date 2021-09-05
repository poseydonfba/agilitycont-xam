using AgilityContXam.Enums;
using Newtonsoft.Json;

namespace AgilityContXam.Models
{
    public class MainMenu
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("type")]
        public MainMenuType Type { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }
    }
}
