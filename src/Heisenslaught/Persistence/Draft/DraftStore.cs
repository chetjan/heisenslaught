using Heisenslaught.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;

namespace Heisenslaught.Persistence.Draft
{
    public class DraftStore : IDraftStore
    {
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

        private void CreateDraftCollection()
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

        public void CreateDraft(DraftModel draft)
        {
            _draftCollection.InsertOne(draft);
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
            /*var q = Builders<DraftModel>.Filter.Eq("draftToken", draftToken);
            return draftCollection.Find<DraftModel>(q).First<DraftModel>();
            */
            return null;
        }

        public ReplaceOneResult SaveDraft(DraftModel draft)
        {
            var q = Builders<DraftModel>.Filter.Eq("_id", draft._id);
            return _draftCollection.ReplaceOne(q, draft);
        }


    }
}
