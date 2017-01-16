using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Heisenslaught.Persistence.MongoDb.Models;
using System.Threading;
using MongoDB.Bson;

namespace Heisenslaught.Persistence.MongoDb.Store
{
    public class CrudMongoStore<Tkey, TDocument>  : ICrudMongoStore<Tkey, TDocument>  where TDocument : IMongoDocument<Tkey> 
    {
        protected readonly IMongoDatabase database;

        public event EventHandler<TDocument> OnBeforeCreate;
        public event EventHandler<TDocument> OnCreated;
        public event EventHandler<TDocument> OnBeforeUpdate;
        public event EventHandler<TDocument> OnUpdated;
        public event EventHandler<TDocument> OnBeforeDelete;
        public event EventHandler<TDocument> OnDeleted;

        public string CollectionName { get; private set; }
        public IMongoCollection<TDocument> Collection { get; private set; }

        public bool CollectionExists {
            get
            {
                var filter = new BsonDocument("name", CollectionName);
                var collections = database.ListCollections(new ListCollectionsOptions { Filter = filter });
                return collections.Any();
            }
        }
            
        

        public CrudMongoStore(IMongoDatabase database, string collectionName)
        {
            CollectionName = collectionName;
            this.database = database;
            Collection = database.GetCollection<TDocument>(collectionName);
        }

        public virtual void Create(TDocument document)
        {
            Emit(OnBeforeCreate, document);
            Collection.InsertOne(document);
            Emit(OnCreated, document);
        }

        public virtual Task CreateAsync(TDocument document, CancellationToken cancellationToken)
        {
            Emit(OnBeforeCreate, document);
            var result = Collection.InsertOneAsync(document);
            result.ContinueWith(task =>
            {
                Emit(OnCreated, document);
            });
            return result;
        }

        public virtual void Delete(TDocument document)
        {
            Emit(OnBeforeDelete, document);
            var q = Builders<TDocument>.Filter.Eq("_id", document.Id);
            Collection.DeleteOne(PrepareWriteQuery(q));
            Emit(OnDeleted, document);
        }

        public virtual Task DeleteAsync(TDocument document, CancellationToken cancellationToken)
        {
            Emit(OnBeforeDelete, document);
            var q = Builders<TDocument>.Filter.Eq("_id", document.Id);
            var result = Collection.DeleteOneAsync(PrepareWriteQuery(q));
            result.ContinueWith(task =>
            {
                Emit(OnDeleted, document);
            });
            return result;
        }

        public virtual List<TDocument> FindAll(int? limit, int? skip)
        {
            return BuildQuery(Builders<TDocument>.Filter.Empty, limit, skip).ToList();
        }

        public virtual List<TDocument> FindAll(SortDefinition<TDocument> sort, int? limit, int? skip)
        {
            return BuildQuery(Builders<TDocument>.Filter.Empty, limit, skip).Sort(sort).ToList();
        }

        public virtual Task<List<TDocument>> FindAllAsync(int? limit, int? skip, CancellationToken cancellationToken)
        {
            return BuildQuery(Builders<TDocument>.Filter.Empty, limit, skip).ToListAsync();
        }

        public virtual Task<List<TDocument>> FindAllAsync(SortDefinition<TDocument> sort, int? limit, int? skip, CancellationToken cancellationToken)
        {
            return BuildQuery(Builders<TDocument>.Filter.Empty, limit, skip).Sort(sort).ToListAsync();
        }

        public virtual TDocument FindById(Tkey id)
        {
            var q = Builders<TDocument>.Filter.Eq("_id", id);
            return Collection.Find(PrepareReadQuery(q)).FirstOrDefault();
        }

        public virtual Task<TDocument> FindByIdAsync(Tkey id, CancellationToken cancellationToken)
        {
            var q = Builders<TDocument>.Filter.Eq("_id", id);
            return Collection.Find(PrepareReadQuery(q)).FirstOrDefaultAsync();
        }

        public virtual List<TDocument> Query(FilterDefinition<TDocument> query, int? limit, int? skip)
        {
            return BuildQuery(query, limit, skip).ToList();
        }

