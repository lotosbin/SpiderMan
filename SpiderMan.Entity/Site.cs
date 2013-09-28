﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using sharp_net.Mongo;
using sharp_net.Repositories;

namespace SpiderMan.Entity {
    public class Site : MEntity {
        public int Act { get; set; }
        [Required]
        public string Name { get; set; }
        public string Link { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "必须大于0")]
        public int GrabInterval { get; set; }
    }
}