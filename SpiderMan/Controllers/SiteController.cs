using MongoRepository;
using SpiderMan.Filters;
using SpiderMan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpiderMan.Controllers {
    public class SiteController : Controller {
        private readonly MongoRepository<Site> repo;
        public SiteController(MongoRepository<Site> _repo) {
            this.repo = _repo;
        }

        //
        // GET: /Site/

        public ActionResult Index() {
            var sites = repo.All();
            return View(sites);
        }

        //
        // GET: /Site/Details/5

        public ActionResult Details(string id) {
            var site = repo.GetById(id);
            return View(site);
        }

        //
        // GET: /Site/Create

        public ActionResult Create() {
            return View();
        }

        //
        // POST: /Site/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Site model) {
            if (!ModelState.IsValid) {
                ModelState.AddModelError("", "表单验证失败。");
                return View(model);
            }
            repo.Add(model);
            return RedirectToAction("Index");
        }

        //
        // GET: /Site/Edit/5

        public ActionResult Edit(string id) {
            var site = repo.GetById(id);
            return View(site);
        }

        //
        // POST: /Site/Edit/5

        [HttpPost]
        public ActionResult Edit(Site model) {
            if (!ModelState.IsValid) {
                ModelState.AddModelError("", "表单验证失败。");
                return View(model);
            }
            repo.Update(model);
            return RedirectToAction("Index");
        }

        //
        // GET: /Site/Delete/5
        public ActionResult Delete(string id) {
            var site = repo.GetById(id);
            return View(site);
        }

        //
        // POST: /Site/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id) {
            repo.Delete(id);
            return Json(new { result = true });
        }
    }
}
