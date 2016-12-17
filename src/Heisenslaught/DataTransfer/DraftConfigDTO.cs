using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Heisenslaught.Models;

namespace Heisenslaught.DataTransfer
{
    public class DraftConfigDTO : CreateDraftDTO
    {
        public DraftStateDTO state;

        public DraftConfigDTO(DraftModel model) : base(model)
        {
            state = new DraftStateDTO(model.state);
        }
        
    }
}
