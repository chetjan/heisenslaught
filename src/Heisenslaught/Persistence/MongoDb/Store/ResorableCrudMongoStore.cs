using Heisenslaught.Persistence.MongoDb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Heisenslaught.Persistence.MongoDb.Store
{
    public class ResorableCrudMongoStore<TKey, TDocument> : CrudMongoStore<TKey, TDocument> where TDocument : IRestorableMongoDocument<TKey>
    {
        public ResorableCrudMongoStore(IMongoDatabase database, string collectionName) : base(database, collectionName)
        {
        }
        /*
        public override void Delete(TDocument document)
        {
            document.DeletedOn = new DateTime();
            Update(document);
        }

        public override Task DeleteAsync(TDocument document)
        {
            document.DeletedOn = new DateTime();
            return UpdateAsync(document);
        }

        protected override FilterDefinition<TDocument> PrepareReadQuery(FilterDefinition<TDocument> query)
        {
            var q = Builders<TDocument>.Filter.And(new List<FilterDefinition<TDocument>>
            {
                query,
                Builders<TDocument>.Filter.Eq( d => d.DeletedOn, null)
            });
            return base.PrepareReadQuery(query);
        }

        public void Undelete(TDocument document)
        {
            document.DeletedOn = null;
            Update(document);
        }

        public Task UndeleteAsync(TDocument document)
        {
            document.DeletedOn = null;
            return UpdateAsync(document);
        }
        */
    }
}
