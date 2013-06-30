using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SpiderMan.Models {

    public class Article : Entity {
        public int Status { get; set; } //eArticleStatus
        public string SourceSite { get; set; }
        public string SourceLink { get; set; }
        public DateTime CreatDate { get; set; }
        public string Content { get; set; }

        public DateTime GrabDate { get; set; }

        public void Init(SpiderTask task) {
            SourceSite = task.Site;
            SourceLink = task.Url;
            GrabDate = DateTime.Now;
        }
    }
}