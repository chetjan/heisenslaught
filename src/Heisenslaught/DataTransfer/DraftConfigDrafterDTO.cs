using Heisenslaught.Infrastructure;
using Heisenslaught.Models;


namespace Heisenslaught.DataTransfer
{
    public class DraftConfigDrafterDTO :DraftConfigDTO
    {
        public int team;


        public DraftConfigDrafterDTO(DraftModel model, string teamToken) : base(model)
        {
            Initialize(model, teamToken);
        }

        public DraftConfigDrafterDTO(DraftRoom room, string teamToken) : base(room) 
        {
            Initialize(room.DraftModel, teamToken);
        }

        private void Initialize(DraftModel model, string teamToken)
        {
            if (teamToken == model.team1DrafterToken)
            {
                team = 1;
            }
            else if (teamToken == model.team2DrafterToken)
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
