using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiderMan.Models {

    public class Shudong : Article {
        public int Hot { get; set; } //流量最高的，Up和Donw数字相加。
        public IList<string> Comments { get; set; }
    }
}