﻿using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiderMan.Models {

    public class User : Entity {
        public string Name { get; set; }

        public int Usability { get; set; }

        public int UsedTimes { get; set; }

        public string AvatorLink { get; set; }
    }
}