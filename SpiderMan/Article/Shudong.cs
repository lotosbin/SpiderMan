using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiderMan.Models {

    public class Shudong : Entity {
        public string SourceSite { get; set; }
        public string SourceLink { get; set; }

        public string Content { get; set; }
        public DateTime PostDate { get; set; }

        public List<Comment> Comments { get; set; }
    }
}