using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using MongoRepository;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using sharp_net.Repositories;
using sharp_net.Mvc;
using SpiderMan.Models;
using SpiderMan.Respository;
using System.Threading;

namespace SpiderMan.ApiControllers {
    public class HuanleController : ApiController {

        private readonly Respositorys repos;
        public HuanleController(Respositorys _repos) {
            this.repos = _repos;
        }

        // GET api/huanle/51c07bbec32d92328066b256
        public Huanle Get(string id) {
            var result = repos.HuanleRepo.GetById(id);
            return result;
        }

        // GET api/huanle/verifying
        [ActionName("Get")]
        public IEnumerable<Huanle> GetList(string boxer) {
            boxer = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(boxer);
            int articleStatus = (int)Enum.Parse(typeof(eArticleStatus), boxer);
            var result = from d in repos.HuanleRepo.Collection.AsQueryable<Huanle>()
                         where d.Status == articleStatus
                         select d;
            return result;
        }

        // PUT api/huanle/5
        [HandleErrorForJsonAttribute]
        public void Put(string id, Huanle value) {
            Huanle item = repos.HuanleRepo.GetById(id);
            var updateResult = repos.HuanleRepo.Collection.Update(
                Query<Huanle>.EQ(p => p.Id, id),
                Update<Huanle>.Replace(value),
                new MongoUpdateOptions {
                    WriteConcern = WriteConcern.Acknowledged
                });

            if (updateResult.DocumentsAffected == 0) {
                throw new Exception("No one updateed!");
            }
        }

        // DELETE api/huanle/5
        public void Delete(string id) {
            repos.HuanleRepo.Delete(id);
        }

    }
}
