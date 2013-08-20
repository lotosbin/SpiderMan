using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SpiderMan.Entity {

    //因为Entity标注了DataContract。不加DataContract的话，默认所有public的属性都要序列化，
    //加了之后，只有标注的[DataMember]属性（或字段）才能序列化，即使是私有的。
    public class GgpttCard : Article {
        public string Title { get; set; }


    }
}