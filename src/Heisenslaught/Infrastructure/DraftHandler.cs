using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

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

        private Timer timer;
        private AdminDraftConfig config;
        private List<List<int>> teamSlots = new List<List<int>>();
        private List<List<int>> teamPickSlots = new List<List<int>>();
        private List<List<int>> teamBanSlots = new List<List<int>>();

        private List<string> teamTokens = new List<string>();

        public DraftHandler(AdminDraftConfig config)
        {
            this.config = config;
            if(state.picks == null)
            {
                state.picks = new List<string>();
            }
            if(config.firstPick == 1)
            {
                teamSlots.Add(firstSlots);
                teamSlots.Add(secondSlots);
                teamPickSlots.Add(firstPickSlots);
                teamPickSlots.Add(secondPickSlots);
                teamBanSlots.Add(firstBanSlots);
                teamBanSlots.Add(secondBanSlots);
                teamTokens.Add(config.team1DrafterToken);
                teamTokens.Add(config.team2DrafterToken);
            }
            else
            {
                teamSlots.Add(secondSlots);
                teamSlots.Add(firstSlots);
                teamPickSlots.Add(secondPickSlots);
                teamPickSlots.Add(firstPickSlots);
                teamBanSlots.Add(secondBanSlots);
                teamBanSlots.Add(firstBanSlots);
                teamTokens.Add(config.team2DrafterToken);
                teamTokens.Add(config.team1DrafterToken);
            }

        }

        public DraftState state
        {
            get
            {
                return this.config.state;
            }
        }

        public int currentPick
        {
            get
            {
                if(state.phase == DraftStatePhase.PICKING)
                {
                    return state.picks.Count;
                }
                return -1;
            }
        }

        public int currentTeam
        {
            get
            {
                if (currentPick != -1)
                {
                    return firstSlots.Contains(currentPick) ? 0 : 1;
                }
                return -1;
            }
        }

        public bool isCurrentTeamToken(string token)
        {
            if (currentTeam != -1)
            {
                return teamTokens[currentTeam] == token;
            }
            return false;
        }

        public bool isBan
        {
            get
            {
                if (currentTeam != -1)
                {
                    return teamBanSlots[currentTeam].Contains(currentPick);
                }
                return false;
            }
        }

        public bool isPick
        {
            get
            {
                if (currentTeam != -1)
                {
                    return teamPickSlots[currentTeam].Contains(currentPick);
                }
                return false;
            }
        }

        public bool isFirstPickOfDoublePick
        {
            get
            {
                if (isPick)
                {
                    var pickSlots = teamPickSlots[currentTeam];
                    var idx = pickSlots.IndexOf(currentPick);
                    return idx != -1 && pickSlots.Contains(currentPick + 1);
                }
                return false;
            }
        }

        public bool isValidHero(string heroId)
        {
            var hero = HeroDataService.getHeroById(heroId);
            return hero != null;
        }

        public bool isHeroPickedOrBanned(string heroId)
        {
            return state.picks.Contains(heroId);
        }

        public bool isHeroDisabled(string heroId)
        {
            return config.disabledHeroes != null && config.disabledHeroes.Contains(heroId);
        }

        public bool canPickHero(string heroId)
        {
        
            var allowed = isValidHero(heroId) && !isHeroPickedOrBanned(heroId) && !isHeroDisabled(heroId);
            if (allowed && (heroId == "cho" || heroId == "gall"))
            {
                
                if (!isFirstPickOfDoublePick && !isBan)
                {
                    allowed = false;
                }
                else
                {
                    var other = heroId == "cho" ? "gall" : "cho";
                    allowed = !isHeroPickedOrBanned(other) && !isHeroDisabled(other);
                }
            }
            return allowed;
        }



        public void start()
        {
            if(timer == null)
            {
                timer = new Timer(tick, null, 0, 1000);
            }
            
        }

        public void stop()
        {
            if(timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }


        public void tick(object timerPhase)
        {
            if(state.phase == DraftStatePhase.WAITING)
            {
                if(state.team1Ready && state.team2Ready)
                {
                    state.phase = DraftStatePhase.PICKING;
                    state.pickTime = config.pickTime;
                    state.team1BonusTime = config.bonusTime;
                    state.team2BonusTime = config.bonusTime;
                }
            }
            else if(state.phase == DraftStatePhase.FINISHED)
            {
                stop();
            }
            else if(state.phase == DraftStatePhase.PICKING)
            {
                if(state.picks.Count >= 14)
                {
                    state.phase = DraftStatePhase.FINISHED;
                }
                else if (state.pickTime > -1)
                {
                    --state.pickTime;
                }
                else
                {
                    var timeLeft = 100;
                    if(currentTeam == 0)
                    {
                        timeLeft = --state.team1BonusTime;
                    }
                    else if(currentTeam == 1)
                    {
                        timeLeft = --state.team2BonusTime;
                    }
                    if(timeLeft < 0)
                    {
                        // pick random hero for team
                        if (isBan)
                        {
                            state.picks.Add("failed_ban");
                            state.pickTime = config.pickTime;
                        }
                        else
                        {
                            pickRandom();
                        }
                    }
                }
            }
        }

        public bool pickHero(string heroId, string teamToken, bool skipTokenValidation=false)
        {
            var team = currentTeam;
            if (team == -1 || (!skipTokenValidation && teamTokens[team] != teamToken) || !canPickHero(heroId))
            {
                return false;
            }

            state.picks.Add(heroId);

            if(!isBan && (heroId == "cho" || heroId == "gall"))
            {
                state.picks.Add(heroId == "cho" ? "gall" : "cho");
            }
            
            if(state.picks.Count >= 14)
            {
                state.phase = DraftStatePhase.FINISHED;
                stop();
            }
            else
            {
                if(config.bankTime && state.pickTime > 0)
                {
                    if((team == 0 && config.firstPick == 1) || (team == 1 && config.firstPick == 2))
                    {
                        state.team1BonusTime += state.pickTime;
                    }
                    else
                    {
                        state.team2BonusTime += state.pickTime;
                    }
                }
                state.pickTime = config.pickTime;
            }

            return true;
        }

        private void pickRandom()
        {
            string heroId = null;
            var heroes = HeroDataService.getHeroes();
            var max = heroes.Count - 1;
            while(heroId == null)
            {
                var rng = rnd.Next(0, max);
                var hero = heroes[rng];
                if(hero.id != "cho" && hero.id != "gall" && canPickHero(hero.id))
                {
                    heroId = hero.id;
                }
                pickHero(heroId, null, true);
            }
        }
    }
}
