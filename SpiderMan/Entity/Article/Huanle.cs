using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiderMan.Models {

    public class Huanle : Article {

        public string Title { get; set; }

        public int ThumbUps { get; set; }
        public int ThumbDowns { get; set; }
        public int Amount { get; set; }

        public List<Comment> Comments { get; set; }
    }
}