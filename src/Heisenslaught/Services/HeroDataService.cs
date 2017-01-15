using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;


namespace Heisenslaught.Services
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

    public class HeroDataService : IHeroDataService
    {
        public List<HeroData> heroesData;
        public List<MapData> mapsData;

        public List<HeroData> GetHeroes()
        {
            if(heroesData == null)
            {
                heroesData = ReadJsonFile<List<HeroData>>(Directory.GetCurrentDirectory() + "/wwwroot/data/heroes.json");
            }
            return heroesData;
        }

        public List<MapData> GetMaps()
        {
            if (heroesData == null)
            {
                mapsData = ReadJsonFile<List<MapData>>(Directory.GetCurrentDirectory() + "/wwwroot/data/maps.json");
            }
            return mapsData;
        }

        public HeroData GetHeroById(string heroId)
        {
            var heroes = GetHeroes();
            return heroes.Find(hero =>
            {
                return hero.id == heroId;
            });
        }

        public MapData GetMapById(string mapId)
        {
            var maps = GetMaps();
            return maps.Find(map =>
            {
                return map.id == mapId;
            });
        }

        public T ReadJsonFile<T>(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader r = new StreamReader(fs);
            var result = JsonConvert.DeserializeObject<T>(r.ReadToEnd());
            r.Dispose();
            fs.Dispose();
            return result; 
        }
    }
}
