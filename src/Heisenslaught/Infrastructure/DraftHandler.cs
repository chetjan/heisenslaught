using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Heisenslaught.Models;

namespace Heisenslaught.Infrastructure
{
    public class DraftHandler
    {
        private static Random rnd = new Random((int)DateTime.Now.Ticks);
        private static List<int> firstSlots= new List<int>{0,2,5,6,8,11,12};
        private static List<int> secondSlots = new List<int> {1,2,3,7,9,10,13};
        private static List<int> firstPickSlots = new List<int> {2,5,6,11,12};
        private static List<int> secondPickSlots = new List<int> {3,4,9,10,13};
        private static List<int> firstBanSlots = new List<int> {0,8};
        private static List<int> secondBanSlots = new List<int> {1,7};

        private List<List<int>> teamSlots = new List<List<int>>();
        private List<List<int>> teamPickSlots = new List<List<int>>();
        private List<List<int>> teamBanSlots = new List<List<int>>();
        private List<string> teamTokens = new List<string>();
        private DraftModel model;


        public DraftHandler(DraftRoom draftRoom)
        {
            this.model = draftRoom.DraftModel;
            if(State.picks == null)
            {
                State.picks = new List<string>();
            }
            if(Config.firstPick == 1)
            {
                teamSlots.Add(firstSlots);
                teamSlots.Add(secondSlots);
                teamPickSlots.Add(firstPickSlots);
                teamPickSlots.Add(secondPickSlots);
                teamBanSlots.Add(firstBanSlots);
                teamBanSlots.Add(secondBanSlots);
                teamTokens.Add(model.team1DrafterToken);
                teamTokens.Add(model.team2DrafterToken);
            }
            else
            {
                teamSlots.Add(secondSlots);
                teamSlots.Add(firstSlots);
                teamPickSlots.Add(secondPickSlots);
                teamPickSlots.Add(firstPickSlots);
                teamBanSlots.Add(secondBanSlots);
                teamBanSlots.Add(firstBanSlots);
                teamTokens.Add(model.team2DrafterToken);
                teamTokens.Add(model.team1DrafterToken);
            }
        }

        private DraftStateModel State
        {
            get
            {
                return this.model.state;
            }
        }

        private DraftConfigModel Config
        {
            get
            {
                return this.model.config;
            }
        }

        private int CurrentPick
        {
            get
            {
                if(State.phase == DraftStatePhase.PICKING)
                {
                    return State.picks.Count;
                }
                return -1;
            }
        }

        private int CurrentTeam
        {
            get
            {
                if (CurrentPick != -1)
                {
                    return firstSlots.Contains(CurrentPick) ? 0 : 1;
                }
                return -1;
            }
        }

        private bool IsCurrentTeamToken(string token)
        {
            if (CurrentTeam != -1)
            {
                return teamTokens[CurrentTeam] == token;
            }
            return false;
        }

        private bool IsBan
        {
            get
            {
                if (CurrentTeam != -1)
                {
                    return teamBanSlots[CurrentTeam].Contains(CurrentPick);
                }
                return false;
            }
        }

        private bool IsPick
        {
            get
            {
                if (CurrentTeam != -1)
                {
                    return teamPickSlots[CurrentTeam].Contains(CurrentPick);
                }
                return false;
            }
        }

        private bool IsFirstPickOfDoublePick
        {
            get
            {
                if (IsPick)
                {
                    var pickSlots = teamPickSlots[CurrentTeam];
                    var idx = pickSlots.IndexOf(CurrentPick);
                    return idx != -1 && pickSlots.Contains(CurrentPick + 1);
                }
                return false;
            }
        }

        private bool IsValidHero(string heroId)
        {
            var hero = HeroDataService.GetHeroById(heroId);
            return hero != null;
        }

        private bool IsHeroPickedOrBanned(string heroId)
        {
            return State.picks.Contains(heroId);
        }

        private bool IsHeroDisabled(string heroId)
        {
            return Config.disabledHeroes != null && Config.disabledHeroes.Contains(heroId);
        }

        private bool CanPickHero(string heroId)
        {
            var allowed = IsValidHero(heroId) && !IsHeroPickedOrBanned(heroId) && !IsHeroDisabled(heroId);
            if (allowed && (heroId == "cho" || heroId == "gall"))
            {
                if (!IsFirstPickOfDoublePick && !IsBan)
                {
                    allowed = false;
                }
                else
                {
                    var other = heroId == "cho" ? "gall" : "cho";
                    allowed = !IsHeroPickedOrBanned(other) && !IsHeroDisabled(other);
                }
            }
            return allowed;
        }

        public bool Tick()
        {
            if(State.phase == DraftStatePhase.WAITING)
            {
                if(State.team1Ready && State.team2Ready)
                {
                    State.phase = DraftStatePhase.PICKING;
                    State.pickTime = Config.pickTime;
                    State.team1BonusTime = Config.bonusTime;
                    State.team2BonusTime = Config.bonusTime;
                    return true;
                }
            }
            else if(State.phase == DraftStatePhase.PICKING)
            {
                if(State.picks.Count >= 14)
                {
                    State.phase = DraftStatePhase.FINISHED;
                    return true;
                }
                else if (State.pickTime > -1)
                {
                    --State.pickTime;
                    return true;
                }
                else
                {
                    var timeLeft = 100;
                    if(CurrentTeam == 0)
                    {
                        timeLeft = --State.team1BonusTime;
                    }
                    else if(CurrentTeam == 1)
                    {
                        timeLeft = --State.team2BonusTime;
                    }
                    if(timeLeft < 0)
                    {
                        // pick random hero for team
                        if (IsBan)
                        {
                            State.picks.Add("failed_ban");
                            State.pickTime = Config.pickTime;
                        }
                        else
                        {
                            PickRandomHero();
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public bool PickHero(string heroId, string teamToken, bool skipTokenValidation=false)
        {
            var team = CurrentTeam;
            if (team == -1 || (!skipTokenValidation && teamTokens[team] != teamToken) || !CanPickHero(heroId))
            {
                return false;
            }

            State.picks.Add(heroId);

            if(!IsBan && (heroId == "cho" || heroId == "gall"))
            {
                State.picks.Add(heroId == "cho" ? "gall" : "cho");
            }
            
            if(State.picks.Count >= 14)
            {
                State.phase = DraftStatePhase.FINISHED;
            }
            else
            {
                if(Config.bankTime && State.pickTime > 0)
                {
                    if((team == 0 && Config.firstPick == 1) || (team == 1 && Config.firstPick == 2))
                    {
                        State.team1BonusTime += State.pickTime;
                    }
                    else
                    {
                        State.team2BonusTime += State.pickTime;
                    }
                }
                State.pickTime = Config.pickTime;
            }
            return true;
        }

        private void PickRandomHero()
        {
            string heroId = null;
            var heroes = HeroDataService.GetHeroes();
            var max = heroes.Count - 1;
            while(heroId == null)
            {
                var rng = rnd.Next(0, max);
                var hero = heroes[rng];
                if(hero.id != "cho" && hero.id != "gall" && CanPickHero(hero.id))
                {
                    heroId = hero.id;
                }
                PickHero(heroId, null, true);
            }
        }
    }
}
