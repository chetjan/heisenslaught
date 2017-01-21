using Heisenslaught.Infrastructure.MongoDb;
using Heisenslaught.Users;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;

namespace Heisenslaught.Draft
{
    public class UserJoinedDraftModel : IMongoDocument<string>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public MongoDBRef user;
        public MongoDBRef draft;
        public DateTime joinedOn;
        public DraftConnectionType joinedAs;

        public UserJoinedDraftModel(){}
        public UserJoinedDraftModel(HSUser user, DraftModel draft, DraftConnectionType joinedAs) : this(user.Id, draft.Id, joinedAs) { }

        public UserJoinedDraftModel(string userId, string draftId, DraftConnectionType joinedAs)
        {
            this.user = new MongoDBRef("users", userId);
            this.draft = new MongoDBRef("drafts", draftId);
            this.joinedOn = new DateTime();
            this.joinedAs = joinedAs;
        }
    }
}
