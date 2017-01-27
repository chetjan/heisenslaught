using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Heisenslaught.Infrastructure.MongoDb
{
    public class QueryUtil
    {

        public static PagedResult<TDocument> Apply<TDocument>(IQueryable<TDocument> query, string filter, string sort, int page, int pageSize)
        {
            var q = Apply(query, filter, sort);
            return q.PageResult(page, pageSize);
        }

        public static IQueryable<TDocument> Apply<TDocument>(IQueryable<TDocument> query, string filter=null, string sort=null)
        {
            var q = query;
            if (filter != null && filter != "")
            {
                q = q.Where(filter);
            }

            if (sort != null && sort != "")
            {
                q = q.OrderBy(sort);
            }
            return q;
        }
    }
}
