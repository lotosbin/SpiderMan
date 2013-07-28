using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using sharp_net.Mongo;

namespace SpiderMan.Models {
    public class Avator : MEntity {
        public string Link { get; set; }
        public int Usability { get; set; }
        public int UsedTimes { get; set; }
    }
}