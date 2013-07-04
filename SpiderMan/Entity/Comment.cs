using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SpiderMan.Models {

    public class Comment : Entity {
        [DataMember]
        public string Content { get; set; }
    }
}