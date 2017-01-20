using Heisenslaught.Infrastructure;
using Heisenslaught.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using Heisenslaught.Persistence.MongoDb.Models;
using MongoDB.Bson;

namespace Heisenslaught.Models.Draft
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
