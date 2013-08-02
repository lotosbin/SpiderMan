﻿using MongoDB.Bson;
using MongoDB.Driver.Linq;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using sharp_net.Infrastructure;
using sharp_net.Mvc;
using SpiderMan.Help;
using SpiderMan.Models;
using sharp_net.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Mvc;
using sharp_net.Repositories;

namespace SpiderMan.Controllers {

    public class TaskController : Controller {
        private readonly MongoCollection<TaskModel> taskModelCollection;
        private readonly MongoCollection<GgpttCard> ggpttCardCollection;
        private readonly MongoCollection<AdianboVideo> adianboVideoCollection;
        private readonly MongoCollection<Shudong> shudongCollection;
        public TaskController(IMongoRepo<TaskModel> taskmodel_repo,
            IMongoRepo<GgpttCard> ggpttcard_repo,
            IMongoRepo<AdianboVideo> adianbovideo_repo,
            IMongoRepo<Shudong> shudong_repo) {
            this.taskModelCollection = taskmodel_repo.Collection;
            this.ggpttCardCollection = ggpttcard_repo.Collection;
            this.adianboVideoCollection = adianbovideo_repo.Collection;
            this.shudongCollection = shudong_repo.Collection;
        }

        public ActionResult Index() {
            ViewBag.TaskModel = taskModelCollection.AsQueryable<TaskModel>().Where(d => d.Act == (int)eAct.Normal && d.Interval > 0);
            return View();
        }

