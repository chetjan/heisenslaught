using System.Collections.Generic;

namespace Heisenslaught.Draft
{
    public enum DraftStatePhase
    {
        WAITING,
        PICKING,
        FINISHED
    }

    public class DraftStateModel
    {
        public DraftStatePhase phase = DraftStatePhase.WAITING;
        public bool team1Ready = false;
        public bool team2Ready = false;
        public int pickTime = 0;
        public int team1BonusTime = 0;
        public int team2BonusTime = 0;
        public List<string> picks = new List<string>();
    }
}
