using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Heisenslaught.Users;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver.Linq;
using MongoDB.Bson;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Heisenslaught.Draft
{
    [Route("api/draft")]
    [Authorize]
    public class DraftController : Controller
    {
        private readonly DraftJoinedStore draftJoinedStore;
        private readonly IDraftStore draftStore;
        private readonly DraftListViewStore draftListViewStore;
        private readonly UserManager<HSUser> userMananger;

        public DraftController(
            IDraftStore draftStore,
            DraftJoinedStore draftJoinedStore,
            DraftListViewStore draftListViewStore,
            UserManager<HSUser> userMananger
        )
        {
            this.draftStore = draftStore;
            this.draftJoinedStore = draftJoinedStore;
            this.userMananger = userMananger;
            this.draftListViewStore = draftListViewStore;
        }

        [HttpGet("recent/joined")]
        public List<object> MyRecentJoined()
        {
            return GetJoinedDraftsQuery().Take(5).ToList<object>();
        }


        private IQueryable<object> GetJoinedDraftsQuery()
        {
            var userId = userMananger.GetUserId(User);
            var result = from joined in draftJoinedStore.QueryableCollection
                         where joined.user == userId && joined.joinedAs != DraftConnectionType.ADMIN
                         join draft in draftListViewStore.QueryableCollection on joined.draft equals draft.Id
                         select new
                         {
                             joinedOn = joined.joinedOn,
                             joinedAs = joined.joinedAs,
                             joindAsTeam1Drafter = joined.joinedAs == DraftConnectionType.DRAFTER_TEAM_1 ? 1 : 0,
                             joindAsTeam2Drafter = joined.joinedAs == DraftConnectionType.DRAFTER_TEAM_2 ? 1 : 0,
                             draft = draft
                         };
            var result2 = from joined in result
                          orderby joined.joinedOn descending
                          group joined by joined.draft.Id into draftJoins
                          orderby draftJoins.First().joinedOn descending
                          select new
                          {
                              joinedOn = draftJoins.First().joinedOn,
                              draftToken = draftJoins.First().draft.draftToken,
                              team1Name = draftJoins.First().draft.team1Name,
                              team2Name = draftJoins.First().draft.team2Name,
                              createdBy = draftJoins.First().draft.displayName,
                              map = draftJoins.First().draft.map,
                              isOwner = draftJoins.First().draft.userId == userId,
                              adminToken = draftJoins.First().draft.userId == userId
                                ? draftJoins.First().draft.adminToken : null,
                              team1DraftToken = draftJoins.Max(x => x.joindAsTeam1Drafter) == 1 ? draftJoins.First().draft.team1DrafterToken : null,
                              team2DraftToken = draftJoins.Max(x => x.joindAsTeam2Drafter) == 1 ? draftJoins.First().draft.team2DrafterToken : null
                          };

            return result2;
        }


        [HttpGet("recent/created")]
        public List<object> MyRecentCreated()
        {
            return GetCreatedDraftsQuery().Take(5).ToList<object>();
        }

        private IQueryable<object> GetCreatedDraftsQuery()
        {
            var userId = userMananger.GetUserId(User);
            var result = from draft in draftStore.QueryableCollection
                         where draft.createdBy == userId
                         orderby draft.Id descending
                         select new
                         {
                             id = draft.Id,
                             draftToken = draft.draftToken,
                             adminToken = draft.adminToken,
                             team1Name = draft.config.team1Name,
                             team2Name = draft.config.team2Name,
                             map = draft.config.map
                         };

            return result;
        }
    }
}
