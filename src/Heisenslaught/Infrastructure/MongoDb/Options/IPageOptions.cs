using MongoDB.Driver;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Heisenslaught.Infrastructure.MongoDb
{
    public interface IPageOptions<TPagable, TDocument>
    {
        int Page { get; set; }
        int PageSize { get; set; }
        PagedResult<TDocument> ApplyPagination(TPagable pagable);
    }

    public interface IMongoPageOptions<TDocument> : IPageOptions<IFindFluent<TDocument, TDocument>, TDocument>
    {
        Task<PagedResult<TDocument>> ApplyPaginationAsync(IFindFluent<TDocument, TDocument> pagable, CancellationToken cancellationToken);
    }

    public interface ILinqPageOptions<TDocument> : IPageOptions<IQueryable<TDocument>, TDocument>
    {

    }
}
