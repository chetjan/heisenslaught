using Heisenslaught.Models.Draft;
using Heisenslaught.Persistence.MongoDb.Store;
using System;
using System.Collections.Generic;


namespace Heisenslaught.Persistence.Draft
{
    public interface IDraftStore : ICrudMongoStore<string, DraftModel>
    {
        DraftModel FindByDraftToken(string draftToken);
        List<DraftModel> FindByUserId(string userId);
    }
}