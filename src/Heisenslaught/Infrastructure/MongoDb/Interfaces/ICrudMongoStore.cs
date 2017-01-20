using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Heisenslaught.Persistence.MongoDb.Models;
using MongoDB.Driver;
using System.Threading;
using Heisenslaught.Persistence.MongoDb.Options;
using System.Linq.Dynamic.Core;

namespace Heisenslaught.Persistence.MongoDb.Store
{
    public interface ICrudMongoStore<TKey, TDocument> where TDocument : IMongoDocument<TKey>
    {
        event EventHandler<TDocument> OnBeforeCreate;
        event EventHandler<TDocument> OnCreated;
        event EventHandler<TDocument> OnBeforeUpdate;
        event EventHandler<TDocument> OnUpdated;
        event EventHandler<TDocument> OnBeforeDelete;
        event EventHandler<TDocument> OnDeleted;

        string CollectionName { get; }
        IMongoCollection<TDocument> Collection { get; }
        bool CollectionExists { get; }

        void Create(TDocument document);
        Task CreateAsync(TDocument document, CancellationToken cancellationToken);

        void Update(TDocument document);
        Task UpdateAsync(TDocument document, CancellationToken cancellationToken);

        void Delete(TDocument document);
        Task DeleteAsync(TDocument document, CancellationToken cancellationToken);

        TDocument FindById(TKey id);
        Task<TDocument> FindByIdAsync(TKey id, CancellationToken cancellationToken);

        IFindFluent<TDocument, TDocument> BuildQuery(FilterDefinition<TDocument> query);
        IQueryable<TDocument> BuildQuery(Func<IQueryable<TDocument>, IQueryable<TDocument>> query);
        IQueryable<TResult> BuildQuery<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query);
        IQueryable<TDocument> BuildQuery(string query, params Object[] args);
        

        List<TDocument> Query(FilterDefinition<TDocument> query, IMongoSortingOptions<TDocument> sortOptions = null, IMongoLimitOptions<TDocument> limitOptions = null);
        PagedResult<TDocument> Query(FilterDefinition<TDocument> query, IMongoPageOptions<TDocument> pageOptions, IMongoSortingOptions<TDocument> sortOptions = null);

        List<TResult> Query<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query, ILinqSortingOptions<TResult> sortOptions = null, ILinqLimitOptions<TResult> limitOptions = null);
        PagedResult<TResult> Query<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query, ILinqPageOptions<TResult> pageOptions, ILinqSortingOptions<TResult> sortOptions = null);

        List<TDocument> Query(string query, ILinqSortingOptions<TDocument> sortOptions = null, ILinqLimitOptions<TDocument> limitOptions = null, params Object[] args);
        PagedResult<TDocument> Query(string query, ILinqPageOptions<TDocument> pageOptions , ILinqSortingOptions<TDocument> sortOptions = null, params Object[] args);

        Task<List<TDocument>> QueryAsync(FilterDefinition<TDocument> query, CancellationToken cancellationToken, IMongoSortingOptions<TDocument> sortOptions = null, IMongoLimitOptions<TDocument> limitOptions = null);
        Task<PagedResult<TDocument>> QueryAsync(FilterDefinition<TDocument> query, CancellationToken cancellationToken, IMongoPageOptions<TDocument> pageOptions, IMongoSortingOptions<TDocument> sortOptions = null);

      
       
        TDocument QueryOne(FilterDefinition<TDocument> query, IMongoSortingOptions<TDocument> sortOptions = null);
    
        TResult QueryOne<TResult>(Func<IQueryable<TDocument>, IQueryable<TResult>> query, ILinqSortingOptions<TResult> sortOptions = null);

        TDocument QueryOne(string query, ILinqSortingOptions<TDocument> sortOptions = null, params Object[] args);

        Task<TDocument> QueryOneAsync(FilterDefinition<TDocument> query, CancellationToken cancellationToken, IMongoSortingOptions<TDocument> sortOptions = null);

       
        List<TDocument> FindAll(ILinqSortingOptions<TDocument> sortOptions = null, ILinqLimitOptions<TDocument> limitOptions = null);
        PagedResult<TDocument> FindAll(ILinqPageOptions<TDocument> pageOptions, ILinqSortingOptions<TDocument> sortOptions = null);
        
    }
}
