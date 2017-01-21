using Heisenslaught.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Heisenslaught.Draft
{
    public class DraftListViewModel : IMongoDocument<string>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string userId;
        public string battleTag;
        public string team1Name;
        public string team2Name;
        public string draftToken;
        public string adminToken;
        public string team1DrafterToken;
        public string team2DrafterToken;
        public string map;
        public int phase;
    }
}
