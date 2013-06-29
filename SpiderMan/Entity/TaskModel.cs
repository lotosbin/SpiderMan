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

        //[BsonRepresentation(BsonType.String)]
        public int ArticleType { get; set; }
        public eArticleType ArticleTypeEnum {
            get {
                return (eArticleType)ArticleType;
            }
        }

        public string Site { get; set; }
        public string Url { get; set; }

        //public int startIndex { get; set; }
        //public int stopIndex { get; set; }
        //public bool ArraysUnion { get; set; }

        public string Command { get; set; }
        public int CommandType { get; set; }
        public string Encoding { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "必须大于0")]
        public int Interval { get; set; }
    }
}