using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using sharp_net.Mvc;
using System.ComponentModel.DataAnnotations;
using sharp_net.Mongo;
using MongoDB.Driver.Linq;
using sharp_net.Repositories;

namespace SpiderMan.Models {
    public class TaskModel : MEntity {
        public int Act { get; set; }
        public string Name { get; set; }

        public int ArticleType { get; set; }

        [Required]
        public string Site { get; set; }
        public string Url { get; set; }
        public string UrlTemp { get; set; }
        [Required]
        public int CommandType { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "必须大于等于0")]
        public int Interval { get; set; }

        [BsonIgnore]
        public System.Timers.Timer Timer { get; set; }

        public static void Initialization() {
            var repo = DependencyResolver.Current.GetService(typeof(IMongoRepo<TaskModel>)) as MongoRepo<TaskModel>;
            if (!repo.Collection.AsQueryable<TaskModel>().Any(d => d.Name == "DoubanOne")) {
                var douban = new TaskModel {
                    Name = "DoubanOne",
                    Act = (int)eAct.Normal,
                    ArticleType = (int)eArticleType.AdianboVideo,
                    Site = "douban",
                    UrlTemp = "http://movie.douban.com/subject/{0}/",
                    CommandType = (int)eCommandType.One
                };
                repo.Collection.Insert(douban);
            }
            if (!repo.Collection.AsQueryable<TaskModel>().Any(d => d.Name == "ImdbOne")) {
                var imdb = new TaskModel {
                    Name = "ImdbOne",
                    Act = (int)eAct.Normal,
                    ArticleType = (int)eArticleType.AdianboVideo,
                    Site = "imdb",
                    UrlTemp = "http://www.imdb.com/title/{0}/",
                    CommandType = (int)eCommandType.One
                };
                repo.Collection.Insert(imdb);
            }
        }
    }
}