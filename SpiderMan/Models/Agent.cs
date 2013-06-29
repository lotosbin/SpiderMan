using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;

namespace SpiderMan.Models {
    public class Agent {
        [JsonProperty("connectionid")]
        public string ConnectionId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("online")]
        public bool Online { get; set; }
        [JsonIgnore]
        public IList<Timer> Timer { get; set; }
    }
}