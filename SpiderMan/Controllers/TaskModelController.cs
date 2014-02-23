using SpiderMan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using sharp_net.Mvc;
using sharp_net.Mongo;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using sharp_net.Repositories;
using SpiderMan.Entity;

namespace SpiderMan.Controllers {
    [Authorize]
    public class TaskModelController : Controller {
        private readonly MongoCollection<TaskModel> taskModelCollection;
        private readonly MongoCollection<Site> siteCollection;
        public TaskModelController(MongoRepo<TaskModel> taskmodel_repo, MongoRepo<Site> site_repo) {
            this.taskModelCollection = taskmodel_repo.Collection;
            this.siteCollection = site_repo.Collection;
        }

        public ActionResult Index() {
            var models = taskModelCollection.AsQueryable<TaskModel>().Where(d => d.Interval > 0);
            return View(models);
        }

        public ActionResult Create() {
            ViewBag.SiteList = from site in siteCollection.FindAll()
                               select new SelectListItem() {
                                   Text = site.Name,
                                   Value = site.Name
                               };
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaskModel model, string urlParams) {
            model.UrlParams = urlParams.Split('\n').ToList();
            if (!ModelState.IsValid) {
                ModelState.AddModelError("", "表单验证失败。");
                return View(model);
            }
            taskModelCollection.Insert(model);
            TaskQueue.Instance.ModelTimerReBuild();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(string id) {
            var model = taskModelCollection.FindOneByIdAs<TaskModel>(new ObjectId(id));
            ViewBag.SiteList = from site in siteCollection.FindAll()
                               select new SelectListItem() {
                                   Text = site.Name,
                                   Value = site.Name
                               };
            if (model.UrlParams != null)
                ViewBag.UrlParams = string.Join("\n", model.UrlParams);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TaskModel model, string urlParams) {
            model.UrlParams = urlParams.Split('\n').ToList();
            if (!ModelState.IsValid) {
                ModelState.AddModelError("", "表单验证失败。");
                return View(model);
            }
            taskModelCollection.Save(model);
            TaskQueue.Instance.ModelTimerReBuild();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id) {
            var site = taskModelCollection.FindOneByIdAs<TaskModel>(new ObjectId(id));
            return View(site);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id) {
            taskModelCollection.Remove(Query<TaskModel>.EQ(d => d.Id, id));
            TaskQueue.Instance.ModelTimerReBuild();
            return RedirectToAction("Index");
        }
    }
}
