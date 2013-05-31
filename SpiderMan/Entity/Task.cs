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

        public TaskModel Model { get; set; }

        [BsonRepresentation(BsonType.String)]
        public TaskStatus Status { get; set; }

        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime ComplateDate { get; set; }

        public string handlerAgent { get; set; }
        
    }
}