        // 关于在web api使用FormDataCollection http://goo.gl/PjJGf
        // 不要使用gbk编码提交，很容易产生字符串错误从而无法提交。
        [HttpPost]
        [ValidateInput(false)]
        public void PostGgpttCardList(string datajson) {
            datajson = FilterConfig.htmlFilter.Filter(datajson, true);
            var data = JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<GgpttCard>)) as IEnumerable<GgpttCard>;
            foreach (var item in data) {
                //item.Content = FilterConfig.htmlFilter.Filter(item.Content);
                var exist = ggpttCardCollection.AsQueryable<GgpttCard>().SingleOrDefault(d => d.ProviderId == item.ProviderId);
                if (exist == null) {
                    ggpttCardCollection.InsertBatch(data);
                } else {
                    exist.Grade = item.Grade;
                    ggpttCardCollection.Save(exist);
                }
            }
        }

        [HttpPost]
        public void PostGgpttCardIds(string taskmodelid, string datajson) {
            var taskModel = taskModelCollection.AsQueryable<TaskModel>().Single(d => d.Id == taskmodelid);
            var data = JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<string>)) as IEnumerable<string>;
            foreach (string id in data) {
                if (!ggpttCardCollection.AsQueryable<GgpttCard>().Any(d => d.ProviderId == id & d.SourceSite == taskModel.Site)) {
                    var newTask = new SpiderTask {
                        Id = Guid.NewGuid(),
                        Site = taskModel.Site,
                        CommandType = eCommandType.One.ToString(),
                        Url = String.Format(taskModel.Url, id),
                        ArticleType = eArticleType.GgpttCard.ToString()
                    };
                    TaskQueue.tasks.Add(newTask);
                }
            }
            TaskQueue.firsthub.Clients.Group("broad").broadcastRanderTask(TaskQueue.tasks);
        }

        [HttpPost]
        [ValidateInput(false)]
        public void PostGgpttCardOne(string datajson) {
            datajson = FilterConfig.htmlFilter.Filter(datajson, true);
            var data = JsonConvert.DeserializeObject(datajson, typeof(GgpttCard)) as GgpttCard;
            var exist = ggpttCardCollection.AsQueryable<GgpttCard>().SingleOrDefault(d => d.ProviderId == data.ProviderId & d.SourceSite == data.SourceSite);
            if (exist == null) {
                ggpttCardCollection.Insert(data);
            } else {
                exist.Grade = data.Grade;
                ggpttCardCollection.Save(exist);
            }
        }

        [HttpPost]
        public void PostShudongIds(string taskmodelid, string datajson) {
            var taskModel = taskModelCollection.AsQueryable<TaskModel>().Single(d => d.Id == taskmodelid);
            var data = JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<string>)) as IEnumerable<string>;
            foreach (string id in data) {
                if (!ggpttCardCollection.AsQueryable<Shudong>().Any(d => d.ProviderId == id & d.SourceSite == taskModel.Site)) {
                    var newTask = new SpiderTask {
                        Id = Guid.NewGuid(),
                        Site = taskModel.Site,
                        CommandType = eCommandType.One.ToString(),
                        Url = String.Format(taskModel.UrlTemp, id),
                        ArticleType = eArticleType.GgpttCard.ToString()
                    };
                    TaskQueue.tasks.Add(newTask);
                }
            }
            TaskQueue.firsthub.Clients.Group("broad").broadcastRanderTask(TaskQueue.tasks);
        }

        [HttpPost]
        [ValidateInput(false)]
        public void PostShudongOne(string datajson) {
            datajson = FilterConfig.htmlFilter.Filter(datajson, true);
            var data = JsonConvert.DeserializeObject(datajson, typeof(Shudong)) as Shudong;
            var exist = ggpttCardCollection.AsQueryable<Shudong>().SingleOrDefault(d => d.ProviderId == data.ProviderId & d.SourceSite == data.SourceSite);
            if (exist == null) {
                shudongCollection.Insert(data);
            } else {
                exist.Grade = data.Grade;
                foreach (var comment in data.Comments) {
                    if (!exist.Comments.Any<string>(d => d == comment))
                        exist.Comments.Add(comment);
                }
                shudongCollection.Save(exist);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public void PostAdianboVideoIds(string taskmodelid, string datajson) {
            var taskModel = taskModelCollection.AsQueryable<TaskModel>().Single(d => d.Id == taskmodelid);
            var data = JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<string>)) as IEnumerable<string>;
            foreach (string id in data) {
                var newTask = new SpiderTask {
                    Id = Guid.NewGuid(),
                    Site = taskModel.Site,
                    CommandType = eCommandType.One.ToString(),
                    Url = String.Format(taskModel.UrlTemp, id),
                    ArticleType = eArticleType.AdianboVideo.ToString()
                };
                TaskQueue.tasks.Add(newTask);
            }
            TaskQueue.firsthub.Clients.Group("broad").broadcastRanderTask(TaskQueue.tasks);
        }

        [HttpPost]
        [ValidateInput(false)]
        public void PostAdianboVideoOne(string datajson) {
            datajson = FilterConfig.htmlFilter.Filter(datajson, true);
            var data = JsonConvert.DeserializeObject(datajson, typeof(VideoSource)) as VideoSource;
            var exist = ggpttCardCollection.AsQueryable<VideoSource>().SingleOrDefault(d => d.ProviderId == data.ProviderId & d.SourceSite == data.SourceSite);
            if (exist == null) {
                adianboVideoCollection.Insert(data);
            } else {
                foreach (var source in data.Links) {
                    if (!exist.Links.Any<VideoLink>(d => d.FileName == source.FileName))
                        exist.Links.Add(source);
                }
                adianboVideoCollection.Save(exist);
            }
            var doubanTaskModel = taskModelCollection.AsQueryable<TaskModel>().Single(d => d.Name == "DoubanOne");
            var imdbTaskModel = taskModelCollection.AsQueryable<TaskModel>().Single(d => d.Name == "ImdbOne");
            if (data.IsTeleplay) {
                var doubanTask = new SpiderTask {
                    Id = Guid.NewGuid(),
                    TaskModelId = doubanTaskModel.Id,
                    Site = "douban",
                    CommandType = eCommandType.ListFirst.ToString(),
                    Url = String.Format("http://movie.douban.com/subject_search?search_text={0}", data.Name),
                    ArticleType = eArticleType.AdianboVideo.ToString()
                };
                TaskQueue.tasks.Add(doubanTask);
                var imdbTask = new SpiderTask {
                    Id = Guid.NewGuid(),
                    TaskModelId = imdbTaskModel.Id,
                    Site = "imdb",
                    CommandType = eCommandType.ListFirst.ToString(),
                    Url = String.Format("http://www.imdb.com/find?q={0}", data.Name),
                    ArticleType = eArticleType.AdianboVideo.ToString()
                };
                TaskQueue.tasks.Add(imdbTask);
            } else {
                var doubanTask = new SpiderTask {
                    Id = Guid.NewGuid(),
                    TaskModelId = doubanTaskModel.Id,
                    Site = "douban",
                    CommandType = eCommandType.ListFirst.ToString(),
                    Url = String.Format("http://movie.douban.com/subject_search?search_text={0}", data.ImdbId),
                    ArticleType = eArticleType.AdianboVideo.ToString()
                };
                TaskQueue.tasks.Add(doubanTask);
                var imdbTask = new SpiderTask {
                    Id = Guid.NewGuid(),
                    TaskModelId = imdbTaskModel.Id,
                    Site = "imdb",
                    CommandType = eCommandType.One.ToString(),
                    Url = String.Format("http://www.imdb.com/title/{0}", data.ImdbId),
                    ArticleType = eArticleType.AdianboVideo.ToString()
                };
                TaskQueue.tasks.Add(imdbTask);
            }
            TaskQueue.firsthub.Clients.Group("broad").broadcastRanderTask(TaskQueue.tasks);
        }

        [HttpPost]
        public void PostAdianboVideoListFirst(string taskmodelid, string datajson) {
            var taskModel = taskModelCollection.AsQueryable<TaskModel>().Single(d => d.Id == taskmodelid);
            var newTask = new SpiderTask {
                Id = Guid.NewGuid(),
                Site = taskModel.Site,
                CommandType = eCommandType.One.ToString(),
                Url = String.Format(taskModel.UrlTemp, datajson),
                ArticleType = eArticleType.AdianboVideo.ToString()
            };
            TaskQueue.tasks.Add(newTask);
            TaskQueue.firsthub.Clients.Group("broad").broadcastRanderTask(TaskQueue.tasks);
        }

        [HttpPost]
        [ValidateInput(false)]
        public void PostAdianboVideoAddition(string datajson) {
            datajson = FilterConfig.htmlFilter.Filter(datajson, true);
            var data = JsonConvert.DeserializeObject(datajson, typeof(AdianboVideo)) as AdianboVideo;
            var query = Query<AdianboVideo>.EQ(d => d.ImdbId, data.ImdbId);
            adianboVideoCollection.Update(query, AdianboVideo.UpdateBuilder(data), UpdateFlags.Upsert);
        }

    }
}
