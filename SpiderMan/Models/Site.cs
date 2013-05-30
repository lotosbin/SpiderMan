using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpiderMan.Models {
    public class Site : Entity {
        [Required]
        public string Name { get; set; }
        public string Link { get; set; }
    }
}