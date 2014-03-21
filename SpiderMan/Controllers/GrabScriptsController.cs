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
    public class GrabScriptsController : Controller {

        public ActionResult Index() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string sourceCode, int commandType, string content) {
            TaskQueue.masterhub.UpdateScript(sourceCode + "_" + ((eCommandType)commandType).ToString().ToLower() + ".js", content);
            return View();
        }

    }
}
