using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Heisenslaught.Persistence.MongoDb.Models;
using MongoDB.Driver;
using System.Threading;

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

        TDocument QueryOne(string query);
        Task<TDocument> QueryOneAsync(string query, CancellationToken cancellationToken);

        TDocument QueryOne(FilterDefinition<TDocument> query);
        Task<TDocument> QueryOneAsync(FilterDefinition<TDocument> query, CancellationToken cancellationToken);

        List<TDocument> FindAll(int? limit, int? skip);
        Task<List<TDocument>> FindAllAsync(int? limit, int? skip, CancellationToken cancellationToken);
        List<TDocument> FindAll(SortDefinition<TDocument> sort, int? limit, int? skip);
        Task<List<TDocument>> FindAllAsync(SortDefinition<TDocument> sort, int? limit, int? skip, CancellationToken cancellationToken);

        List<TDocument> Query(string query, int? limit, int? skip);
        Task<List<TDocument>> QueryAsync(string query, int? limit, int? skip, CancellationToken cancellationToken);
        List<TDocument> Query(string query, string sort, int? limit, int? skip);
        Task<List<TDocument>> QueryAsync(string query, string sort, int? limit, int? skip, CancellationToken cancellationToken);

        List<TDocument> Query(FilterDefinition<TDocument> query, int? limit, int? skip);
        Task<List<TDocument>> QueryAsync(FilterDefinition<TDocument> query, int? limit, int? skip, CancellationToken cancellationToken);
        List<TDocument> Query(FilterDefinition<TDocument> query, SortDefinition<TDocument> sort, int? limit, int? skip);
        Task<List<TDocument>> QueryAsync(FilterDefinition<TDocument> query, SortDefinition<TDocument> sort, int? limit, int? skip, CancellationToken cancellationToken);
    }
}
