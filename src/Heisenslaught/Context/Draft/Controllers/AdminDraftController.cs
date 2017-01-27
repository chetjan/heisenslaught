using Heisenslaught.Infrastructure.Hubs;
using Heisenslaught.Infrastructure.MongoDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Heisenslaught.Draft
{
    [Route("api/admin/draft")]
    [Authorize]
    public class AdminDraftController : Controller
    {
        private readonly IDraftStore draftStore;
        private readonly DraftListViewStore draftListViewStore;
        private readonly IDraftService draftService;
        private readonly DraftJoinedStore draftJoinedStore;
        private readonly IHubConnectionsService hubConnectionsService;
        public AdminDraftController(
            IDraftStore draftStore,
            DraftListViewStore draftListViewStore,
            IDraftService draftService,
            DraftJoinedStore draftJoinedStore,
            IHubConnectionsService hubConnectionsService
        ){
            this.draftStore = draftStore;
            this.draftListViewStore = draftListViewStore;
            this.draftService = draftService;
            this.hubConnectionsService = hubConnectionsService;
            this.draftJoinedStore = draftJoinedStore;
        }


        // Get Draft List
        [HttpGet]
        public PagedResult<DraftListViewModel> Get(string q = null, string s = null, int page = 1, int pageSize = 25)
        {
            var query = from draft in draftListViewStore.QueryableCollection
                     select draft;

            return QueryUtil.Apply(query, q, s, page, pageSize);
        }

        // Get Draft Statistics
        [HttpGet("stats")]
        public object GetDraftStats()
        {
            var connectedUsers = hubConnectionsService.GetUsersConnectedToAChannel(typeof(DraftHub)).Count;
            var channels = hubConnectionsService.GetChannels(typeof(DraftHub)).Count;
            var totalDrafts = draftListViewStore.QueryableCollection.Count();

            var queryUsersJoined24 =    from joins in draftJoinedStore.QueryableCollection
                                        where joins.joinedOn >= DateTime.Now.AddDays(-1) && joins.joinedAs != DraftConnectionType.ADMIN
                                        group joins by joins.user into userJoins
                                        select userJoins.First().user;

            var queryUsersJoinedWeek =  from joins in draftJoinedStore.QueryableCollection
                                        where joins.joinedOn >= DateTime.Now.AddDays(-7) && joins.joinedAs != DraftConnectionType.ADMIN
                                        group joins by joins.user into userJoins
                                        select userJoins.First().user;

            var queryUsersJoinedEver =  from joins in draftJoinedStore.QueryableCollection
                                        where  joins.joinedAs != DraftConnectionType.ADMIN
                                        group joins by joins.user into userJoins
                                        select userJoins.First().user;

            var queryDraftsJoined24 =   from joins in draftJoinedStore.QueryableCollection
                                        where joins.joinedOn >= DateTime.Now.AddDays(-1) && joins.joinedAs != DraftConnectionType.ADMIN
                                        group joins by joins.draft into draftJoins
                                        select draftJoins.First().draft;
                                        
            var queryDraftsJoinedWeek = from joins in draftJoinedStore.QueryableCollection
                                        where joins.joinedOn >= DateTime.Now.AddDays(-7) && joins.joinedAs != DraftConnectionType.ADMIN
                                        group joins by joins.draft into draftJoins
                                        select draftJoins.First().draft;

            var queryDraftsJoinedEver = from joins in draftJoinedStore.QueryableCollection
                                        where joins.joinedAs != DraftConnectionType.ADMIN
                                        group joins by joins.draft into draftJoins
                                        select draftJoins.First().draft;

            

            return new {
                numConnectedUsers = connectedUsers,
                numActiveDrafts = channels,
                numDrafts = totalDrafts,
                numDrafts24 = queryDraftsJoined24.Distinct().Count(),
                numUsers24 = queryUsersJoined24.Distinct().Count(),
                numDraftsWeek = queryDraftsJoinedWeek.Distinct().Count(),
                numUsersWeek = queryUsersJoinedWeek.Distinct().Count(),
                numDraftsEver = queryDraftsJoinedEver.Distinct().Count(),
                numUsersEver = queryUsersJoinedEver.Distinct().Count(),
            };
        }

        // Get active Drafts
        [HttpGet("active")]
        public PagedResult<DraftListViewModel> Active(string q = null, string s = null, int page = 1, int pageSize = 25)
        {
            var rooms = draftService.ActiveRooms;
            var activeIds = (from room in rooms
                             select room.DraftModel.Id).ToList();

            var query = from draft in draftListViewStore.QueryableCollection
                        where activeIds.Contains(draft.Id)
                        select draft;

            return QueryUtil.Apply(query, q, s, page, pageSize);
        }

        // Get draft by id
        [HttpGet("{id}")]
        public DraftConfigAdminDTO Get(string id)
        {
            var result = draftStore.FindById(id);
            return new DraftConfigAdminDTO(result);
        }




        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
