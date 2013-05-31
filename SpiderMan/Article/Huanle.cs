using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiderMan.Models {

    public class Huanle : Entity {
        public string SourceSite { get; set; }
        public string SourceLink { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PostDate { get; set; }

        public int ThumbUps { get; set; }
        public int ThumbDowns { get; set; }
        public int Score { get; set; }
        public int Amount { get; set; }

        public List<Comment> Comments { get; set; }
    }
}