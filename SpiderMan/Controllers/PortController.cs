using MongoDB.Driver;
using MongoDB.Driver.Linq;
using sharp_net.Mongo;
using sharp_net.Repositories;
using SpiderMan.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SpiderMan.Controllers {
    public class PortController : ApiController {
        private readonly MongoCollection<TaskModel> taskModelCollection;

        public PortController(IMongoRepo<TaskModel> taskmodel_repo) {
            this.taskModelCollection = taskmodel_repo.Collection;
        }

        // GET api/<controller>
        public IEnumerable<string> Get() {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id) {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string taskModelId) {
            var model = taskModelCollection.FindOneById(taskModelId);
            var newTask = new SpiderTask {
                Id = Guid.NewGuid(),
                BirthTime = DateTime.Now,
                TaskModelId = model.Id,
                Site = model.Site,
                Source = model.SourceCode,
                CommandType = ((eCommandType)model.CommandType).ToString(),
                Url = model.Url,
                ArticleType = ((eArticleType)model.ArticleType).ToString()
            };
            TaskQueue.tasks.Add(newTask);
            if (TaskQueue.masterhub != null) {
                TaskQueue.masterhub.BroadcastRanderTask();
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value) {
        }

        // DELETE api/<controller>/5
        public void Delete(int id) {
        }
    }
}