using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using sharp_net.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SpiderMan.Models {
    public class TaskModel : Entity {
        public int Act { get; set; }
        public string Name { get; set; }

        //[BsonRepresentation(BsonType.String)]
        public int ArticleType { get; set; }

        [Required]
        public string Site { get; set; }
        [Required]
        public string Url { get; set; }
        [Required]
        public string Command { get; set; }
        public int CommandType { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "必须大于0")]
        public int Interval { get; set; }
        public int DelayStart { get; set; }

        [BsonIgnore]
        public System.Timers.Timer Timer { get; set; }
        [BsonIgnore]
        public bool EnableTimer { get; set; }
    }
}