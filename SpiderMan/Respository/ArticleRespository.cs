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
    public class ArticleRespository : MongoRepository<Article> {
        public IEnumerable<Article> GetArticles(int limit, int skip) {
            var qiubaisCursor = this.Collection.FindAllAs<Article>()
                .SetSortOrder(SortBy<Article>.Descending(g => g.Amount))
                .SetLimit(limit)
                .SetSkip(skip)
                .SetFields(Fields<Article>.Include(g => g.Content, g => g.ThumbUps, g => g.ThumbDowns, g => g.Comments));
            return qiubaisCursor;
        }

        public void AddComment(string articleId, Comment comment) {
            var updateResult = this.Collection.Update(
                    Query<Article>.EQ(p => p.Id, articleId),
                    Update<Article>.Push(p => p.Comments, comment),
                    new MongoUpdateOptions {
                        WriteConcern = WriteConcern.Acknowledged
                    });

            if (updateResult.DocumentsAffected == 0) {
                //// Something went wrong

            }
        }


        public override void Update(Task entity) {
            //// Not necessary for the example
        }
    }
}