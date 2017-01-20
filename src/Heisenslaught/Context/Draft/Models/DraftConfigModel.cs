using System.Collections.Generic;

namespace Heisenslaught.Models.Draft
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
