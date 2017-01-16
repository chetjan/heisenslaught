using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heisenslaught.Persistence.MongoDb.Models
{
    public interface IMongoDocument<TKey>
    {
        TKey Id { get; set; }
    }
}
