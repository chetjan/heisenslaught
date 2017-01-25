using Heisenslaught.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Heisenslaught.Draft
{
    public class DraftModel : IMongoDocument<string>
    {
        private static Random rnd = new Random((int)DateTime.Now.Ticks);
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string draftToken;
        public string adminToken;
        public string team1DrafterToken;
        public string team2DrafterToken;
        public bool wasFirstPickRandom = false;
        public DraftConfigModel config;
        public DraftStateModel state;
        public string createdBy;

        

        public DraftModel(DraftConfigModel config = null)
        {
            if (config != null)
            {
                this.config = config;
                state = new DraftStateModel();
                draftToken = GenerateToken();
                adminToken = GenerateToken();
                team1DrafterToken = GenerateToken();
                team2DrafterToken = GenerateToken();
                if (config.firstPick != 1 && config.firstPick != 2)
                {
                    int rng = rnd.Next(1, 10000);
                    config.firstPick = (rng % 2) + 1;
                    wasFirstPickRandom = true;
                }
            }
        }

        private string GenerateToken()
        {
            string token = null;
            while (token == null || token.IndexOfAny(new char[] { '/', '+' }) != -1)
            {
                token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            }
            return token.TrimEnd('=');
        }
    }
}
