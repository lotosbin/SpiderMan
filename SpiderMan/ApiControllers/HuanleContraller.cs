using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using sharp_net.Repositories;
using SpiderMan.Models;
using SpiderMan.Respository;
using MongoRepository;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using sharp_net.Mvc;

namespace SpiderMan.ApiControllers {
    public class HuanleContraller : ApiController {
        private readonly ArticleRespository repo;
        public HuanleContraller(ArticleRespository _repo) {
            this.repo = _repo;
        }

        // GET api/values/a56asdf65as5
        public Huanle Get(string id) {
            var result = repo.HuanleRepo.GetById(id);
            return result;
        }

        // GET api/values/verifying
        [ActionName("Get")]
        public IEnumerable<Huanle> GetList(string boxer) {
            int articleStatus = (int)Enum.Parse(typeof(eArticleStatus), boxer);
            var result = from d in repo.HuanleRepo.Collection.AsQueryable<Huanle>()
                         where d.Status == articleStatus
                         select d;
            return result;
        }

        // PUT api/values/5
        [HandleErrorForJsonAttribute]
        public void Put(string id, Huanle value) {
            Huanle item = repo.HuanleRepo.GetById(id);
            var updateResult = repo.HuanleRepo.Collection.Update(
                Query<Huanle>.EQ(p => p.Id, id),
                Update<Huanle>.Replace(value),
                new MongoUpdateOptions {
                    WriteConcern = WriteConcern.Acknowledged
                });

            if (updateResult.DocumentsAffected == 0) {
                throw new Exception("No one updateed!");
            }
        }

        // DELETE api/values/5
        public void Delete(string id) {
            repo.HuanleRepo.Delete(id);
        }

    }
}
