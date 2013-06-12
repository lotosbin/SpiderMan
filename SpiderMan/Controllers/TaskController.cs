using MongoRepository;
using SpiderMan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpiderMan.Controllers {
    public class TaskController : Controller {
        private readonly MongoRepository<TaskModel> repo;
        public TaskController(MongoRepository<TaskModel> _repo) {
            this.repo = _repo;
        }

        public ActionResult Index() {
            ViewBag.TaskModel = repo.All();
            return View();
        }

    }
}
