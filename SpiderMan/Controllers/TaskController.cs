using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using sharp_net.Mvc;
using SpiderMan.Help;
using SpiderMan.Models;
using SpiderMan.Respository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SpiderMan.Controllers {

    public class TaskController : Controller {
        private readonly Respositorys repos;
        public TaskController(Respositorys _repos) {
            this.repos = _repos;
        }

        public ActionResult Index() {
            ViewBag.TaskModel = repos.TaskModelRepo.Collection.FindAll();
            return View();
        }

        static readonly MatchEvaluator replacer = m => ((char)int.Parse(m.Groups[1].Value, NumberStyles.AllowHexSpecifier)).ToString();
        // 关于在web api使用FormDataCollection http://goo.gl/PjJGf
        [HttpPost]
        public void PostData(string taskjson, string datajson) {
            SpiderTask task = (SpiderTask)JsonConvert.DeserializeObject(taskjson, typeof(SpiderTask));
            switch (task.ArticleType) {
                case eArticleType.Huanle:
                    if (task.CommandType == eCommandType.List) {
                        //因为Mongodb Respository's Entity有DataContract特性，所以无法被json.net序列化，只能使用DataContractJsonSerializer
                        //DataContractJsonSerializer 不能忽略大小写
                        var data = JsonHelper.JsonDeserialize<IEnumerable<Huanle>>(datajson);
                        foreach (var item in data) item.Init(task);
                        repos.HuanleRepo.Add(data);
                    } else {
                        var data = JsonHelper.JsonDeserialize<Huanle>(datajson);
                        data.Init(task);
                        repos.HuanleRepo.Add(data);
                    }
                    break;
                case eArticleType.Shudong:
                    if (task.CommandType == eCommandType.List) {
                        var data = JsonHelper.JsonDeserialize<IEnumerable<Shudong>>(datajson);
                        foreach (var item in data) item.Init(task);
                        repos.ShudongRepo.Add(data);
                    } else {
                        var data = JsonHelper.JsonDeserialize<Shudong>(datajson);
                        data.Init(task);
                        repos.ShudongRepo.Add(data);
                    }
                    break;
                case eArticleType.Dianbo:
                    if (task.CommandType == eCommandType.List) {
                        var data = JsonHelper.JsonDeserialize<IEnumerable<Dianbo>>(datajson);
                        foreach (var item in data) item.Init(task);
                        repos.DianboRepo.Add(data);
                    } else {
                        var data = JsonHelper.JsonDeserialize<Dianbo>(datajson);
                        data.Init(task);
                        repos.DianboRepo.Add(data);
                    }
                    break;
                default:
                    break;
            }
        }

    }
}
