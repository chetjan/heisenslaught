using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Heisenslaught.Infrastructure
{
    public enum DraftState
    {
        Waiting,
        FirstBanTeam0,
        FirstBanTeam1,
        FirstPickTeam0,
        FirstPickTeam1,
        SecondPickTeam1,
        SecondPickTeam0,
        ThirdPickTeam0,
        SecondBanTeam1,
        SecondBanTeam0,
        ThirdPickTeam1,
        FourthPickTeam1,
        FourthPickTeam0,
        FifthPickTeam0,
        FifthPickTeam1,
        Finished
    }

    public class Draft
    {
        private DraftState State = DraftState.Waiting;
        public string CurrentState
        {
            get
            {
                return State.ToString();
            }
        }
        public int TimeTeam0 = 180, TimeTeam1 = 180, TimeBonus = 30;
        public string CurrentAction
        {
            get
            {
                if (BanStates.Contains(State))
                {
                    return "Banned: ";
                }
                else
                {
                    return "Picked: ";
                }
            }
        }

        private Timer Ticker;

        private List<DraftState> Team0States = new List<DraftState> {
            DraftState.FirstBanTeam0,
            DraftState.FirstPickTeam0,
            DraftState.SecondPickTeam0,
            DraftState.ThirdPickTeam0,
            DraftState.SecondBanTeam0,
            DraftState.FourthPickTeam0,
            DraftState.FifthPickTeam0
        };
        private List<DraftState> Team1States = new List<DraftState> {
            DraftState.FirstBanTeam1,
            DraftState.FirstPickTeam1,
            DraftState.SecondPickTeam1,
            DraftState.ThirdPickTeam1,
            DraftState.SecondBanTeam1,
            DraftState.FourthPickTeam1,
            DraftState.FifthPickTeam1
        };
        private List<DraftState> BanStates = new List<DraftState>
        {
            DraftState.FirstBanTeam0,
            DraftState.FirstBanTeam1,
            DraftState.SecondBanTeam0,
            DraftState.SecondBanTeam1
        };

        public Draft()
        {
            Ticker = new Timer(Tick, null, 0, 1000);
        }

        public void NextState()
        {
            if (State != DraftState.Finished)
            {
                State++;
                if (Team0States.Contains(State))
                {
                    TimeTeam0 += TimeBonus;
                }
                else if (Team1States.Contains(State))
                {
                    TimeTeam1 += TimeBonus;
                }
            }
        }

        private void Tick(object timerState)
        {
            if (Team0States.Contains(State))
            {
                TimeTeam0--;
            }
            else if (Team1States.Contains(State))
            {
                TimeTeam1--;
            }
        }
    }
}
