using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using Heisenslaught.Infrastructure;

namespace Heisenslaught.Models
{
    public class DraftModel
    {
        private static Random rnd = new Random((int)DateTime.Now.Ticks);

        public ObjectId _id;
        public string draftToken;
        public string adminToken;
        public string team1DrafterToken;
        public string team2DrafterToken;
        public bool wasFirstPickRandom = false;
        public DraftConfigModel config;
        public DraftStateModel state;

        public DraftModel(DraftConfigModel config = null)
        {
            if (config != null)
            {
                this.config = config;
                state = new DraftStateModel();
                draftToken = generateToken();
                adminToken = generateToken();
                team1DrafterToken = generateToken();
                team2DrafterToken = generateToken();
                if (config.firstPick != 1 && config.firstPick != 2)
                {
                    int rng = rnd.Next(1, 10000);
                    config.firstPick = (rng % 2) + 1;
                    wasFirstPickRandom = true;
                }
            }
        }

        private string generateToken()
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
