using Heisenslaught.Infrastructure.MongoDb;
using MongoDB.Driver;

namespace Heisenslaught.Draft
{
    public class DraftJoinedStore : CrudMongoStore<string, UserJoinedDraftModel>
    {
        public DraftJoinedStore(IMongoDatabase database) : base(database, "draft_joins") { }
        public DraftJoinedStore(IMongoDatabase database, string collectionName) : base(database, collectionName) {}
    }
}