        public virtual List<TDocument> Query(string query, int? limit, int? skip)
        {
            var q = new JsonFilterDefinition<TDocument>(query);
            return Query(q, limit, skip);
        }

        public virtual List<TDocument> Query(FilterDefinition<TDocument> query, SortDefinition<TDocument> sort, int? limit, int? skip)
        {
            return BuildQuery(query, limit, skip).Sort(sort).ToList();
        }

        public virtual List<TDocument> Query(string query, string sort, int? limit, int? skip)
        {
            var q = new JsonFilterDefinition<TDocument>(query);
            var s = new JsonSortDefinition<TDocument>(sort);
            return Query(q, s, limit, skip);
        }

        public virtual Task<List<TDocument>> QueryAsync(FilterDefinition<TDocument> query, int? limit, int? skip, CancellationToken cancellationToken)
        {
            return BuildQuery(query, limit, skip).ToListAsync();
        }

        public virtual Task<List<TDocument>> QueryAsync(string query, int? limit, int? skip, CancellationToken cancellationToken)
        {
            var q = new JsonFilterDefinition<TDocument>(query);
            return QueryAsync(q, limit, skip, cancellationToken);
        }

        public virtual Task<List<TDocument>> QueryAsync(FilterDefinition<TDocument> query, SortDefinition<TDocument> sort, int? limit, int? skip, CancellationToken cancellationToken)
        {
            return BuildQuery(query, limit, skip).Sort(sort).ToListAsync();
        }

        public virtual Task<List<TDocument>> QueryAsync(string query, string sort, int? limit, int? skip, CancellationToken cancellationToken)
        {
            var q = new JsonFilterDefinition<TDocument>(query);
            var s = new JsonSortDefinition<TDocument>(sort);
            return QueryAsync(q, s, limit, skip, cancellationToken);
        }

        public virtual TDocument QueryOne(FilterDefinition<TDocument> query)
        {
            return Collection.Find(PrepareReadQuery(query)).FirstOrDefault();
        }

        public virtual TDocument QueryOne(string query)
        {
            var q = new JsonFilterDefinition<TDocument>(query);
            return Collection.Find(PrepareReadQuery(q)).FirstOrDefault();
        }

        public virtual Task<TDocument> QueryOneAsync(FilterDefinition<TDocument> query, CancellationToken cancellationToken)
        {
            return Collection.Find(PrepareReadQuery(query)).FirstOrDefaultAsync();
        }

        public virtual Task<TDocument> QueryOneAsync(string query, CancellationToken cancellationToken)
        {
            var q = new JsonFilterDefinition<TDocument>(query);
            return Collection.Find(PrepareReadQuery(q)).FirstOrDefaultAsync();
        }

        public virtual void Update(TDocument document)
        {
            Emit(OnBeforeUpdate, document);
            var q = Builders<TDocument>.Filter.Eq("_id", document.Id);
            Collection.FindOneAndReplace(PrepareWriteQuery(q), document);
           Emit(OnUpdated, document);
        }

        public virtual Task UpdateAsync(TDocument document, CancellationToken cancellationToken)
        {
            Emit(OnBeforeUpdate, document);
            var q = Builders<TDocument>.Filter.Eq("_id", document.Id);
            var result = Collection.FindOneAndReplaceAsync(PrepareWriteQuery(q), document);
            result.ContinueWith(task => {
               Emit(OnUpdated, document);
            });
            return result;
        }

        protected virtual IFindFluent<TDocument, TDocument> BuildQuery(FilterDefinition<TDocument> query, int? limit, int? skip)
        {
            var result = Collection.Find(PrepareReadQuery(query));
            if (skip != null)
            {
                result = result.Skip(skip);
            }
            if (limit != null)
            {
                result = result.Limit(limit);
            }
            return result;
        }

        protected virtual FilterDefinition<TDocument> PrepareReadQuery(FilterDefinition<TDocument> query)
        {
            return query;
        }
        protected virtual FilterDefinition<TDocument> PrepareWriteQuery(FilterDefinition<TDocument> query)
        {
            return query;
        }

        protected void Emit<T>(EventHandler<T> handler, T evt)
        {
            handler?.Invoke(this, evt);
        }

    }
}
