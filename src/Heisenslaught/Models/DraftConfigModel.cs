using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heisenslaught.Models
{
    public class DraftConfigModel
    {
        public int firstPick;
        public int pickTime;
        public int bonusTime;
        public bool bankTime;
        public string team1Name;
        public string team2Name;
        public string map;
        public List<string> disabledHeroes;

    }
}
