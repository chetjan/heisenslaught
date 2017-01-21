using Heisenslaught.Infrastructure.MongoDb;
using System.Collections.Generic;


namespace Heisenslaught.Draft
{
    public interface IDraftStore : ICrudMongoStore<string, DraftModel>
    {
        DraftModel FindByDraftToken(string draftToken);
        List<DraftModel> FindByUserId(string userId);
    }
}