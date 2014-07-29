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

    public class BaozouMatchController : Controller {
        private readonly MongoCollection<TaskModel> taskModelCollection;
        private readonly MongoCollection<Match> baozouMatchCollection;
        private readonly MongoCollection<Team> baozouTeamCollection;
        private readonly MongoCollection<LiveVideo> baozouLiveCollection;
        public BaozouMatchController(IMongoRepo<TaskModel> taskmodel_repo,
            IMongoRepo<Team> baozouteam_repo,
            IMongoRepo<LiveVideo> baozoulive_repo,
            IMongoRepo<Match> baozoumatch_repo) {
            this.taskModelCollection = taskmodel_repo.Collection;
            this.baozouMatchCollection = baozoumatch_repo.Collection;
            this.baozouTeamCollection = baozouteam_repo.Collection;
            this.baozouLiveCollection = baozoulive_repo.Collection;
        }

        public bool AnalyzeCapString(Match m) {
            m.Type = 0;
            m.Cap = 0;
            foreach (var cap in (eSoccerCap[])Enum.GetValues(typeof(eSoccerCap))) {
                string capChinese = cap.GetAttachedData<string>(DescripCap.Chinese);
                if (m.CapString == capChinese || m.CapString.Contains(capChinese)) {
                    m.Type = (int)eMatchType.Soccer;
                    m.Cap = (int)cap;
                    if (m.CapString != capChinese) {
                        m.CapStringDetial = m.CapString;
                        m.CapString = capChinese;
                    }
                    break;
                }
            }
            if (m.Type != 0) return true;
            foreach (var cap in (eBasketballCap[])Enum.GetValues(typeof(eBasketballCap))) {
                string capChinese = cap.GetAttachedData<string>(DescripCap.Chinese);
                if (m.CapString == capChinese || m.CapString.Contains(capChinese)) {
                    m.Type = (int)eMatchType.Nba;
                    m.Cap = (int)cap;
                    if (m.CapString != capChinese) {
                        m.CapStringDetial = m.CapString;
                        m.CapString = capChinese;
                    }
                    break;
                }
            }
            if (m.Type != 0) return true;
            foreach (var cap in (eTennisCap[])Enum.GetValues(typeof(eTennisCap))) {
                string capChinese = cap.GetAttachedData<string>(DescripCap.Chinese);
                if (m.CapString == capChinese || m.CapString.Contains(capChinese)) {
                    m.Type = (int)eMatchType.Tennis;
                    m.Cap = (int)cap;
                    if (m.CapString != capChinese) {
                        m.CapStringDetial = m.CapString;
                        m.CapString = capChinese;
                    }
                    break;
                }
            }
            if (m.Type != 0) return true;
            foreach (var cap in (eArtsCap[])Enum.GetValues(typeof(eArtsCap))) {
                string capChinese = cap.GetAttachedData<string>(DescripCap.Chinese);
                if (m.CapString == capChinese || m.CapString.Contains(capChinese)) {
                    m.Type = (int)eMatchType.Arts;
                    m.Cap = (int)cap;
                    if (m.CapString != capChinese) {
                        m.CapStringDetial = m.CapString;
                        m.CapString = capChinese;
                    }
                    break;
                }
            }
            if (m.Type != 0) return true;
            if (m.CapString.Contains("斯诺克")) {
                m.Type = (int)eMatchType.Billiards;
            }
            return (m.Type != 0);
        }

        private bool InjectTeamName(Match m) {
            bool exitTeam = false;
            var team = baozouTeamCollection.FindOne(Query<Team>.EQ(e => e.NameChinese, m.TeamNameChinese));
            if (team != null) { 
                m.TeamName = team.Name; exitTeam = true; 
            }
            var teamGorGuest = baozouTeamCollection.FindOne(Query<Team>.EQ(e => e.NameChinese, m.TeamNameChineseForGuest));
            if (teamGorGuest != null) { 
                m.TeamNameForGuest = teamGorGuest.Name; exitTeam = true;
            }
            return exitTeam;
        }

        private Match QueryExitMatch(Match m) {
            return baozouMatchCollection.FindOne(Query.And(
                Query.Or(
                    Query<Match>.EQ(e => e.CapString, m.CapString),
                    Query<Match>.EQ(e => e.CapStringDetial, m.CapString)
                ),
                Query<Match>.EQ(e => e.TeamNameChinese, m.TeamNameChinese),
                Query<Match>.EQ(e => e.TeamNameChineseForGuest, m.TeamNameChineseForGuest),
                Query.And(
                    Query<Match>.GT(e => e.Time, m.Time.Subtract(new TimeSpan(3, 0, 0))),
                    Query<Match>.LT(e => e.Time, m.Time.AddHours(3))
                )
            ));
        }

        #region Kanbisai
        [HttpPost]
        public void List_kanbisai(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            var data = JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<Match>)) as IEnumerable<Match>;
            foreach (var m in data) {
                var exist = QueryExitMatch(m);
                if (exist == null) {
                    if (AnalyzeCapString(m) && InjectTeamName(m)) {
                        m.SourceCode = task.Source;
                        baozouMatchCollection.Insert(m);
                    }
                } else {
                    exist.Point = m.Point;
                    exist.PointForGuest = m.PointForGuest;
                    if (m.Status > 0) exist.Status = m.Status;
                    exist.Quarter = m.Quarter;
                    exist.QuarterTime = m.QuarterTime;
                    if (exist.Status == (int)eMatchStatus.Ago && exist.Time > DateTime.Now.Subtract(new TimeSpan(24, 0, 0))) {
                        TaskQueue.tasks.Add(new SpiderTask {
                            Id = Guid.NewGuid(),
                            Source = "kanbisai",
                            Site = "kanbisai",
                            CommandType = eCommandType.One.ToString(),
                            Url = exist.KanbisaiLink,
                            ArticleType = eArticleType.BaozouMatch.ToString(),
                            PostSourceName = true
                        });
                        TaskQueue.tasks.Add(new SpiderTask {
                            Id = Guid.NewGuid(),
                            Source = "kanbisai",
                            Site = "kanbisai",
                            CommandType = eCommandType.One.ToString(),
                            Url = exist.KanbisaiLink,
                            ArticleType = eArticleType.BaozouMatch.ToString(),
                            IsMobile = true,
                            PostSourceName = true
                        });
                    }
                    baozouMatchCollection.Save(exist);
                }
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public void One_kanbisai(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            datajson = FilterConfig.htmlFilter.Filter(datajson, true);
            var m = JsonConvert.DeserializeObject(datajson, typeof(Match)) as Match;
            var exist = baozouMatchCollection.AsQueryable<Match>().SingleOrDefault(d => d.KanbisaiLink == m.KanbisaiLink);
            if (exist != null && !string.IsNullOrEmpty(m.BestVideo)) {
                exist.BestVideo = m.BestVideo;
                baozouMatchCollection.Save(exist);
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public void One_kanbisai_mobi(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            datajson = FilterConfig.htmlFilter.Filter(datajson, true);
            var m = JsonConvert.DeserializeObject(datajson, typeof(Match)) as Match;
            var exist = baozouMatchCollection.AsQueryable<Match>().SingleOrDefault(d => d.KanbisaiLink == m.KanbisaiLink);
            if (exist != null && !string.IsNullOrEmpty(m.BestVideoMobi)) {
                exist.BestVideoMobi = m.BestVideoMobi;
                baozouMatchCollection.Save(exist);
            }
        }
        #endregion

        #region 7M
        [HttpPost]
        public void List_7M_soccer_mobi(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            var data = JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<Match>)) as IEnumerable<Match>;
            foreach (var m in data) {
                var exist = QueryExitMatch(m);
                if (exist == null) {
                    if (AnalyzeCapString(m) && InjectTeamName(m)) {
                        m.SourceCode = task.Source;
                        baozouMatchCollection.Insert(m);
                    }
                } else {
                    exist.Point = m.Point;
                    exist.PointForGuest = m.PointForGuest;
                    if (m.Status > 0) exist.Status = m.Status;
                    exist.Quarter = m.Quarter;
                    exist.QuarterTime = String.Empty;
                    baozouMatchCollection.Save(exist);
                }
            }
        }

        [HttpPost]
        public void List_7m_soccer_schedule(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            var data = JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<Match>)) as IEnumerable<Match>;
            foreach (var m in data) {
                var exist = QueryExitMatch(m);
                if (exist == null) {
                    if (AnalyzeCapString(m) && InjectTeamName(m)) {
                        m.SourceCode = task.Source;
                        baozouMatchCollection.Insert(m);
                    }
                } else {
                    exist.Point = m.Point;
                    exist.PointForGuest = m.PointForGuest;
                    if (m.Status > 0) exist.Status = m.Status;
                    exist.Quarter = m.Quarter;
                    exist.QuarterTime = String.Empty;
                    baozouMatchCollection.Save(exist);
                }
            }
        }

        [HttpPost]
        public void List_7m_nba(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            var data = JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<Match>)) as IEnumerable<Match>;
            foreach (var m in data) {
                var exist = QueryExitMatch(m);
                if (exist == null) {
                    if (AnalyzeCapString(m) && InjectTeamName(m)) {
                        m.SourceCode = task.Source;
                        baozouMatchCollection.Insert(m);
                    }
                } else {
                    exist.Point = m.Point;
                    exist.PointForGuest = m.PointForGuest;
                    if (m.Status > 0) exist.Status = m.Status;
                    exist.Quarter = m.Quarter;
                    exist.QuarterTime = String.Empty;
                    baozouMatchCollection.Save(exist);
                }
            }
        }
        #endregion

        #region Azhibo
        private Match MatchByAzhiboTitle(Match m) {
            IMongoQuery queryTime = Query.And(
                Query<Match>.GT(e => e.Time, m.Time.Subtract(new TimeSpan(3, 0, 0))),
                Query<Match>.LT(e => e.Time, m.Time.AddHours(3))
            );
            Match match = null;
            if (m.CapString == "网球") {
                match = baozouMatchCollection.FindOne(Query.And(
                    queryTime,
                    Query<Match>.EQ(e => e.Type, (int)eMatchType.Tennis),
                    Query.Where(new BsonJavaScript("'" + m.Title + "'.indexOf(this.CapString) > -1"))
                ));
            } else if (m.CapString == "乒乓球") {
                match = baozouMatchCollection.FindOne(Query.And(
                    queryTime,
                    Query<Match>.EQ(e => e.Type, (int)eMatchType.Pingpong),
                    Query<Match>.EQ(e => e.Title, m.Title)
                ));
            } else {
                IMongoQuery queryCap;
                if (m.CapString == "足球友谊赛") {
                    queryCap = Query.And(queryTime, Query<Match>.EQ(e => e.Type, (int)eMatchType.Soccer));
                } else if (m.CapString == "篮球友谊赛") {
                    queryCap = Query.And(queryTime, Query<Match>.EQ(e => e.Type, (int)eMatchType.Nba));
                } else {
                    queryCap = Query.And(queryTime, Query<Match>.EQ(e => e.CapString, m.CapString));
                }
                match = baozouMatchCollection.FindOne(Query.And(
                    queryCap, Query<Match>.EQ(e => e.Title, m.Title)
                ));
                if (match == null) {
                    var matchs = baozouMatchCollection.Find(queryCap);
                    if (matchs.Count() == 0) return null;
                    foreach (var item in matchs.ToList()) {
                        if (m.Title.Contains(item.TeamNameChinese) || m.Title.Contains(item.TeamNameChineseForGuest)) {
                            match = item;
                            break;
                        }
                    }
                }
            }
            return match;
        }

        private bool AnalyzeAzhiboCapString(Match m) {
            if (m.CapString.Contains("乒乓球")) {
                m.Type = (int)eMatchType.Pingpong;
            }
            return (m.Type != 0);
        }

        [ValidateInput(false)]
        [HttpPost]
        public void Addition_azhibo(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            var data = JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<Match>)) as IEnumerable<Match>;
            foreach (Match m in data) {
                Match match = MatchByAzhiboTitle(m);
                if (match != null) {
                    match.Title = m.Title;
                    if (!string.IsNullOrEmpty(m.LiveText))
                        match.LiveText = new WebRequestRobot().Get302Location(m.LiveText);
                    InjectLiveVideos(match, m.LiveVideos, false);
                    baozouMatchCollection.Save(match);
                } else {
                    if (AnalyzeAzhiboCapString(m)) {
                        match = m;
                        match.SourceCode = task.Source;
                        if (!string.IsNullOrEmpty(m.LiveText))
                            match.LiveText = new WebRequestRobot().Get302Location(m.LiveText);
                        InjectLiveVideos(match, m.LiveVideos, false);
                        baozouMatchCollection.Insert(match);
                    };
                }
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public void Addition_azhibo_mobi(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            var data = JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<Match>)) as IEnumerable<Match>;
            foreach (Match m in data) {
                Match match = MatchByAzhiboTitle(m);
                if (match == null) continue;
                match.Title = m.Title;
                match.LiveTextForMobile = m.LiveTextForMobile;
                InjectLiveVideos(match, m.LiveVideosForMobile, true);
                baozouMatchCollection.Save(match);
            }
        }

        private void InjectLiveVideos(Match match, IEnumerable<Link> links, bool isMobi) {
            if (!match.LockSpider) {
                IEnumerable<string> liveString = links.Select(d => d.Name);
                if (isMobi) {
                    var lvsMobi = from d in baozouLiveCollection.AsQueryable()
                                  where d.LinkForMobile != null && d.AliasForMobile != null
                                  orderby d.Rank descending
                                  select d;
                    foreach (var lv in lvsMobi) lv.InjectKanbisai(match.KanbisaiLink);
                    match.LiveVideosForMobile = lvsMobi.Where(d => d.AliasForMobile.ContainsAny(liveString)).Select(x => new Link {
                        Name = x.Name,
                        Url = x.LinkForMobile
                    });
                    match.LiveVideosForAndroid = lvsMobi.Where(d => d.AliasForMobile.ContainsAny(liveString)).Select(x => new Link {
                        Name = x.Name,
                        Url = string.IsNullOrEmpty(x.LinkForAndroid) ? x.LinkForMobile : x.LinkForAndroid
                    });
                } else {
                    match.LiveVideos = from d in baozouLiveCollection.AsQueryable<LiveVideo>()
                                       where d.Alias.ContainsAny(liveString) //http://goo.gl/WyLqec 1.5+版本支持
                                       orderby d.Rank descending
                                       select new Link {
                                           Name = d.Name + (d.WithClient ? "φ" : ""),
                                           Url = (d.Name == "腾讯看比赛" ? match.KanbisaiLink : d.Link)
                                       };
                }
            }
        }
        #endregion

        #region Zhiboba
        [ValidateInput(false)]
        [HttpPost]
        public void Ids_zhiboba(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            var taskModel = taskModelCollection.AsQueryable<TaskModel>().Single(d => d.Id == task.TaskModelId);
            var data = JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<Match>)) as IEnumerable<Match>;
            foreach (Match m in data) {
                Match match = MatchByAzhiboTitle(m);
                if (match == null) continue;
                TaskQueue.tasks.Add(new SpiderTask {
                    Id = Guid.NewGuid(),
                    Site = taskModel.Site,
                    Source = taskModel.SourceCode,
                    CommandType = eCommandType.Addition.ToString(),
                    Url = m.BestVideo,
                    ArticleType = eArticleType.BaozouMatch.ToString(),
                    Error = match.Id,
                    PostSourceName = true
                });
            }
            TaskQueue.masterhub.Clients.Group("broad").broadcastRanderTask(TaskQueue.tasks);
        }

        [ValidateInput(false)]
        [HttpPost]
        public void Addition_zhiboba(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            var data = JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<string>)) as IEnumerable<string>;
            foreach (string url in data) {
                TaskQueue.tasks.Add(new SpiderTask {
                    Id = Guid.NewGuid(),
                    Site = "zhiboba",
                    Source = "zhiboba",
                    CommandType = eCommandType.One.ToString(),
                    Url = url,
                    ArticleType = eArticleType.BaozouMatch.ToString(),
                    Error = task.Error,
                    Delay = 2,
                    PostSourceName = true
                });
                TaskQueue.tasks.Add(new SpiderTask {
                    Id = Guid.NewGuid(),
                    Site = "zhiboba",
                    Source = "zhiboba",
                    CommandType = eCommandType.One.ToString(),
                    Url = url,
                    ArticleType = eArticleType.BaozouMatch.ToString(),
                    Error = task.Error,
                    Delay = 2,
                    IsMobile = true,
                    PostSourceName = true
                });
            }
            TaskQueue.masterhub.Clients.Group("broad").broadcastRanderTask(TaskQueue.tasks);
        }

        [ValidateInput(false)]
        [HttpPost]
        public void One_zhiboba(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            var data = JsonConvert.DeserializeObject(datajson, typeof(Match)) as Match;
            var match = baozouMatchCollection.FindOneByIdAs<Match>(new MongoDB.Bson.ObjectId(task.Error));
            if (match != null && !string.IsNullOrEmpty(data.BestVideo)) {
                if (match.Recording == null)
                    match.Recording = new List<Link>();
                if (match.Recording.Any(d => d.Name == data.CapString)) {
                    match.Recording.First(d => d.Name == data.CapString).Url = data.BestVideo;
                } else {
                    match.Recording.Add(new Link {
                        Name = data.CapString,
                        Url = data.BestVideo
                    });
                }
                baozouMatchCollection.Save(match);
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public void One_zhiboba_mobi(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            var data = JsonConvert.DeserializeObject(datajson, typeof(Match)) as Match;
            var match = baozouMatchCollection.FindOneByIdAs<Match>(new MongoDB.Bson.ObjectId(task.Error));
            if (match != null && !string.IsNullOrEmpty(data.BestVideoMobi)) {
                if (match.RecordingMobi == null)
                    match.RecordingMobi = new List<Link>();
                if (match.RecordingMobi.Any(d => d.Name == data.CapString)) {
                    match.RecordingMobi.First(d => d.Name == data.CapString).Url = data.BestVideoMobi;
                } else {
                    match.RecordingMobi.Add(new Link {
                        Name = data.CapString,
                        Url = data.BestVideoMobi
                    });
                }
                baozouMatchCollection.Save(match);
            }
        }
        #endregion
    }
}
