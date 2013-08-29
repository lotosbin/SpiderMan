using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiderMan.Entity {
    public class SpiderTask {
        //SignlR 暂时不知道怎样配置驼峰明明，CamelCasePropertyNamesContractResolver无效
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("birthTime")]
        public DateTime BirthTime { get; set; }
        [JsonProperty("taskModelId")]
        public string TaskModelId { get; set; }
        [JsonProperty("status")]
        public eTaskStatus Status { get; set; }
        [JsonProperty("handlerAgent")]
        public string HandlerAgent { get; set; }
        [JsonProperty("handlerTime")]
        public DateTime HandlerTime { get; set; }
        [JsonProperty("articleType")]
        public string ArticleType { get; set; }
        [JsonIgnore]
        public string Site { get; set; }
        [JsonProperty("source")]
        public string Source { get; set; }
        [JsonProperty("commandType")]
        public string CommandType { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("spend")]
        public float Spend { get; set; }
    }
}