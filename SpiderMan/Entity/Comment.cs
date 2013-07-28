using MongoDB.Bson.Serialization.Attributes;
using sharp_net.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SpiderMan.Models {

    public class Comment {
        public string Content { get; set; }
    }
}