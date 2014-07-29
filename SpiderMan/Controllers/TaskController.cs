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

        public TaskController(IMongoRepo<TaskModel> taskmodel_repo, IMongoRepo<Shudong> shudong_repo) {
            this.taskModelCollection = taskmodel_repo.Collection;
        }

        public ActionResult Index() {
            ViewBag.TaskModel = taskModelCollection.AsQueryable<TaskModel>().Where(d => d.Act == (int)eAct.Normal && d.Interval > 0);
            return View();
        }

    }
}
