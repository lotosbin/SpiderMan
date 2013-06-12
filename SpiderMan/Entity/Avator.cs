using MongoRepository;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiderMan.Models {
    public class Avator : Entity {
        public string Link { get; set; }
        public int Usability { get; set; }
        public int UsedTimes { get; set; }
    }
}