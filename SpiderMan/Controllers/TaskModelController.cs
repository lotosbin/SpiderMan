using MongoRepository;
using SpiderMan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using sharp_net.Mvc;
using SpiderMan.Respository;

namespace SpiderMan.Controllers {
    public class TaskModelController : Controller {
        private readonly Respositorys repo;
        public TaskModelController(Respositorys _repo) {
            this.repo = _repo;
        }

        public ActionResult Index() {
            var models = repo.TaskModelRepo.Collection.FindAll();
            return View(models);
        }

        public ActionResult Create() {
            ViewBag.SiteList = from site in repo.SiteRepo.Collection.FindAll()
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
            repo.TaskModelRepo.Add(model);
            TaskQueue.Instance.ModelTimerReBuild();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(string id) {
            var model = repo.TaskModelRepo.GetById(id);
            ViewBag.SiteList = from site in repo.SiteRepo.Collection.FindAll()
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
            repo.TaskModelRepo.Update(model);
            TaskQueue.Instance.ModelTimerReBuild();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id) {
            var site = repo.TaskModelRepo.GetById(id);
            return View(site);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id) {
            repo.TaskModelRepo.Delete(id);
            TaskQueue.Instance.ModelTimerReBuild();
            return RedirectToAction("Index");
        }
    }
}
