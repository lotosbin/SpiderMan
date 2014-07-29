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

    public class ShudongController : Controller {
        private readonly MongoCollection<TaskModel> taskModelCollection;
        private readonly MongoCollection<Shudong> shudongCollection;
        public ShudongController(IMongoRepo<TaskModel> taskmodel_repo,
            IMongoRepo<Shudong> shudong_repo) {
            this.taskModelCollection = taskmodel_repo.Collection;
            this.shudongCollection = shudong_repo.Collection;
        }

        [HttpPost]
        public void Ids(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            var taskModel = taskModelCollection.AsQueryable<TaskModel>().Single(d => d.Id == task.TaskModelId);
            var data = JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<string>)) as IEnumerable<string>;
            foreach (string id in data) {
                if (!shudongCollection.AsQueryable<Shudong>().Any(d => d.ProviderId == id & d.SourceCode == taskModel.SourceCode)) {
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
        public void One(string taskjson, string datajson) {
            var task = JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask)) as SpiderTask;
            datajson = FilterConfig.htmlFilter.Filter(datajson, true);
            var data = JsonConvert.DeserializeObject(datajson, typeof(Shudong)) as Shudong;
            var exist = shudongCollection.AsQueryable<Shudong>().SingleOrDefault(d => d.ProviderId == data.ProviderId & d.SourceCode == task.Source);
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

    }
}
