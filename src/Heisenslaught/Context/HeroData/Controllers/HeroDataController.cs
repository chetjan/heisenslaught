using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Heisenslaught.HeroData
{
    [Route("api/herodata")]
    public class HeroDataController : Controller
    {
    
        protected readonly ImageStore imageStore;

        public HeroDataController(
            ImageStore imageStore
        ) {
            this.imageStore = imageStore;
        }

        [HttpGet("heroes/images")]
        public Dictionary<string, string> GetHeroImages()
        {
           return (from image in imageStore.QueryableCollection
                         where image.Id.type == "hero"
                         select image).ToDictionary(i => i.Id.id, i => i.image);
        }

        [HttpGet("heroes/{id}/image")]
        public ImageModel GetHeroImage(string id)
        {
            return imageStore.FindById(new ImageKey("hero", id));
        }


        [HttpGet("maps/images")]
        public Dictionary<string, string> GetMapImages()
        {
            return (from image in imageStore.QueryableCollection
                    where image.Id.type == "map"
                    select image).ToDictionary(i => i.Id.id, i => i.image);
        }

        [HttpGet("maps/{id}/image")]
        public ImageModel GetMapImage(string id)
        {
            return imageStore.FindById(new ImageKey("map", id));
        }

    }
}
