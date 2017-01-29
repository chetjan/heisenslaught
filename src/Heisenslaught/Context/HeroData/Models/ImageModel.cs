using Heisenslaught.Infrastructure.MongoDb;
using MongoDB.Bson.Serialization.Attributes;

namespace Heisenslaught.HeroData
{

    public class ImageKey {
        public string type;
        public string id;

        public ImageKey() { }
        public ImageKey(string type, string id) {
            this.type = type;
            this.id = id;
        }

    }
    public class ImageModel : IMongoDocument<ImageKey>
    {
        [BsonId]
        public ImageKey Id { get; set; }
        public string image;
        public string originalUrl;
    }
}
