using Heisenslaught.Infrastructure.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Heisenslaught.HeroData
{
    public class ImageStore : CrudMongoStore<ImageKey, ImageModel>
    {
        public ImageStore(IMongoDatabase database, string collectionName) : base(database, collectionName)
        {
        }

        public ImageStore(IMongoDatabase database) : base(database, "heroesdata_images")
        {
        }
    }
}
