using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace speakers.Models
{
    public class Speaker
    {
        [Required]
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("email")]
        public string email { get; set; }

        [JsonProperty("firstName")]
        public string firstName { get; set; }

        [JsonProperty("lastName")]
        public string lastName { get; set; }

        [JsonProperty("listOrder")]
        public int listOrder { get; set; }

        [JsonProperty("loggedIn")]
        public bool loggedIn { get; set; }

        [JsonProperty("timeLeft")]
        public int timeLeft { get; set; }
    }
}