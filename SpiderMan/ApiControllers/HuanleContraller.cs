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
        public IEnumerable<Huanle> GetList(string boxer, int pager) {
            boxer = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(boxer);
            var result = from d in repos.HuanleRepo.Collection.AsQueryable<Huanle>()
                         where d.Status == (int)Enum.Parse(typeof(eArticleStatus), boxer)
                         select d;
            if (pager == 0)
                return result.Take(30);
            else
                return result.Skip(30 * pager).Take(30);
        }

        // PUT api/huanle/51c07bbec32d92328066b256
        [HandleErrorForJsonAttribute]
        public void Put(Huanle value) {
            if (value.Comments.Count == 1 && value.Comments.First().Id == null)
                value.Comments = null; //ToDo: Json Convert issus, i don't know why.
            var updateResult = repos.HuanleRepo.Collection.Update(
                Query<Huanle>.EQ(p => p.Id, value.Id),
                Update<Huanle>.Replace(value),
                new MongoUpdateOptions {
                    WriteConcern = WriteConcern.Acknowledged //ToDo: i don't know what's means.
                });

            if (updateResult.DocumentsAffected == 0) {
                throw new Exception("No one updateed!");
            }
        }

        // DELETE api/huanle/51c07bbec32d92328066b256
        public void Delete(string id) {
            repos.HuanleRepo.Delete(id);
        }

    }
}
