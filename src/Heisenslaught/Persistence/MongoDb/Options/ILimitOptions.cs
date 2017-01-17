using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heisenslaught.Persistence.MongoDb.Options
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
