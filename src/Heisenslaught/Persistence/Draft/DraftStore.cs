using Heisenslaught.Models.Draft;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System;
using Heisenslaught.Persistence.MongoDb.Store;
using System.Linq.Dynamic.Core;

namespace Heisenslaught.Persistence.Draft
{
    public class DraftStore : CrudMongoStore<string, DraftModel>, IDraftStore
    {
        public DraftStore(IMongoDatabase database) : base(database, "drafts") { }
        public DraftStore(IMongoDatabase database, string collectionName) : base(database, collectionName) { }


        public DraftModel FindByDraftToken(string draftToken)
        {
            var q = Builders<DraftModel>.Filter.Eq(_ => _.draftToken, draftToken);
            return QueryOne(q);
        }

        public List<DraftModel> FindByUserId(string userId)
        {
            var q = Builders<DraftModel>.Filter.Eq(_ => _.createdBy, userId);
            return Query(q);
        }

        protected override void EnsureIndecies()
        {
            base.EnsureIndecies();
            var draftToken =  Builders<DraftModel>.IndexKeys.Ascending(_ => _.draftToken);
            var createdBy = Builders<DraftModel>.IndexKeys.Ascending(_ => _.createdBy);
            var map = Builders<DraftModel>.IndexKeys.Ascending(_ => _.config.map);
            var phase = Builders<DraftModel>.IndexKeys.Ascending(_ => _.state.phase);

            Collection.Indexes.CreateOneAsync(draftToken, new CreateIndexOptions { Unique = true });
            Collection.Indexes.CreateOneAsync(createdBy);
            Collection.Indexes.CreateOneAsync(map);
            Collection.Indexes.CreateOneAsync(phase);
        }

        /*
         * 
         *  private void CreateDraftCollection()
        {
            _database.CreateCollection("drafts", new CreateCollectionOptions
            {
                AutoIndexId = true
            });

            var builder = new IndexKeysDefinitionBuilder<DraftModel>();
            var draftTokenIndex = builder.Ascending(_ => _.draftToken);
            var adminTokenIndex = builder.Ascending(_ => _.adminToken);
            var mapIndex = builder.Ascending(_ => _.config.map);
            var phaseIndex = builder.Ascending(_ => _.state.phase);

            var collection = _database.GetCollection<DraftModel>("drafts");
            collection.Indexes.CreateOne(draftTokenIndex, new CreateIndexOptions { Unique = true });
            collection.Indexes.CreateOne(adminTokenIndex);
            collection.Indexes.CreateOne(mapIndex);
            collection.Indexes.CreateOne(phaseIndex);
        }
        private IMongoDatabase _database;
        private IMongoCollection<DraftModel> _draftCollection;

        public DraftStore(IMongoDatabase database, ILoggerFactory loggerFactory) : this(database, loggerFactory, "drafts") { }
        public DraftStore(IMongoDatabase database, ILoggerFactory loggerFactory, string draftCollectionName)
        {
            _database = database;
            if (!CollectionExists("drafts"))
            {
                CreateDraftCollection();
            }
            _draftCollection = _database.GetCollection<DraftModel>("drafts");
        }

        private bool CollectionExists(string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var collections = _database.ListCollections(new ListCollectionsOptions { Filter = filter });
            return collections.Any();
        }

       

        public void CreateDraft(DraftModel draft)
        {
            _draftCollection.InsertOne(draft);
            emit(OnDraftCreated, draft);
        }

        protected void emit(EventHandler<DraftModel> evt, DraftModel model)
        {
            evt?.Invoke(this, model);
        }

        public DraftModel FindById(string id)
        {
            return null;
        }

        public DraftModel FindByDraftToken(string draftToken)
        {
            var q = Builders<DraftModel>.Filter.Eq("draftToken", draftToken);
            return _draftCollection.Find<DraftModel>(q).First<DraftModel>();
        }

        public DraftModel FindByUserId(string userId)
        {
          

            return null;
        }

        public ReplaceOneResult SaveDraft(DraftModel draft)
        {
            var q = Builders<DraftModel>.Filter.Eq("_id", draft.Id);
            var result = _draftCollection.ReplaceOne(q, draft);
            OnDraftUpdated(this, draft);
           
            return result;
        }

    */

    }
}
