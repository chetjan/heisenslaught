using Heisenslaught.Infrastructure.MongoDb;
using MongoDB.Driver;

namespace Heisenslaught.Draft
{
    public class DraftListViewStore : CrudMongoStore<string, DraftListViewModel>
    {
        private bool _shouldInitViewGenerotors = false;

        public DraftListViewStore(IMongoDatabase database) : base(database, "draft_listviews") { }
        public DraftListViewStore(IMongoDatabase database, string collectionName) : base(database, collectionName) { }

        public bool ShouldInitializeGenerators
        {
            get
            {
                return _shouldInitViewGenerotors;
            }
        }

        protected override void EnsureIndecies()
        {
            if (!CollectionExists)
            {
                _shouldInitViewGenerotors = true;
            }
            base.EnsureIndecies();
            var draftToken = Builders<DraftListViewModel>.IndexKeys.Ascending(_ => _.draftToken);
            var userId = Builders<DraftListViewModel>.IndexKeys.Ascending(_ => _.userId);
            var battleTag = Builders<DraftListViewModel>.IndexKeys.Ascending(_ => _.battleTag);
            var team1Name = Builders<DraftListViewModel>.IndexKeys.Ascending(_ => _.team1Name);
            var team2Name = Builders<DraftListViewModel>.IndexKeys.Ascending(_ => _.team2Name);
            var map = Builders<DraftListViewModel>.IndexKeys.Ascending(_ => _.map);
            var phase = Builders<DraftListViewModel>.IndexKeys.Ascending(_ => _.phase);

            Collection.Indexes.CreateOneAsync(draftToken, new CreateIndexOptions { Unique = true });
            Collection.Indexes.CreateOneAsync(userId);
            Collection.Indexes.CreateOneAsync(battleTag);
            Collection.Indexes.CreateOneAsync(team1Name);
            Collection.Indexes.CreateOneAsync(team2Name);
            Collection.Indexes.CreateOneAsync(map);
            Collection.Indexes.CreateOneAsync(phase);
        }
    }
}
