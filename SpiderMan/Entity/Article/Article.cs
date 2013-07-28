using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using sharp_net.Mongo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SpiderMan.Models {

    public class Article : MEntity {
        public int Status { get; set; } //eArticleStatus
        public string SourceSite { get; set; }
        public string SourceLink { get; set; }
        public DateTime BrithDate { get; set; }
        public string Content { get; set; }
        public DateTime GrabDate { get; set; }

        public string ProviderId { get; set; }

        public void Init(SpiderTask task) {
            SourceSite = task.Site;
            SourceLink = task.Url;
            GrabDate = DateTime.Now;
        }
    }
}