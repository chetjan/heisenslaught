namespace Heisenslaught.Infrastructure.MongoDb
{
    public interface IMongoDocument<TKey>
    {
        TKey Id { get; set; }
    }
}
