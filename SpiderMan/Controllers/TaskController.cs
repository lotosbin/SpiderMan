using MongoDB.Bson;
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
using SpiderMan.Entity;
using System.Configuration;
using Baozou.Entity;
using sharp_net;

namespace SpiderMan.Controllers {

    public class TaskController : Controller {
        private readonly MongoCollection<TaskModel> taskModelCollection;
        private readonly MongoCollection<GgpttCard> ggpttCardCollection;
        private readonly MongoCollection<AdianboVideo> adianboVideoCollection;
        private readonly MongoCollection<Shudong> shudongCollection;
        private readonly MongoCollection<Match> baozouMatchCollection;
        private readonly MongoCollection<Team> baozouTeamCollection;
        private readonly MongoCollection<LiveVideo> baozouLiveCollection;
        public TaskController(IMongoRepo<TaskModel> taskmodel_repo,
            IMongoRepo<GgpttCard> ggpttcard_repo,
            IMongoRepo<AdianboVideo> adianbovideo_repo,
            IMongoRepo<Shudong> shudong_repo,
            IMongoRepo<Team> baozouteam_repo,
            IMongoRepo<LiveVideo> baozoulive_repo,
            IMongoRepo<Match> baozoumatch_repo) {
            this.taskModelCollection = taskmodel_repo.Collection;
            this.ggpttCardCollection = ggpttcard_repo.Collection;
            this.adianboVideoCollection = adianbovideo_repo.Collection;
            this.shudongCollection = shudong_repo.Collection;
            this.baozouMatchCollection = baozoumatch_repo.Collection;
            this.baozouTeamCollection = baozouteam_repo.Collection;
            this.baozouLiveCollection = baozoulive_repo.Collection;
        }

        public ActionResult Index() {
            ViewBag.TaskModel = taskModelCollection.AsQueryable<TaskModel>().Where(d => d.Act == (int)eAct.Normal && d.Interval > 0);
            return View();
        }

