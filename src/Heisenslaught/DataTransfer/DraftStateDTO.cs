using Heisenslaught.Infrastructure;
using Heisenslaught.Models;
using System.Collections.Generic;


namespace Heisenslaught.DataTransfer
{
    public class DraftStateDTO
    {
        public DraftStatePhase phase;
        public bool team1Ready;
        public bool team2Ready;
        public int pickTime;
        public int team1BonusTime;
        public int team2BonusTime;
        public List<string> picks;
        public int connectionCount;

        public DraftStateDTO(DraftStateModel model)
        {
            phase = model.phase;
            team1Ready = model.team1Ready;
            team2Ready = model.team2Ready;
            pickTime = model.pickTime;
            team1BonusTime = model.team1BonusTime;
            team2BonusTime = model.team2BonusTime;
            picks = model.picks;
        }

        public DraftStateDTO(DraftRoom room) : this(room.DraftModel.state)
        {
            connectionCount = room.ConnectionCount;
        }

    }
}
