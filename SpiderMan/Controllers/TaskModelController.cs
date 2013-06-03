using MongoRepository;
using SpiderMan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using sharp_net.Mvc;

namespace SpiderMan.Controllers {
    public class TaskModelController : Controller {
        private readonly MongoRepository<TaskModel> repo;
        private readonly MongoRepository<Site> repo_site;
        public TaskModelController(MongoRepository<TaskModel> _repo, MongoRepository<Site> _repo_site) {
            this.repo = _repo;
            this.repo_site = _repo_site;
        }

        public ActionResult Index() {
            var models = repo.All();
            return View(models);
        }

        public ActionResult Create() {
            ViewBag.SiteList = from site in repo_site.All()
                               select new SelectListItem() {
                                   Text = site.Name,
                                   Value = site.Name
                               };
            return View();
        }

        [HttpPost]
        public ActionResult Create(TaskModel model) {
            if (!ModelState.IsValid) {
                ModelState.AddModelError("", "表单验证失败。");
                return View(model);
            }
            repo.Add(model);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(string id) {
            var model = repo.GetById(id);
            ViewBag.SiteList = from site in repo_site.All()
                               select new SelectListItem() {
                                   Text = site.Name,
                                   Value = site.Name
                               };
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(TaskModel model) {
            if (!ModelState.IsValid) {
                ModelState.AddModelError("", "表单验证失败。");
                return View(model);
            }
            repo.Update(model);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id) {
            var site = repo.GetById(id);
            return View(site);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id) {
            repo.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
