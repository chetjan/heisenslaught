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
        private MongoClient Client;
        private IMongoDatabase Database;

        private IMongoCollection<DraftModel> DraftCollection;

        public MongoDraftRepository(string dbName = "Heisenslaught", string connectionString = "mongodb://localhost:27017")
        {
            Client = new MongoClient(connectionString);
            Database = Client.GetDatabase(dbName);
            if (!collectionExists("drafts"))
            {
                createDraftCollection();
            }
            DraftCollection = Database.GetCollection<DraftModel>("drafts");
        }

        private bool collectionExists(string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var collections = Database.ListCollections(new ListCollectionsOptions { Filter = filter });
            return collections.Any();
        }


        private void createDraftCollection()
        {
            Database.CreateCollection("drafts", new CreateCollectionOptions
            {
                AutoIndexId = true
            });

            var builder = new IndexKeysDefinitionBuilder<DraftModel>();
            var draftTokenIndex = builder.Ascending(_ => _.draftToken);
            var adminTokenIndex = builder.Ascending(_ => _.adminToken);
            var mapIndex = builder.Ascending(_ => _.config.map);
            var phaseIndex = builder.Ascending(_ => _.state.phase);


            var collection = Database.GetCollection<DraftModel>("drafts");
            collection.Indexes.CreateOne(draftTokenIndex, new CreateIndexOptions { Unique = true });
            collection.Indexes.CreateOne(adminTokenIndex);
            collection.Indexes.CreateOne(mapIndex);
            collection.Indexes.CreateOne(phaseIndex);
        }

        public DraftModel createDraft(DraftModel draft)
        {
            DraftCollection.InsertOne(draft);
            return draft;
        }

    }
}
