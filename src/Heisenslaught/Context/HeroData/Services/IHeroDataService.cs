using System.Collections.Generic;

namespace Heisenslaught.HeroData
{
    public interface IHeroDataService
    {
        HeroData GetHeroById(string heroId);
        List<HeroData> GetHeroes();
        MapData GetMapById(string mapId);
        List<MapData> GetMaps();
    }
}