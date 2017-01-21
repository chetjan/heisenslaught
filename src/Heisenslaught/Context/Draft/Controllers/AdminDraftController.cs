using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Heisenslaught.Draft
{
    [Route("api/admin/draft")]
    public class AdminDraftController : Controller
    {
        private readonly IDraftStore draftStore;
        private readonly DraftListViewStore draftListViewStore;
        private readonly IDraftService draftService;
        public AdminDraftController(
            IDraftStore draftStore,
            DraftListViewStore draftListViewStore,
            IDraftService draftService
        ){
            this.draftStore = draftStore;
            this.draftListViewStore = draftListViewStore;
            this.draftService = draftService;
        }


        // GET: api/values
        [HttpGet]
        public List<DraftListViewModel> Get()
        {
            return draftListViewStore.FindAll();
        }

        
        [HttpGet("active")]
        public List<DraftConfigAdminDTO> Active()
        {
            var rooms = draftService.ActiveRooms;
            return (from room in rooms
                select new DraftConfigAdminDTO(room)).ToList();
        }

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
