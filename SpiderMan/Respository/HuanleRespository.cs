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
    public class HuanleRespository : ArticleRespository {
        public IEnumerable<Huanle> GetArticles(int limit, int skip) {
            var qiubaisCursor = HuanleRepo.Collection.FindAllAs<Huanle>()
                .SetSortOrder(SortBy<Huanle>.Descending(g => g.Amount))
                .SetLimit(limit)
                .SetSkip(skip)
                .SetFields(Fields<Huanle>.Include(g => g.Content, g => g.ThumbUps, g => g.ThumbDowns, g => g.Comments));
            return qiubaisCursor;
        }

        public void AddComment(string articleId, Comment comment) {
            var updateResult = HuanleRepo.Collection.Update(
                    Query<Huanle>.EQ(p => p.Id, articleId),
                    Update<Huanle>.Push(p => p.Comments, comment),
                    new MongoUpdateOptions {
                        WriteConcern = WriteConcern.Acknowledged
                    });

            if (updateResult.DocumentsAffected == 0) {
                //// Something went wrong

            }
        }

    }
}