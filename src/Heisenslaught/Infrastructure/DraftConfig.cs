using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Heisenslaught.Infrastructure
{
    public class DraftConfig
    {
        private static Random rnd = new Random((int) DateTime.Now.Ticks);

        private int _firstPick;

        public int pickTime;
        public int bonusTime;
        public bool bankTime;
        public string team1Name;
        public string team2Name;
        public string mapName;
        public List<string> disabledHeroes;


        public DraftConfig() {
            
        }

        public int firstPick
        {
            get
            {
                return _firstPick;
            }

            set 
            {
                if(value != 1 && value != 2)
                {
                    int rng = rnd.Next(1, 10000);
                    _firstPick = (rng % 2) + 1;
                }
                else
                {
                    _firstPick = value;
                }
            }

        }

    }
}
