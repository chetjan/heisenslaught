using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heisenslaught.Infrastructure
{
    public class TeamDraftConfig : DraftConfig
    {
        public int team;

        public TeamDraftConfig(int team, DraftConfig cfg)
        {
            this.team = team;
            team1Name = cfg.team1Name;
            team2Name = cfg.team2Name;
            firstPick = cfg.firstPick;
            pickTime = cfg.pickTime;
            bonusTime = cfg.bonusTime;
            bankTime = cfg.bankTime;
            map = cfg.map;
            disabledHeroes = cfg.disabledHeroes;
            state = cfg.state;

        }
    }
}
