using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiderMan.Models {

    public class Shudong : Article {

        public List<Comment> Comments { get; set; }
    }
}