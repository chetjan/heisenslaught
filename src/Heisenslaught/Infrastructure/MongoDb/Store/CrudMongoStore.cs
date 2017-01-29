using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Heisenslaught.Infrastructure.MongoDb
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

            
        public CrudMongoStore(IMongoDatabase database, string collectionName)
        {
            CollectionName = collectionName;
            this.database = database;
            Collection = database.GetCollection<TDocument>(collectionName);
            EnsureIndecies();
        }

        public bool CollectionExists {
            get
            {
                var filter = new BsonDocument("name", CollectionName);
                var collections = database.ListCollections(new ListCollectionsOptions { Filter = filter });
                return collections.Any();
            }
        }

        public IMongoQueryable<TDocument> QueryableCollection
        {
            get
            {

                return Collection.AsQueryable<TDocument>();
            }
        }

        protected void Emit<T>(EventHandler<T> handler, T evt)
        {
            handler?.Invoke(this, evt);
        }

        protected virtual void EnsureIndecies() { }

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
            var q = Builders<TDocument>.Filter.Eq(_ => _.Id, document.Id);
            Collection.DeleteOne(q);
            Emit(OnDeleted, document);
        }

        public virtual Task DeleteAsync(TDocument document, CancellationToken cancellationToken)
        {
            Emit(OnBeforeDelete, document);
            var q = Builders<TDocument>.Filter.Eq(_ => _.Id, document.Id);
            var result = Collection.DeleteOneAsync(q);
            result.ContinueWith(task =>
            {
                Emit(OnDeleted, document);
            });
            return result;
        }



        public virtual void Update(TDocument document)
        {
            Emit(OnBeforeUpdate, document);
            var q = Builders<TDocument>.Filter.Eq(_ => _.Id, document.Id);
            Collection.FindOneAndReplace(q, document);
            Emit(OnUpdated, document);
        }

        public virtual Task UpdateAsync(TDocument document, CancellationToken cancellationToken)
        {
            Emit(OnBeforeUpdate, document);
            var q = Builders<TDocument>.Filter.Eq(_ => _.Id, document.Id);
            var result = Collection.FindOneAndReplaceAsync(q, document);
            result.ContinueWith(task => {
                Emit(OnUpdated, document);
            });
            return result;
        }

        public virtual void CreateOrUpdate(TDocument document)
        {
            Emit(OnBeforeCreate, document);
            var q = Builders<TDocument>.Filter.Eq(_ => _.Id, document.Id);
            var result = Collection.ReplaceOne(q, document, new UpdateOptions { IsUpsert = true });
            if(result.UpsertedId != null)
            {
                Emit(OnCreated, document);
            }
            else
            {
                Emit(OnUpdated, document);
            }
            
        }

        public virtual TDocument FindById(Tkey id)
        {
            var q = Builders<TDocument>.Filter.Eq(_ => _.Id, id);
            return Collection.Find(q).FirstOrDefault();
        }

        public virtual Task<TDocument> FindByIdAsync(Tkey id, CancellationToken cancellationToken)
        {
            var q = Builders<TDocument>.Filter.Eq(_ => _.Id, id);
            return Collection.Find(q).FirstOrDefaultAsync();
        }

       /* protected IFindFluent<TDocument, TDocument> Find(FilterDefinition<TDocument> filter)
        {
            return Collection.Find(filter);
        }
        
        protected IFindFluent<TDocument, TDocument> Find(Expression<Func<TDocument, bool>> filter)
        {
            return Collection.Find(filter);
        }
        */
        public IFindFluent<TDocument, TDocument> BuildQuery(FilterDefinition<TDocument> query)
        {
            return Collection.Find(query);
        }

        public IQueryable<TDocument> BuildQuery(Func<IQueryable<TDocument>, IQueryable<TDocument>> query)
        {
            return BuildQuery<TDocument>(query);
        }

        public IQueryable<TResult> BuildQuery<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query)
        {
            var q = Collection.AsQueryable<TDocument>();
            return query(q);
        }

        public IQueryable<TDocument> BuildQuery(string query, params object[] args)
        {
            var q = Collection.AsQueryable<TDocument>();
            return q.Where<TDocument>(query);
        }

        public List<TDocument> Query(FilterDefinition<TDocument> query, IMongoSortingOptions<TDocument> sortOptions = null, IMongoLimitOptions<TDocument> limitOptions = null)
        {
            var result = BuildQuery(query);
            if (sortOptions != null)
                result = sortOptions.ApplySort(result);
            if (limitOptions != null)
                result = limitOptions.ApplyLimits(result);
            return result.ToList();
        }

        public PagedResult<TDocument> Query(FilterDefinition<TDocument> query, IMongoPageOptions<TDocument> pageOptions, IMongoSortingOptions<TDocument> sortOptions = null)
        {
            var result = BuildQuery(query);
            if (sortOptions != null)
                result = sortOptions.ApplySort(result);
            return pageOptions.ApplyPagination(result);
        }

        public List<TResult> Query<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query, ILinqSortingOptions<TResult> sortOptions = null, ILinqLimitOptions<TResult> limitOptions = null)
        {
            var result = BuildQuery(query);
            if (sortOptions != null)
                result = sortOptions.ApplySort(result);
            if (limitOptions != null)
                result = limitOptions.ApplyLimits(result);
            return result.ToList();
        }

        public PagedResult<TResult> Query<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query, ILinqPageOptions<TResult> pageOptions, ILinqSortingOptions<TResult> sortOptions = null)
        {
            var result = BuildQuery(query);
            if (sortOptions != null)
                result = sortOptions.ApplySort(result);
            return pageOptions.ApplyPagination(result);
        }

        public List<TDocument> Query(string query, ILinqSortingOptions<TDocument> sortOptions = null, ILinqLimitOptions<TDocument> limitOptions = null, params object[] args)
        {
            var result = BuildQuery(query);
            if (sortOptions != null)
                result = sortOptions.ApplySort(result);
            if (limitOptions != null)
                result = limitOptions.ApplyLimits(result);
            return result.ToList();
        }

        public PagedResult<TDocument> Query(string query, ILinqPageOptions<TDocument> pageOptions, ILinqSortingOptions<TDocument> sortOptions = null, params object[] args)
        {
            var result = BuildQuery(query);
            if (sortOptions != null)
                result = sortOptions.ApplySort(result);
            return pageOptions.ApplyPagination(result);
        }

        public Task<List<TDocument>> QueryAsync(FilterDefinition<TDocument> query, CancellationToken cancellationToken, IMongoSortingOptions<TDocument> sortOptions = null, IMongoLimitOptions<TDocument> limitOptions = null)
        {
            var result = BuildQuery(query);
            if (sortOptions != null)
                result = sortOptions.ApplySort(result);
            if (limitOptions != null)
                result = limitOptions.ApplyLimits(result);
            return result.ToListAsync(cancellationToken);
        }

        public Task<PagedResult<TDocument>> QueryAsync(FilterDefinition<TDocument> query, CancellationToken cancellationToken, IMongoPageOptions<TDocument> pageOptions, IMongoSortingOptions<TDocument> sortOptions = null)
        {
            var result = BuildQuery(query);
            if (sortOptions != null)
                result = sortOptions.ApplySort(result);
            return pageOptions.ApplyPaginationAsync(result, cancellationToken);
        }

        public TDocument QueryOne(FilterDefinition<TDocument> query, IMongoSortingOptions<TDocument> sortOptions = null)
        {
            var result = BuildQuery(query);
            if (sortOptions != null)
                result = sortOptions.ApplySort(result);
            return result.FirstOrDefault();
        }

        public TResult QueryOne<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query, ILinqSortingOptions<TResult> sortOptions = null)
        {
            var result = BuildQuery(query);
            if (sortOptions != null)
                result = sortOptions.ApplySort(result);
            return result.FirstOrDefault();
        }

        public TDocument QueryOne(string query, ILinqSortingOptions<TDocument> sortOptions = null, params object[] args)
        {
            var result = BuildQuery(query);
            if (sortOptions != null)
                result = sortOptions.ApplySort(result);
            return result.FirstOrDefault();
        }

        public Task<TDocument> QueryOneAsync(FilterDefinition<TDocument> query, CancellationToken cancellationToken, IMongoSortingOptions<TDocument> sortOptions = null)
        {
            var result = BuildQuery(query);
            if (sortOptions != null)
                result = sortOptions.ApplySort(result);
            return result.FirstOrDefaultAsync(cancellationToken);
        }

        public List<TDocument> FindAll(ILinqSortingOptions<TDocument> sortOptions = null, ILinqLimitOptions<TDocument> limitOptions = null)
        {
            var result = (IQueryable<TDocument>)Collection.AsQueryable();
            if (sortOptions != null)
                result = sortOptions.ApplySort(result);
            if (limitOptions != null)
                result = limitOptions.ApplyLimits(result);
            return result.ToList();
        }

        public PagedResult<TDocument> FindAll(ILinqPageOptions<TDocument> pageOptions, ILinqSortingOptions<TDocument> sortOptions = null)
        {
            var result = (IQueryable<TDocument>)Collection.AsQueryable();
            if (sortOptions != null)
                result = sortOptions.ApplySort(result);
            return pageOptions.ApplyPagination(result);
        }


        /*
        public List<TDocument> Query(FilterDefinition<TDocument> query)
        {
            return BuildQuery(query).ToList();
        }

        public List<TDocument> Query(FilterDefinition<TDocument> query, QueryOptions<TDocument> options)
        {
            var result = BuildQuery(query);
            if(options.Sort != null)
            {
                result = result.Sort(options.Sort);
            }
            return result.ToList();
        }

        public List<TDocument> Query(FilterDefinition<TDocument> query, QueryLimitOptions<TDocument> options)
        {
            var result = BuildQuery(query);
            if (options.Sort != null)
            {
                result = result.Sort(options.Sort);
            }
            if (options.Skip != null)
            {
                result = result.Skip(options.Skip);
            }
            if (options.Limit != null)
            {
                result = result.Limit(options.Limit);
            }
            return result.ToList();
        }

        public PagedResult<TDocument> Query(FilterDefinition<TDocument> query, PageableQueryOptions<TDocument> options)
        {
            var result = BuildQuery(query);
            if (options.Sort != null)
            {
                result = result.Sort(options.Sort);
            }

            throw new NotImplementedException();

        }

        public List<TResult> Query<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query)
        {
            var result = BuildQuery(query);
            return result.ToList();
        }

        public List<TResult> Query<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query, QueryOptions<TDocument> options)
        {
            var result = BuildQuery(query);
            result.OrderByDescending()
            /*var result = BuildQuery(query);
            if(options.Sort != null)
            {
                options.Sort.
            }
            return result.ToList();
            throw new NotImplementedException();
        }

        public List<TResult> Query<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query, QueryLimitOptions<TDocument> options)
        {
            throw new NotImplementedException();
        }

        public PagedResult<TResult> Query<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query, PageableQueryOptions<TDocument> options)
        {
            throw new NotImplementedException();
        }

        public List<TDocument> Query(string query, params object[] args)
        {
            throw new NotImplementedException();
        }

        public List<TDocument> Query(string query, QueryOptions<TDocument> options, params object[] args)
        {
            throw new NotImplementedException();
        }

        public List<TDocument> Query(string query, QueryLimitOptions<TDocument> options, params object[] args)
        {
            throw new NotImplementedException();
        }

        public PagedResult<TDocument> Query(string query, PageableQueryOptions<TDocument> options, params object[] args)
        {
            throw new NotImplementedException();
        }

        public Task<List<TDocument>> QueryAsync(FilterDefinition<TDocument> query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<TDocument>> QueryAsync(FilterDefinition<TDocument> query, QueryOptions<TDocument> options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<TDocument>> QueryAsync(FilterDefinition<TDocument> query, QueryLimitOptions<TDocument> options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<TDocument>> QueryAsync(FilterDefinition<TDocument> query, PageableQueryOptions<TDocument> options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<TResult>> QueryAsync<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<TResult>> QueryAsync<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query, QueryOptions<TDocument> options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<TResult>> QueryAsync<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query, QueryLimitOptions<TDocument> options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<TResult>> QueryAsync<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query, PageableQueryOptions<TDocument> options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<TDocument>> QueryAsync(string query, CancellationToken cancellationToken, params object[] args)
        {
            throw new NotImplementedException();
        }

        public Task<List<TDocument>> QueryAsync(string query, CancellationToken cancellationToken, QueryOptions<TDocument> options, params object[] args)
        {
            throw new NotImplementedException();
        }

        public Task<List<TDocument>> QueryAsync(string query, CancellationToken cancellationToken, QueryLimitOptions<TDocument> options, params object[] args)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<TDocument>> QueryAsync(string query, CancellationToken cancellationToken, PageableQueryOptions<TDocument> options, params object[] args)
        {
            throw new NotImplementedException();
        }

        public TDocument QueryOne(FilterDefinition<TDocument> query)
        {
            throw new NotImplementedException();
        }

        public TDocument QueryOne(FilterDefinition<TDocument> query, QueryOptions<TDocument> options)
        {
            throw new NotImplementedException();
        }

        public TDocument QueryOne(FilterDefinition<TDocument> query, QueryLimitOptions<TDocument> options)
        {
            throw new NotImplementedException();
        }

        public TResult QueryOne<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query)
        {
            throw new NotImplementedException();
        }

        public TResult QueryOne<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query, QueryOptions<TDocument> options)
        {
            throw new NotImplementedException();
        }

        public TResult QueryOne<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query, QueryLimitOptions<TDocument> options)
        {
            throw new NotImplementedException();
        }

        public TDocument QueryOne(string query, params object[] args)
        {
            throw new NotImplementedException();
        }

        public TDocument QueryOne(string query, QueryOptions<TDocument> options, params object[] args)
        {
            throw new NotImplementedException();
        }

        public TDocument QueryOne(string query, QueryLimitOptions<TDocument> options, params object[] args)
        {
            throw new NotImplementedException();
        }

        public Task<TDocument> QueryOneAsync(FilterDefinition<TDocument> query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TDocument> QueryOneAsync(FilterDefinition<TDocument> query, QueryOptions<TDocument> options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TDocument> QueryOneAsync(FilterDefinition<TDocument> query, QueryLimitOptions<TDocument> options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> QueryOneAsync<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> QueryOneAsync<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query, QueryOptions<TDocument> options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> QueryOneAsync<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query, QueryLimitOptions<TDocument> options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TDocument> QueryOneAsync(string query, CancellationToken cancellationToken, params object[] args)
        {
            throw new NotImplementedException();
        }

        public Task<TDocument> QueryOneAsync(string query, QueryOptions<TDocument> options, CancellationToken cancellationToken, params object[] args)
        {
            throw new NotImplementedException();
        }

        public Task<TDocument> QueryOneAsync(string query, QueryLimitOptions<TDocument> options, CancellationToken cancellationToken, params object[] args)
        {
            throw new NotImplementedException();
        }

        public List<TDocument> FindAll()
        {
            throw new NotImplementedException();
        }

        public List<TDocument> FindAll(QueryOptions<TDocument> options)
        {
            throw new NotImplementedException();
        }

        public List<TDocument> FindAll(QueryLimitOptions<TDocument> options)
        {
            throw new NotImplementedException();
        }

        public PagedResult<TDocument> FindAll(PageableQueryOptions<TDocument> options)
        {
            throw new NotImplementedException();
        }

        public Task<List<TDocument>> FindAllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<TDocument>> FindAllAsync(QueryOptions<TDocument> options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<TDocument>> FindAllAsync(QueryLimitOptions<TDocument> options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<TDocument>> FindAllAsync(PageableQueryOptions<TDocument> options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }




        /*
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

        */

    }
}
