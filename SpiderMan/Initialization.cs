using sharp_net.Mongo;
using MongoDB.Driver.Linq;
using sharp_net.Repositories;
using SpiderMan.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpiderMan {
    public static class Initialization {
        public static void SiteInit() {
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

        public static void TaskModelInit() {
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
