using System.Collections.Generic;
using System.Threading.Tasks;


namespace Heisenslaught.Draft
{
    public interface IDraftService
    {
        List<DraftRoom> ActiveRooms { get; }
        void ClientDisconnected(DraftHub hub);
        void CompleteDraft(DraftRoom room);
        Task<DraftConfigDTO> ConnectToDraftAsync(DraftHub hub, string draftToken, string authToken = null);
        Task<DraftConfigAdminDTO> CreateDraftAsync(CreateDraftDTO config, DraftHub hub);
        DraftRoom GetDraftRoom(string draftToken, bool autoCreate = false);
    }
}