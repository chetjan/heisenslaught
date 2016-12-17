using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Heisenslaught.Models;

namespace Heisenslaught.DataTransfer
{
    public class DraftConfigDrafterDTO :DraftConfigDTO
    {
        public int team;


        public DraftConfigDrafterDTO(DraftModel model, string teamToken) : base(model)
        {
            if(teamToken == model.team1DrafterToken)
            {
                team = 1;
            }else if(teamToken == model.team2DrafterToken)
            {
                team = 2;
            }
            else
            {
                team = 0;
            }
        }
    }
}
