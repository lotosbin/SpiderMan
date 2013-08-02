using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiderMan.Models {
    public class SpiderTask {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("birthtime")]
        public DateTime BirthTime { get; set; }
        [JsonProperty("taskmodelid")]
        public string TaskModelId { get; set; }
        [JsonProperty("status")]
        public eTaskStatus Status { get; set; }
        [JsonProperty("handleragent")]
        public string HandlerAgent { get; set; }
        [JsonProperty("handlertime")]
        public DateTime HandlerTime { get; set; }
        [JsonProperty("articletype")]
        public string ArticleType { get; set; }
        [JsonProperty("site")]
        public string Site { get; set; }
        [JsonProperty("commandtype")]
        public string CommandType { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("spend")]
        public int Spend { get; set; }
    }
}