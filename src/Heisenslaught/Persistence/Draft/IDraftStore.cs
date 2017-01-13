using Heisenslaught.Models;
using MongoDB.Driver;

namespace Heisenslaught.Persistence.Draft
{
    public interface IDraftStore
    {
        void CreateDraft(DraftModel draft);
        DraftModel FindByDraftToken(string draftToken);
        DraftModel FindById(string id);
        DraftModel FindByUserId(string userId);
        ReplaceOneResult SaveDraft(DraftModel draft);
    }
}