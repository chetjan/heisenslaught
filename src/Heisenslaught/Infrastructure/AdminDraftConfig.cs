using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Heisenslaught.Infrastructure
{

    public class AdminDraftConfig
    {

        private static Random rnd = new Random((int)DateTime.Now.Ticks);
        private DraftConfig _config;
        private bool _isRandomFirstPick = false;

        public string draftToken;
        public string team1DrafterToken;
        public string team2DrafterToken;





        public AdminDraftConfig(DraftConfig cfg)
        {
            _config = cfg;
            if (cfg.firstPick != 1 && cfg.firstPick != 2)
            {
                int rng = rnd.Next(1, 10000);
                cfg.firstPick = (rng % 2) + 1;
                _isRandomFirstPick = true;
            }

            draftToken = generateToken();
            team1DrafterToken = generateToken();
            team2DrafterToken = generateToken();

            reset();
        }


        private string generateToken()
        {

            string token = null;
            while(token == null || token.IndexOfAny(new char[] { '/', '+' }) != -1)
            {
                token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            }    
            
            return token.TrimEnd('=');
        }

        public void reset()
        {
            _config.state = new DraftState();
            _config.state.pickTime = _config.pickTime;
            _config.state.team1BonusTime = _config.bonusTime;
            _config.state.team2BonusTime = _config.bonusTime;
        }

        public DraftConfig getConfig()
        {
            return _config;   
        }

        public bool randomFirstPick
        {
            get
            {
                return _isRandomFirstPick;
            }
        }

        public int firstPick
        {
            get
            {
                return _config.firstPick;
            }

            set
            {
                _config.firstPick = value;
            }

        }

        public int pickTime
        {
            get
            {
                return _config.pickTime;
            }

            set
            {
                _config.pickTime = value;
            }

        }


        public int bonusTime
        {
            get
            {
                return _config.bonusTime;
            }

            set
            {
                _config.bonusTime = value;
            }

        }


        public bool bankTime
        {
            get
            {
                return _config.bankTime;
            }

            set
            {
                _config.bankTime = value;
            }

        }


        public string team1Name
        {
            get
            {
                return _config.team1Name;
            }

            set
            {
                _config.team1Name = value;
            }

        }


        public string team2Name
        {
            get
            {
                return _config.team2Name;
            }

            set
            {
                _config.team2Name = value;
            }

        }


        public string map
        {
            get
            {
                return _config.map;
            }

            set
            {
                _config.map = value;
            }

        }


        public List<string> disabledHeroes
        {
            get
            {
                return _config.disabledHeroes;
            }

            set
            {
                _config.disabledHeroes = value;
            }

        }

        public DraftState state
        {
            get
            {
                return _config.state;
            }

            set
            {
                _config.state = value;
            }

        }

    }
}
