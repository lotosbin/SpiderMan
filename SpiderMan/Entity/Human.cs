using MongoDB.Bson.Serialization.Attributes;
using sharp_net.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiderMan.Models {

    public class UserName : MEntity {
        public string Name { get; set; }
        public int Status { get; set; }
    }

    public class Avator : MEntity {
        public string Link { get; set; }
        public int Status { get; set; }
    }

    public class Human: MEntity {
        public string Name { get; set; }
        public string Avator { get; set; }
        public bool GgpttUsed { get; set; }
        public bool ShudongUsed { get; set; }
        public bool AitiUsed { get; set; }
        public bool CaijinUsed { get; set; }
    }
}