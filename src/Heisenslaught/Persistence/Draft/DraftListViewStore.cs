using Heisenslaught.Models.Draft;
using Heisenslaught.Persistence.MongoDb.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Heisenslaught.Persistence.Draft
{
    public class DraftListViewStore : CrudMongoStore<string, DraftListViewModel>
    {
        public DraftListViewStore(IMongoDatabase database) : base(database, "draft_listviews") { }
        public DraftListViewStore(IMongoDatabase database, string collectionName) : base(database, collectionName)
        {
        }
    }
}
