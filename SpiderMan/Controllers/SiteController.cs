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

namespace SpiderMan.Controllers {
    public class SiteController : Controller {
        private readonly MongoCollection<Site> siteCollection;
        public SiteController(IMongoRepo<Site> site_repo) {
            this.siteCollection = site_repo.Collection;
        }

        public ActionResult Index() {
            var sites = siteCollection.FindAll();
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
            siteCollection.Insert(model);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(string id) {
            var site = siteCollection.FindOneByIdAs<Site>(new ObjectId(id));
            return View(site);
        }

        [HttpPost]
        public ActionResult Edit(Site model) {
            if (!ModelState.IsValid) {
                ModelState.AddModelError("", "表单验证失败。");
                return View(model);
            }
            siteCollection.Save(model);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id) {
            var site = siteCollection.FindOneByIdAs<Site>(new ObjectId(id));
            return View(site);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id) {
            siteCollection.Remove(Query<Site>.EQ(d => d.Id, id));
            return RedirectToAction("Index");
        }
    }
}
