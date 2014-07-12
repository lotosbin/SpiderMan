using SpiderMan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using sharp_net.Mongo;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using SpiderMan.Entity;

namespace SpiderMan.Controllers {
    [Authorize]
    public class AssistController : Controller {
        private readonly MongoCollection<TaskModel> taskModelCollection;
        private readonly MongoCollection<Site> siteCollection;
        public AssistController(MongoRepo<TaskModel> taskmodel_repo, MongoRepo<Site> site_repo) {
            this.taskModelCollection = taskmodel_repo.Collection;
            this.siteCollection = site_repo.Collection;
        }

        public ActionResult Grabscript() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Grabscript(string sourceCode, int commandType, bool isMobi, string content) {
            TaskQueue.masterhub.UpdateScript(sourceCode + "_" + ((eCommandType)commandType).ToString().ToLower() + (isMobi ? "_mobi" : "") + ".js", content);
            return View();
        }

        private void SiteList() {
            ViewBag.SiteList = from site in siteCollection.FindAll()
                               select new SelectListItem() {
                                   Text = site.Name,
                                   Value = site.Name
                               };
        }

        public ActionResult Creattask() {
            SiteList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Creattask(TaskModel model, string urlParams) {
            model.UrlParams = urlParams.Split('\n').Select(d => d.Trim()).ToList();
            TaskQueue.tasks.AddRange(model.GenerateSpiderTask());
            if (TaskQueue.masterhub != null)
                TaskQueue.masterhub.BroadcastRanderTask();
            SiteList();
            return View();
        }

    }
}
