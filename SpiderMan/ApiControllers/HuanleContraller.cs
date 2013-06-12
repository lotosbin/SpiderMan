using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using sharp_net.Repositories;
using Newtonsoft.Json;
using MongoRepository;
using SpiderMan.Models;
using SpiderMan.Respository;

namespace SpiderMan.ApiControllers {
    public class HuanleContraller : ApiController {
        private readonly ArticleRespository repo;
        public HuanleContraller(ArticleRespository _repo) {
            this.repo = _repo;
        }

        public Huanle Get(string id) {
            var result = repo.HuanleRepo.GetById(id);
            return result;
        }

        public IEnumerable<Huanle> Get(string boxer) {
            var articleStatus = Enum.Parse(typeof(eArticleStatus), boxer);
            var result = repo.HuanleRepo.Collection.Find(d => d.ArticleStatusEnum == articleStatus);
            return result;
        }

    }
}
