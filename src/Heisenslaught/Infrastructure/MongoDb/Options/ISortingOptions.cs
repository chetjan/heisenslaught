using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heisenslaught.Persistence.MongoDb.Options
{
    public interface ISortingOptions<TSortable>
    {
        TSortable ApplySort(TSortable sortable);
    }

    public interface IMongoSortingOptions<TDocument> : ISortingOptions<IFindFluent<TDocument, TDocument>>
    {
       
    }

    public interface ILinqSortingOptions<TDocument> : ISortingOptions<IQueryable<TDocument>>
    {

    }
}
