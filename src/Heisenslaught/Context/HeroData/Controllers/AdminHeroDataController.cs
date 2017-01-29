using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Heisenslaught.HeroData
{
    [Route("api/admin/herodata")]
    [Authorize]
    public class AdminHeroDataController : HeroDataController
    {
        private readonly IHeroDataService heroDataService;
        public AdminHeroDataController(ImageStore imageStore, IHeroDataService heroDataService) : base(imageStore)
        {
            this.heroDataService = heroDataService;
        }

        [HttpGet("heroes/{id}/image/import")]
        public async Task ImportHeroImageAsync(string id, string url)
        {
            await ImportImageAsync("hero", id, url);
        }


        [HttpGet("heroes/images/report")]
        public List<object> GetHeroImageImportReport()
        {
            var heroes = heroDataService.GetHeroes();
            var images = (from image in imageStore.QueryableCollection
                          where image.Id.type == "hero"
                          select new
                          {
                              id = image.Id.id,
                              url = image.originalUrl
                          }).ToList();
            var results = from hero in heroes
                          where !images.Contains(new
                          {
                              id = hero.id,
                              url = hero.iconSmall
                          })
                          select new {
                              id = hero.id,
                              name = hero.name,
                              url = hero.iconSmall,
                             
                          };
            return results.ToList<object>();
        }

        [HttpGet("maps/{id}/image/import")]
        public async Task ImportMapImageAsync(string id, string url)
        {
            await ImportImageAsync("map", id, url);
        }

      


        private async Task ImportImageAsync(string type, string id, string url)
        {
            HttpClient http = new HttpClient();
            var res = await http.GetAsync(url);
            var imageData = "data:image/jpg;base64," + Convert.ToBase64String(await res.Content.ReadAsByteArrayAsync());
            var imageModel = new ImageModel();
            imageModel.Id = new ImageKey(type, id);
            imageModel.image = imageData;
            imageModel.originalUrl = url;
            imageStore.CreateOrUpdate(imageModel);
        }



    }
}
