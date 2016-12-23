using Heisenslaught.Models;
using Heisenslaught.Infrastructure;


namespace Heisenslaught.DataTransfer
{
    public class DraftConfigDTO : CreateDraftDTO
    {
        public DraftStateDTO state;

        public DraftConfigDTO(DraftModel model) : base(model)
        {
            state = new DraftStateDTO(model.state);
        }

        public DraftConfigDTO(DraftRoom room) : base(room.DraftModel)
        {
            state = new DraftStateDTO(room);
        }

    }
}
