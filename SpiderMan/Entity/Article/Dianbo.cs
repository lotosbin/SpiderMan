using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiderMan.Models {

    //还没想好，是不是要做一个完整的外国影视库
    public class Dianbo : Article {
        public string Title { get; set; }
    }
}