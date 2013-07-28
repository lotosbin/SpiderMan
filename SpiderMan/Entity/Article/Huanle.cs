using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SpiderMan.Models {

    //因为Entity标注了DataContract。不加DataContract的话，默认所有public的属性都要序列化，
    //加了之后，只有标注的[DataMember]属性（或字段）才能序列化，即使是私有的。
    public class Huanle : Article {
        public int ThumbUps { get; set; }
        public int ThumbDowns { get; set; }
        public int Amount { get; set; }
        public int Grade { get; set; }

        //[DataMember]
        //public List<Comment> Comments { get; set; }
    }
}