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

    [DataContract]
    public class Article : Entity {
        [DataMember]
        public int Status { get; set; } //eArticleStatus
        [DataMember]
        public string SourceSite { get; set; }
        [DataMember]
        public string SourceLink { get; set; }
        [DataMember]
        public DateTime CreatDate { get; set; }
        [DataMember]
        public string Content { get; set; }
        [DataMember]
        public DateTime GrabDate { get; set; }

        public void Init(SpiderTask task) {
            SourceSite = task.Site;
            SourceLink = task.Url;
            GrabDate = DateTime.Now;
        }
    }
}