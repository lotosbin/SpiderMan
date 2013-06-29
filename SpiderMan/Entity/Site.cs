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
        public int Act { get; set; }
        [Required]
        public string Name { get; set; }
        public string Link { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "必须大于0")]
        public int GrabInterval { get; set; }
    }
}