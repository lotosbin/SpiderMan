using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiderMan.Models {
    public class SpiderTask {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("status")]
        public eTaskStatus Status { get; set; }
        [JsonProperty("articletype")]
        public eArticleType ArticleType { get; set; }
        [JsonProperty("site")]
        public string Site { get; set; }
        [JsonProperty("command")]
        public string Command { get; set; }
        [JsonProperty("commandtype")]
        public eCommandType CommandType { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("spend")]
        public int Spend { get; set; }
        [JsonProperty("grabdate")]
        public DateTime GrabDate { get; set; }
        [JsonProperty("encoding")]
        public string Encoding { get; set; }
    }
}