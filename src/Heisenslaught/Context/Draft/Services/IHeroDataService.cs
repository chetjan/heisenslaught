using System.Collections.Generic;

namespace Heisenslaught.Services
{
    public interface IHeroDataService
    {
        HeroData GetHeroById(string heroId);
        List<HeroData> GetHeroes();
        MapData GetMapById(string mapId);
        List<MapData> GetMaps();
    }
}