using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
namespace Heisenslaught.Infrastructure
{

    public class HeroData
    {
        public string id;
        public string name;
        public List<string> roles;
        public List<string> keywords;
        public string iconSmall;
        public string iconLarge;
    }

    public class MapData
    {
        public string id;
        public string name;
    }

    public class HeroDataService
    {
        public static List<HeroData> heroesData;
        public static List<MapData> mapsData;

        public static List<HeroData> getHeroes()
        {
            if(heroesData == null)
            {
                heroesData = readJsonFile<List<HeroData>>(Directory.GetCurrentDirectory() + "/wwwroot/data/heroes.json");
            }
            return heroesData;
        }

        public static List<MapData> getMaps()
        {
            if (heroesData == null)
            {
                mapsData = readJsonFile<List<MapData>>(Directory.GetCurrentDirectory() + "/wwwroot/data/maps.json");
            }
            return mapsData;
        }

        public static HeroData getHeroById(string heroId)
        {
            var heroes = getHeroes();
            return heroes.Find(hero =>
            {
                return hero.id == heroId;
            });
        }

        public static MapData getMapById(string mapId)
        {
            var maps = getMaps();
            return maps.Find(map =>
            {
                return map.id == mapId;
            });
        }

        public static T readJsonFile<T>(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader r = new StreamReader(fs);
            return JsonConvert.DeserializeObject<T>(r.ReadToEnd());
        }
    }
}
