using MongoDB.Driver;
using System.Linq;

namespace Heisenslaught.Infrastructure.MongoDb
{
    public interface ILimitOptions<TLimitable>
    {
        int Skip { get; set; }
        int Limit { get; set; }
        TLimitable ApplyLimits(TLimitable limitable);
    }

    public interface IMongoLimitOptions<TDocument> : ILimitOptions<IFindFluent<TDocument, TDocument>>
    {

    }

    public interface ILinqLimitOptions<TDocument> : ILimitOptions<IQueryable<TDocument>>
    {

    }
}
