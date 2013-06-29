using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiderMan.Models {

    public class Article : Entity {
        public eArticleStatus ArticleStatus {
            get { return (eArticleStatus)Status; }
        }
        public int Status { get; set; }
        public string SourceSite { get; set; }
        public string SourceLink { get; set; }
        public DateTime CreatDate { get; set; }

        public string Content { get; set; }
        public int Score { get; set; }
    }
}