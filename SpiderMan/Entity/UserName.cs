using MongoDB.Bson.Serialization.Attributes;
using sharp_net.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiderMan.Models {

    public class UserName : MEntity {
        public string Name { get; set; }
        public int Usability { get; set; }
        public int UsedTimes { get; set; }
    }
}