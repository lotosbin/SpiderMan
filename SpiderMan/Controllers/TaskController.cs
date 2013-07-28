using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using sharp_net.Infrastructure;
using sharp_net.Mvc;
using SpiderMan.Help;
using SpiderMan.Models;
using sharp_net.Mongo;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SpiderMan.Controllers {

    public class TaskController : Controller {
        private readonly MongoCollection<TaskModel> taskModelCollection;
        private readonly MongoCollection<Huanle> huanleCollection;
        private readonly MongoCollection<Dianbo> dianboCollection;
        private readonly MongoCollection<Shudong> shudongCollection;
        public TaskController(IMongoRepo<TaskModel> taskmodel_repo, IMongoRepo<Huanle> huanle_repo, IMongoRepo<Dianbo> dianbo_repo, IMongoRepo<Shudong> shudong_repo) {
            this.taskModelCollection = taskmodel_repo.Collection;
            this.huanleCollection = huanle_repo.Collection;
            this.dianboCollection = dianbo_repo.Collection;
            this.shudongCollection = shudong_repo.Collection;
        }

        public ActionResult Index() {
            ViewBag.TaskModel = taskModelCollection.FindAll();
            return View();
        }
        
        // 关于在web api使用FormDataCollection http://goo.gl/PjJGf
        // 不要使用gbk编码提交，很容易产生字符串错误从而无法提交。
        [HttpPost]
        [ValidateInput(false)]
        public void PostData(string taskjson, string datajson) {
            SpiderTask task = (SpiderTask)JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask));
            datajson = FilterConfig.htmlFilter.Filter(datajson);

            switch (task.ArticleType) {
                case eArticleType.Huanle:
                    if (task.CommandType == eCommandType.List) {
                        //Mongodb Respository's Entity有DataContract特性，所以无法被json.net序列化，只能使用DataContractJsonSerializer。DataContractJsonSerializer 不能忽略大小写
                        //var data = JsonHelper.JsonDeserialize<IEnumerable<Huanle>>(datajson);
                        var data = JsonConvert.DeserializeObject(datajson, typeof(IEnumerable<Huanle>)) as IEnumerable<Huanle>;
                        foreach (var item in data) item.Init(task);
                        huanleCollection.InsertBatch(data);
                    } else {
                        var data = JsonHelper.JsonDeserialize<Huanle>(datajson);
                        data.Init(task);
                        huanleCollection.Insert(data);
                    }
                    break;
                case eArticleType.Shudong:
                    if (task.CommandType == eCommandType.List) {
                        var data = JsonHelper.JsonDeserialize<IEnumerable<Shudong>>(datajson);
                        foreach (var item in data) item.Init(task);
                        shudongCollection.InsertBatch(data);
                    } else {
                        var data = JsonHelper.JsonDeserialize<Shudong>(datajson);
                        data.Init(task);
                        shudongCollection.Insert(data);
                    }
                    break;
                case eArticleType.Dianbo:
                    if (task.CommandType == eCommandType.List) {
                        var data = JsonHelper.JsonDeserialize<IEnumerable<Dianbo>>(datajson);
                        foreach (var item in data) item.Init(task);
                        dianboCollection.InsertBatch(data);
                    } else {
                        var data = JsonHelper.JsonDeserialize<Dianbo>(datajson);
                        data.Init(task);
                        dianboCollection.Insert(data);
                    }
                    break;
                default:
                    break;
            }
        }

    }
}
