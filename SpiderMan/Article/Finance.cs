using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiderMan.Models {

    public class Finance : Article {

        public string Title { get; set; }
        public string Content { get; set; }

        public List<Comment> Comments { get; set; }
    }
}