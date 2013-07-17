using MongoRepository;
using SpiderMan.Models;
using SpiderMan.Respository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpiderMan.Controllers {
    public class SiteController : Controller {
        private readonly Respositorys repo;
        public SiteController(Respositorys _repo) {
            this.repo = _repo;
        }

        public ActionResult Index() {
            var sites = repo.SiteRepo.Collection.FindAll();
            return View(sites);
        }

        public ActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Site model) {
            if (!ModelState.IsValid) {
                ModelState.AddModelError("", "表单验证失败。");
                return View(model);
            }
            repo.SiteRepo.Add(model);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(string id) {
            var site = repo.SiteRepo.GetById(id);
            return View(site);
        }

        [HttpPost]
        public ActionResult Edit(Site model) {
            if (!ModelState.IsValid) {
                ModelState.AddModelError("", "表单验证失败。");
                return View(model);
            }
            repo.SiteRepo.Update(model);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id) {
            var site = repo.SiteRepo.GetById(id);
            return View(site);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id) {
            repo.SiteRepo.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
