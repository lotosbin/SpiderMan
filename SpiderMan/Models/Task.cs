using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiderMan.Models {
    public class Task : Entity {

        public int QueueId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public TaskType Type { get; set; }

        [BsonRepresentation(BsonType.String)]
        public TaskStatus Status { get; set; }

        public string Url { get; set; }

        public int startIndex { get; set; }
        public int stopIndex { get; set; }

        public string Command { get; set; }
        public bool ArraysUnion { get; set; }

        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime ComplateDate { get; set; }
        
    }
}