using MongoDB.Driver;
using System.Linq;

namespace Heisenslaught.Infrastructure.MongoDb
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
