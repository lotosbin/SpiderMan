using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using sharp_net.Mongo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SpiderMan.Entity {

    public class Image {
        public string Link { get; set; }
        public string SmallLink { get; set; }
        public string Title { get; set; }
        public string Describe { get; set; }
    }
}