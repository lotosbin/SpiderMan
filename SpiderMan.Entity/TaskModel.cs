using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using sharp_net.Mvc;
using System.ComponentModel.DataAnnotations;
using sharp_net.Mongo;
using MongoDB.Driver.Linq;
using sharp_net.Repositories;

namespace SpiderMan.Entity {
    public class TaskModel : MEntity {
        public int Act { get; set; }
        public string Name { get; set; }

        public int ArticleType { get; set; }

        [Required]
        public string Site { get; set; }
        public string Url { get; set; }
        public string UrlTemp { get; set; }
        public string SourceCode { get; set; }
        [Required]
        public int CommandType { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "必须大于等于0")]
        public int Interval { get; set; }

        [BsonIgnore]
        public System.Timers.Timer Timer { get; set; }
    }
}