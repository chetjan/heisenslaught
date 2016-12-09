using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Heisenslaught.Infrastructure
{
    public enum Team
    {
        Team0,
        Team1
    }
    public enum DraftPhase
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
        public string CurrentPhase
        {
            get
            {
                return Phase.ToString();
            }
        }
        public Team CurrentTeam
        {
            get
            {
                if (Team0Phases.Contains(Phase))
                {
                    return Team.Team0;
                }
                else 
                {
                    return Team.Team1;
                }
            }
        }
        public int TimeTeam0 = 180, TimeTeam1 = 180, TimeBonus = 30;
        public List<string> 
            BansTeam0 = new List<string>(), 
            PicksTeam0 = new List<string>(), 
            BansTeam1 = new List<string>(),
            PicksTeam1 = new List<string>();

        private DraftPhase Phase = DraftPhase.Waiting;

        private Timer Ticker;

        private List<DraftPhase> Team0Phases = new List<DraftPhase> {
            DraftPhase.FirstBanTeam0,
            DraftPhase.FirstPickTeam0,
            DraftPhase.SecondPickTeam0,
            DraftPhase.ThirdPickTeam0,
            DraftPhase.SecondBanTeam0,
            DraftPhase.FourthPickTeam0,
            DraftPhase.FifthPickTeam0
        };
        private List<DraftPhase> Team1Phases = new List<DraftPhase> {
            DraftPhase.FirstBanTeam1,
            DraftPhase.FirstPickTeam1,
            DraftPhase.SecondPickTeam1,
            DraftPhase.ThirdPickTeam1,
            DraftPhase.SecondBanTeam1,
            DraftPhase.FourthPickTeam1,
            DraftPhase.FifthPickTeam1
        };
        private List<DraftPhase> BanPhases = new List<DraftPhase>
        {
            DraftPhase.FirstBanTeam0,
            DraftPhase.FirstBanTeam1,
            DraftPhase.SecondBanTeam0,
            DraftPhase.SecondBanTeam1
        };

        public Draft()
        {
            Ticker = new Timer(Tick, null, 0, 1000);
        }

        public void StartDraft() 
        {
            if (DraftPhase.Waiting == Phase)
            {
                Phase++;
            }
        }

        public bool SelectHero(Team team, string hero)
        {
            if (CurrentTeam != team) 
            {
                return false;
            }

            if (Team.Team0 == team) 
            {
                if (BanPhases.Contains(Phase))
                {
                    BansTeam0.Add(hero);
                }
                else
                {
                    PicksTeam0.Add(hero);
                }
            }
            else
            {
                if (BanPhases.Contains(Phase))
                {
                    BansTeam1.Add(hero);
                }
                else
                {
                    PicksTeam1.Add(hero);
                }
            }
            NextPhase();
            return true;
        }

        private void NextPhase()
        {
            if (Phase != DraftPhase.Finished)
            {
                Phase++;
                if (Team0Phases.Contains(Phase))
                {
                    TimeTeam0 += TimeBonus;
                }
                else if (Team1Phases.Contains(Phase))
                {
                    TimeTeam1 += TimeBonus;
                }
            }
        }

        private void Tick(object timerPhase)
        {
            if (Team0Phases.Contains(Phase))
            {
                TimeTeam0--;
            }
            else if (Team1Phases.Contains(Phase))
            {
                TimeTeam1--;
            }
        }
    }
}
