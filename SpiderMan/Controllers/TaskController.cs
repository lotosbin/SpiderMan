using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpiderMan.Models;
using SpiderMan.Respository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SpiderMan.Controllers {

    public class TaskController : Controller {
        private readonly Respositorys repo;
        public TaskController(Respositorys _repo) {
            this.repo = _repo;
        }

        public ActionResult Index() {
            ViewBag.TaskModel = repo.TaskModelRepo.Collection.FindAll();
            return View();
        }

    }
}
