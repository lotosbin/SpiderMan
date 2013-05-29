using SpiderMan.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoRepository;

namespace SpiderMan.Respository {
    public class TaskRespository : MongoRepository<Task> {
        public IEnumerable<Task> GetTasks(int limit, int skip) {
            var gamesCursor = this.Collection.FindAllAs<Task>()
                .SetSortOrder(SortBy<Task>.Descending(g => g.QueueId))
                .SetLimit(limit)
                .SetSkip(skip)
                .SetFields(Fields<Task>.Include(g => g.QueueId, g => g.Status, g => g.Url, g => g.Type));
            return gamesCursor;
        }
    }
}