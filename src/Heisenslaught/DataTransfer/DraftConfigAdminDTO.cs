using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Heisenslaught.Models;

namespace Heisenslaught.DataTransfer
{
    public class DraftConfigAdminDTO : DraftConfigDTO 
    {
        public string id;
        public string draftToken;
        public string team1DrafterToken;
        public string team2DrafterToken;
        public string adminToken;
        public bool wasFirstPickRandom;

        public DraftConfigAdminDTO(DraftModel model): base(model)
        {
            id = model._id.ToString();
            draftToken = model.draftToken;
            team1DrafterToken = model.team1DrafterToken;
            team2DrafterToken = model.team2DrafterToken;
            adminToken = model.adminToken;
            wasFirstPickRandom = model.wasFirstPickRandom;
        }
    }
}
