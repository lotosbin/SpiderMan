using MongoDB.Bson.Serialization.Attributes;
using sharp_net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Configuration;

namespace SpiderMan.Entity {

    //因为Entity标注了DataContract。不加DataContract的话，默认所有public的属性都要序列化，
    //加了之后，只有标注的[DataMember]属性（或字段）才能序列化，即使是私有的。
    public class GgpttCard : Article {
        public string Title { get; set; }
        public IEnumerable<Image> Images { get; set; }
        public IEnumerable<string> LocalImages { get; set; }

        public IEnumerable<string> defTagCodes { get; set; } //Bridge

        public void DownloadImagesLocal() {
            if (LocalImages == null) return;
            WebRequestRobot webRequestRobot = new WebRequestRobot();
            foreach (string imgstring in LocalImages) {
                Uri uri = new Uri(imgstring);
                string filename = imgstring.Replace(uri.Scheme + "://" + uri.Authority, ConfigurationManager.AppSettings["LocalImageStore"] + SourceCode);
                filename = filename.Replace("/", "\\");
                webRequestRobot.DownloadImage(imgstring, filename);
            }
        }
    }
}