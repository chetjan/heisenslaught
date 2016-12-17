using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

using Heisenslaught.Models;

namespace Heisenslaught.Persistence
{
    public class MongoDraftRepository
    {
        private MongoClient client;
        private IMongoDatabase database;

        private IMongoCollection<DraftModel> draftCollection;

        public MongoDraftRepository(string dbName = "Heisenslaught", string connectionString = "mongodb://localhost:27017")
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase(dbName);
            if (!CollectionExists("drafts"))
            {
                CreateDraftCollection();
            }
            draftCollection = database.GetCollection<DraftModel>("drafts");
        }

        private bool CollectionExists(string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var collections = database.ListCollections(new ListCollectionsOptions { Filter = filter });
            return collections.Any();
        }

        private void CreateDraftCollection()
        {
            database.CreateCollection("drafts", new CreateCollectionOptions
            {
                AutoIndexId = true
            });

            var builder = new IndexKeysDefinitionBuilder<DraftModel>();
            var draftTokenIndex = builder.Ascending(_ => _.draftToken);
            var adminTokenIndex = builder.Ascending(_ => _.adminToken);
            var mapIndex = builder.Ascending(_ => _.config.map);
            var phaseIndex = builder.Ascending(_ => _.state.phase);

            var collection = database.GetCollection<DraftModel>("drafts");
            collection.Indexes.CreateOne(draftTokenIndex, new CreateIndexOptions { Unique = true });
            collection.Indexes.CreateOne(adminTokenIndex);
            collection.Indexes.CreateOne(mapIndex);
            collection.Indexes.CreateOne(phaseIndex);
        }

        public void CreateDraft(DraftModel draft)
        {
            draftCollection.InsertOne(draft);
        }

        public DraftModel FindByDraftToken(string draftToken)
        {
            var q = Builders<DraftModel>.Filter.Eq("draftToken", draftToken);
            return draftCollection.Find<DraftModel>(q).First<DraftModel>();
        }

        public ReplaceOneResult SaveDraft(DraftModel draft)
        {
            var q = Builders<DraftModel>.Filter.Eq("_id", draft._id);
            return draftCollection.ReplaceOne(q, draft);
        }

    }
}
