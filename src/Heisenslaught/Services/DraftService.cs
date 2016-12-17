using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Heisenslaught.Infrastructure;
using Heisenslaught.Models;
using Heisenslaught.DataTransfer;
using Microsoft.AspNetCore.SignalR.Hubs;
using Heisenslaught.Persistence;


namespace Heisenslaught.Services
{


    public class DraftService
    {
        private static MongoDraftRepository DraftRepo = new MongoDraftRepository();

        private Dictionary<string, DraftRoom> activeRooms = new Dictionary<string, DraftRoom>();
        private DraftHub hub;

        public DraftService(DraftHub hub)
        {
            this.hub = hub;
        }

        public DraftConfigDTO getDraftConfig(string draftToken, string authToken = null)
        {
            return null;
        }

        public List<DraftConfigAdminDTO> getActiveDrafts()
        {
            return null;
        }

        public List<DraftConfigAdminDTO> getDrafts(int start = 0, int limit = 0)
        {
            return null;
        }

        public DraftConfigAdminDTO createDraft(CreateDraftDTO config)
        {
            var model = new DraftModel(config.ToModel());
            DraftRepo.createDraft(model);
            return new DraftConfigAdminDTO(model);
        }


        public DraftConfigDTO connectToDraft(IHubCallerConnectionContext<dynamic> caller, string draftToken, string authToken = null)
        {
            return null;
        }

        public DraftConfigAdminDTO resetDraft(string draftToken, string admintoken)
        {
            return null;
        }

        public bool closeDraft(string draftToken, string admintoken)
        {
            return false;
        }

        public bool setReady(string draftToken, string teamToken)
        {
            return false;
        }

        public bool pickHero(string heroId, string draftToken, string teamToken)
        {
            return false;
        }
    }
}
