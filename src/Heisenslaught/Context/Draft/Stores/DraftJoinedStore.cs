using Heisenslaught.Models.Draft;
using Heisenslaught.Persistence.MongoDb.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Heisenslaught.Persistence.Draft
{
    public class DraftJoinedStore : CrudMongoStore<string, UserJoinedDraftModel>
    {
        public DraftJoinedStore(IMongoDatabase database) : base(database, "draft_joins") { }
        public DraftJoinedStore(IMongoDatabase database, string collectionName) : base(database, collectionName) {}
    }
}
