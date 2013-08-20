using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using sharp_net.Repositories;
using sharp_net.Mvc;
using sharp_net.Mongo;
using MongoDB.Bson;
using System.Threading;
using SpiderMan.Entity;

namespace SpiderMan.ApiControllers {
    public class GgpttCardController : ApiController {

        private readonly MongoCollection<GgpttCard> Collection;
        public GgpttCardController(MongoRepo<GgpttCard> huanle_repos) {
            this.Collection = huanle_repos.Collection;
        }

        // GET api/GgpttCard/51c07bbec32d92328066b256
        public GgpttCard Get(string id) {
            var result = Collection.FindOneByIdAs<GgpttCard>(new ObjectId(id));
            return result;
        }

        // GET api/GgpttCard/verifying
        public IEnumerable<GgpttCard> GetList(string boxer, int pager) {
            boxer = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(boxer);
            var result = from d in Collection.AsQueryable<GgpttCard>()
                         where d.Status == (int)Enum.Parse(typeof(eArticleStatus), boxer)
                         orderby d.Grade
                         select d;
            return result.Skip(30 * pager).Take(30);
        }

        // PUT api/GgpttCard/51c07bbec32d92328066b256
        [HandleErrorForJsonAttribute]
        public void Put(GgpttCard value) {
            //if (value.Comments.Count == 1 && value.Comments.First().Id == null)
            //    value.Comments = null; //ToDo: Json Convert issus, i don't know why.
            var updateResult = Collection.Update(
                Query<GgpttCard>.EQ(p => p.Id, value.Id),
                Update<GgpttCard>.Replace(value),
                new MongoUpdateOptions {
                    WriteConcern = WriteConcern.Acknowledged //ToDo: i don't know what's means.
                });

            if (updateResult.DocumentsAffected == 0) {
                throw new Exception("No one updateed!");
            }
        }

        // DELETE api/GgpttCard/51c07bbec32d92328066b256
        public void Delete(string id) {
            Collection.Remove(Query<GgpttCard>.EQ(d => d.Id, id));
        }

    }
}