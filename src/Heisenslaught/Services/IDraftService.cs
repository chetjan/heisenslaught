using Heisenslaught.DataTransfer;
using Heisenslaught.Infrastructure;
using System.Threading.Tasks;

namespace Heisenslaught.Services
{
    public interface IDraftService
    {
        void ClientDisconnected(DraftHub hub);
        void CompleteDraft(DraftRoom room);
        DraftConfigDTO ConnectToDraft(DraftHub hub, string draftToken, string authToken = null);
        Task<DraftConfigAdminDTO> CreateDraft(CreateDraftDTO config, DraftHub hub);
        DraftRoom GetDraftRoom(string draftToken, bool autoCreate = false);
    }
}