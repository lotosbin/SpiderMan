using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using sharp_net.Mongo;
using System.Web.Mvc;
using sharp_net.Repositories;

namespace SpiderMan.Models {
    public class Site : MEntity {
        public int Act { get; set; }
        [Required]
        public string Name { get; set; }
        public string Link { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "必须大于0")]
        public int GrabInterval { get; set; }

        public static void Initialization() {
            var repo = DependencyResolver.Current.GetService(typeof(IMongoRepo<Site>)) as MongoRepo<Site>;
            if (!repo.Collection.AsQueryable<Site>().Any(d => d.Name == "douban")) {
                var douban = new Site {
                    Name = "douban",
                    Act = (int)eAct.Normal,
                    GrabInterval = 10,
                    Link = "http://www.douban.com"
                };
                repo.Collection.Insert(douban);
            }
            if (!repo.Collection.AsQueryable<Site>().Any(d => d.Name == "imdb")) {
                var imdb = new Site {
                    Name = "imdb",
                    Act = (int)eAct.Normal,
                    GrabInterval = 10,
                    Link = "http://www.imdb.com"
                };
                repo.Collection.Insert(imdb);
            }
        }
    }
}