using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiderMan.Models {

    public class Finance : Article {
        public int Hot { get; set; }
        public string Title { get; set; }
        public List<Comment> Comments { get; set; }
    }
}