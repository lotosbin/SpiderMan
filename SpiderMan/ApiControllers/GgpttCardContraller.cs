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
using SpiderMan.Models;
using sharp_net.Mongo;
using MongoDB.Bson;
using System.Threading;

namespace SpiderMan.ApiControllers {
    public class GgpttCardController : ApiController {

        private readonly MongoCollection<GgpttCard> huanleCollection;
        public GgpttCardController(MongoRepo<GgpttCard> huanle_repos) {
            this.huanleCollection = huanle_repos.Collection;
        }

        // GET api/huanle/51c07bbec32d92328066b256
        public GgpttCard Get(string id) {
            var result = huanleCollection.FindOneByIdAs<GgpttCard>(new ObjectId(id));
            return result;
        }

        // GET api/huanle/verifying
        public IEnumerable<GgpttCard> GetList(string boxer, int pager) {
            boxer = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(boxer);
            var result = from d in huanleCollection.AsQueryable<GgpttCard>()
                         where d.Status == (int)Enum.Parse(typeof(eArticleStatus), boxer)
                         orderby d.GrabDate
                         select d;
            return result.Skip(30 * pager).Take(30);
        }

        // PUT api/huanle/51c07bbec32d92328066b256
        [HandleErrorForJsonAttribute]
        public void Put(GgpttCard value) {
            //if (value.Comments.Count == 1 && value.Comments.First().Id == null)
            //    value.Comments = null; //ToDo: Json Convert issus, i don't know why.
            var updateResult = huanleCollection.Update(
                Query<GgpttCard>.EQ(p => p.Id, value.Id),
                Update<GgpttCard>.Replace(value),
                new MongoUpdateOptions {
                    WriteConcern = WriteConcern.Acknowledged //ToDo: i don't know what's means.
                });

            if (updateResult.DocumentsAffected == 0) {
                throw new Exception("No one updateed!");
            }
        }

        // DELETE api/huanle/51c07bbec32d92328066b256
        public void Delete(string id) {
            huanleCollection.Remove(Query<GgpttCard>.EQ(d => d.Id, id));
        }

    }
}