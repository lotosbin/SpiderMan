﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiderMan.Models {

    public class Shudong : Article {
        public int Hot { get; set; }
        public List<Comment> Comments { get; set; }
    }
}