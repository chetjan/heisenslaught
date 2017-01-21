using Heisenslaught.Infrastructure.MongoDb;
using Heisenslaught.Users;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Heisenslaught.Draft
{
    public class UserJoinedDraftModel : IMongoDocument<string>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string user;
        [BsonRepresentation(BsonType.ObjectId)]
        public string draft;
        public DateTime joinedOn;
        public DraftConnectionType joinedAs;

        public UserJoinedDraftModel(){}
        public UserJoinedDraftModel(HSUser user, DraftModel draft, DraftConnectionType joinedAs) : this(user.Id, draft.Id, joinedAs) { }

        public UserJoinedDraftModel(string userId, string draftId, DraftConnectionType joinedAs)
        {
            this.user = userId;
            this.draft = draftId;
            this.joinedOn = DateTime.Now;
            this.joinedAs = joinedAs;
        }
    }
}
