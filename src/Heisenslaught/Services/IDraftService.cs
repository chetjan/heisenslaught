using Heisenslaught.DataTransfer;
using Heisenslaught.Infrastructure;

namespace Heisenslaught.Services
{
    public interface IDraftService
    {
        void ClientDisconnected(DraftHub hub);
        void CompleteDraft(DraftRoom room);
        DraftConfigDTO ConnectToDraft(DraftHub hub, string draftToken, string authToken = null);
        DraftConfigAdminDTO CreateDraft(CreateDraftDTO config);
        DraftRoom GetDraftRoom(string draftToken, bool autoCreate = false);
    }
}