        #region Ggptt
        // 关于在web api使用FormDataCollection http://goo.gl/PjJGf
        // 不要使用gbk编码提交，很容易产生字符串错误从而无法提交。
        [HttpPost]
        [ValidateInput(false)]
        public void PostGgpttCardList(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            datajson = FilterConfig.htmlFilter.Filter(datajson, true);
            var data = JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<GgpttCard>)) as IEnumerable<GgpttCard>;
            foreach (var item in data) {
                var exist = ggpttCardCollection.AsQueryable<GgpttCard>().SingleOrDefault(d => d.SourceCode == task.Source && d.ProviderId == item.ProviderId);
                if (exist == null) {
                    item.Inject(task);
                    ggpttCardCollection.Insert(item);
                    item.DownloadImagesLocal();
                } else {
                    exist.GrabDate = DateTime.Now;
                    exist.Grade = item.Grade;
                    ggpttCardCollection.Save(exist);
                }
            }
        }

        [HttpPost]
        public void PostGgpttCardIds(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            var taskModel = taskModelCollection.AsQueryable<TaskModel>().Single(d => d.Id == task.TaskModelId);
            var data = JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<string>)) as IEnumerable<string>;
            foreach (string id in data) {
                if (!ggpttCardCollection.AsQueryable<GgpttCard>().Any(d => d.ProviderId == id & d.SourceCode == taskModel.SourceCode)) {
                    TaskQueue.tasks.Add(new SpiderTask {
                        Id = Guid.NewGuid(),
                        Site = taskModel.Site,
                        Source = taskModel.SourceCode,
                        CommandType = eCommandType.One.ToString(),
                        Url = String.Format(taskModel.UrlTemp, id),
                        ArticleType = eArticleType.GgpttCard.ToString()
                    });
                }
            }
            TaskQueue.masterhub.Clients.Group("broad").broadcastRanderTask(TaskQueue.tasks);
        }

        [HttpPost]
        [ValidateInput(false)]
        public void PostGgpttCardOne(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            datajson = FilterConfig.htmlFilter.Filter(datajson, true);
            var data = JsonConvert.DeserializeObject(datajson, typeof(GgpttCard)) as GgpttCard;
            var exist = ggpttCardCollection.AsQueryable<GgpttCard>().SingleOrDefault(d => d.ProviderId == data.ProviderId & d.SourceCode == task.Source);
            if (exist == null) {
                data.Inject(task);
                ggpttCardCollection.Insert(data);
                data.DownloadImagesLocal();
            } else {
                exist.GrabDate = DateTime.Now;
                exist.Grade = data.Grade;
                ggpttCardCollection.Save(exist);
            }
        }
        #endregion

        #region BaozouMatch
        [HttpPost]
        public void PostBaozouMatchList(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            var data = JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<Match>)) as IEnumerable<Match>;
            foreach (var m in data) {
                var exist = baozouMatchCollection.AsQueryable<Match>().SingleOrDefault(d => d.KanbisaiLink == m.KanbisaiLink & d.SourceCode == task.Source);
                if (exist == null) {
                    m.SourceCode = task.Source;
                    m.InitializeCap();

                    var team = baozouTeamCollection.FindOne(Query<Team>.EQ(e => e.NameChinese, m.TeamNameChinese));
                    if (team != null)
                        m.TeamName = team.Name;
                    var teamGorGuest = baozouTeamCollection.FindOne(Query<Team>.EQ(e => e.NameChinese, m.TeamNameChineseForGuest));
                    if (teamGorGuest != null)
                        m.TeamNameForGuest = teamGorGuest.Name;

                    baozouMatchCollection.Insert(m);
                } else {
                    exist.Point = m.Point;
                    exist.PointForGuest = m.PointForGuest;
                    exist.Status = m.Status;
                    exist.Quarter = m.Quarter;
                    exist.QuarterTime = m.QuarterTime;
                    baozouMatchCollection.Save(exist);
                }
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public void PostBaozouMatchAddition(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            var data = JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<Match>)) as IEnumerable<Match>;
            foreach (Match m in data) {
                if (m.CapString == "CBA" && m.Time.Hour == 19 && m.Time.Minute == 35)
                    m.Time = m.Time.Date + new TimeSpan(19, 30, 0);
                IMongoQuery queryPre = Query.And(
                    Query<Match>.EQ(e => e.CapString, m.CapString),
                    Query<Match>.EQ(e => e.Time, m.Time)
                );
                var match = baozouMatchCollection.FindOne(Query.And(
                    queryPre, 
                    Query<Match>.EQ(e => e.Title, m.Title)
                ));
                if (match == null) {
                    var matchs = baozouMatchCollection.Find(queryPre);
                    if (matchs == null) continue;
                    foreach (var item in matchs.ToList()) {
                        if (m.Title.Contains(item.TeamNameChinese) && m.Title.Contains(item.TeamNameChineseForGuest)) {
                            match = item;
                            break;
                        }
                    }
                }

                if (match == null) continue;
                match.Title = m.Title;
                match.LiveText = m.LiveText;
                IEnumerable<string> liveString = m.LiveVideos.Select(d => d.Name);
                match.LiveVideos = from d in baozouLiveCollection.AsQueryable<LiveVideo>()
                                   where d.Alias.ContainsAny(liveString) //http://goo.gl/WyLqec 1.5+版本支持
                                   orderby d.Rank descending
                                   select new Link {
                                       Name = d.Name + (d.WithClient ? "φ" : ""),
                                       Url = d.Link
                                   };
                match.FixKanbisaiLiveVideos();
                baozouMatchCollection.Save(match);
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public void PostBaozouMatchAdditionMobile(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            var data = JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<Match>)) as IEnumerable<Match>;
            foreach (Match m in data) {
                if (m.CapString == "CBA" && m.Time.Hour == 19 && m.Time.Minute == 35) {
                    m.Time = m.Time.Date + new TimeSpan(19, 30, 0);
                }
                IMongoQuery queryPre = Query.And(
                    Query<Match>.EQ(e => e.CapString, m.CapString),
                    Query<Match>.EQ(e => e.Time, m.Time)
                );
                var match = baozouMatchCollection.FindOne(Query.And(
                    queryPre,
                    Query<Match>.EQ(e => e.Title, m.Title)
                ));
                if (match == null) {
                    var matchs = baozouMatchCollection.Find(queryPre);
                    if (matchs == null) continue;
                    foreach (var item in matchs.ToList()) {
                        if (m.Title.Contains(item.TeamNameChinese) && m.Title.Contains(item.TeamNameChineseForGuest)) {
                            match = item;
                            break;
                        }
                    }
                }

                if (match == null) continue;
                match.Title = m.Title;
                match.LiveTextForMobile = m.LiveTextForMobile;
                IEnumerable<string> liveString = m.LiveVideosForMobile.Select(d => d.Name);
                match.LiveVideosForMobile = from d in baozouLiveCollection.AsQueryable<LiveVideo>()
                                            where d.AliasForMobile.ContainsAny(liveString) //http://goo.gl/WyLqec 1.5+版本支持
                                            orderby d.Rank descending
                                            select new Link {
                                                Name = d.Name,
                                                Url = d.LinkForMobile
                                            };
                match.FixKanbisaiLiveVideos();
                baozouMatchCollection.Save(match);
            }
        }
        #endregion

        #region Shudong
        [HttpPost]
        public void PostShudongIds(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            var taskModel = taskModelCollection.AsQueryable<TaskModel>().Single(d => d.Id == task.TaskModelId);
            var data = JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<string>)) as IEnumerable<string>;
            foreach (string id in data) {
                if (!ggpttCardCollection.AsQueryable<Shudong>().Any(d => d.ProviderId == id & d.SourceCode == taskModel.SourceCode)) {
                    TaskQueue.tasks.Add(new SpiderTask {
                        Id = Guid.NewGuid(),
                        Site = taskModel.Site,
                        Source = taskModel.SourceCode,
                        CommandType = eCommandType.One.ToString(),
                        Url = String.Format(taskModel.UrlTemp, id),
                        ArticleType = eArticleType.GgpttCard.ToString()
                    });
                }
            }
            TaskQueue.masterhub.Clients.Group("broad").broadcastRanderTask(TaskQueue.tasks);
        }

        [HttpPost]
        [ValidateInput(false)]
        public void PostShudongOne(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            datajson = FilterConfig.htmlFilter.Filter(datajson, true);
            var data = JsonConvert.DeserializeObject(datajson, typeof(Shudong)) as Shudong;
            var exist = ggpttCardCollection.AsQueryable<Shudong>().SingleOrDefault(d => d.ProviderId == data.ProviderId & d.SourceCode == task.Source);
            if (exist == null) {
                data.Inject(task);
                shudongCollection.Insert(data);
            } else {
                exist.GrabDate = DateTime.Now;
                exist.Grade = data.Grade;
                foreach (var comment in data.Comments) {
                    if (!exist.Comments.Any<string>(d => d == comment))
                        exist.Comments.Add(comment);
                }
                shudongCollection.Save(exist);
            }
        }
        #endregion

        #region Adianbo
        [HttpPost]
        [ValidateInput(false)]
        public void PostAdianboVideoIds(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            var taskModel = taskModelCollection.AsQueryable<TaskModel>().Single(d => d.Id == task.TaskModelId);
            var data = JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<string>)) as IEnumerable<string>;
            foreach (string id in data) {
                TaskQueue.tasks.Add(new SpiderTask {
                    Id = Guid.NewGuid(),
                    Site = taskModel.Site,
                    Source = taskModel.SourceCode,
                    CommandType = eCommandType.One.ToString(),
                    Url = String.Format(taskModel.UrlTemp, id),
                    ArticleType = eArticleType.AdianboVideo.ToString()
                });
            }
            TaskQueue.masterhub.Clients.Group("broad").broadcastRanderTask(TaskQueue.tasks);
        }

        [HttpPost]
        [ValidateInput(false)]
        public void PostAdianboVideoOne(string taskjson, string datajson) {
            datajson = FilterConfig.htmlFilter.Filter(datajson, true);
            var data = JsonConvert.DeserializeObject(datajson, typeof(VideoSource)) as VideoSource;
            var exist = ggpttCardCollection.AsQueryable<VideoSource>().SingleOrDefault(d => d.ProviderId == data.ProviderId & d.SourceCode == data.SourceCode);
            if (exist == null) {
                var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
                data.Inject(task);
                adianboVideoCollection.Insert(data);
            } else {
                exist.GrabDate = DateTime.Now;
                foreach (var source in data.Links) {
                    if (!exist.Links.Any<VideoLink>(d => d.FileName == source.FileName))
                        exist.Links.Add(source);
                }
                adianboVideoCollection.Save(exist);
            }
            var doubanTaskModel = taskModelCollection.AsQueryable<TaskModel>().Single(d => d.Name == "DoubanOne");
            var imdbTaskModel = taskModelCollection.AsQueryable<TaskModel>().Single(d => d.Name == "ImdbOne");
            if (data.IsTeleplay) {
                TaskQueue.tasks.Add(new SpiderTask {
                    Id = Guid.NewGuid(),
                    TaskModelId = doubanTaskModel.Id,
                    Site = "douban",
                    Source = "douban",
                    CommandType = eCommandType.ListFirst.ToString(),
                    Url = String.Format("http://movie.douban.com/subject_search?search_text={0}", data.Name),
                    ArticleType = eArticleType.AdianboVideo.ToString()
                });
                TaskQueue.tasks.Add(new SpiderTask {
                    Id = Guid.NewGuid(),
                    TaskModelId = imdbTaskModel.Id,
                    Site = "imdb",
                    Source = "imdb",
                    CommandType = eCommandType.ListFirst.ToString(),
                    Url = String.Format("http://www.imdb.com/find?q={0}", data.Name),
                    ArticleType = eArticleType.AdianboVideo.ToString()
                });
            } else {
                TaskQueue.tasks.Add(new SpiderTask {
                    Id = Guid.NewGuid(),
                    TaskModelId = doubanTaskModel.Id,
                    Site = "douban",
                    Source = "douban",
                    CommandType = eCommandType.ListFirst.ToString(),
                    Url = String.Format("http://movie.douban.com/subject_search?search_text={0}", data.ImdbId),
                    ArticleType = eArticleType.AdianboVideo.ToString()
                });
                TaskQueue.tasks.Add(new SpiderTask {
                    Id = Guid.NewGuid(),
                    TaskModelId = imdbTaskModel.Id,
                    Site = "imdb",
                    Source = "imdb",
                    CommandType = eCommandType.One.ToString(),
                    Url = String.Format("http://www.imdb.com/title/{0}", data.ImdbId),
                    ArticleType = eArticleType.AdianboVideo.ToString()
                });
            }
            TaskQueue.masterhub.Clients.Group("broad").broadcastRanderTask(TaskQueue.tasks);
        }

        [HttpPost]
        public void PostAdianboVideoListFirst(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            var taskModel = taskModelCollection.AsQueryable<TaskModel>().Single(d => d.Id == task.TaskModelId);
            TaskQueue.tasks.Add(new SpiderTask {
                Id = Guid.NewGuid(),
                Site = taskModel.Site,
                Source = taskModel.SourceCode,
                CommandType = eCommandType.One.ToString(),
                Url = String.Format(taskModel.UrlTemp, datajson),
                ArticleType = eArticleType.AdianboVideo.ToString()
            });
            TaskQueue.masterhub.Clients.Group("broad").broadcastRanderTask(TaskQueue.tasks);
        }

        [HttpPost]
        [ValidateInput(false)]
        public void PostAdianboVideoAddition(string taskjson, string datajson) {
            datajson = FilterConfig.htmlFilter.Filter(datajson, true);
            var data = JsonConvert.DeserializeObject(datajson, typeof(AdianboVideo)) as AdianboVideo;
            var query = Query<AdianboVideo>.EQ(d => d.ImdbId, data.ImdbId);
            adianboVideoCollection.Update(query, AdianboVideo.UpdateBuilder(data), UpdateFlags.Upsert);
        }
        #endregion
    }
}
