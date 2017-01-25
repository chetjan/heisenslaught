using Heisenslaught.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;


namespace Heisenslaught.AngularApplication
{
    public class AngularController : Controller
    {
        private readonly UserManager<HSUser> _userManager;

        public AngularController(UserManager<HSUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            AuthenticatedUserDTO authenticatedUser = null;
            
            if(this.User != null)
            {
                var user = await _userManager.GetUserAsync(this.User);
                if(user != null)
                {
                    authenticatedUser = new AuthenticatedUserDTO(user);
                }
            }
            ViewData["authenticatedUser"] = JsonConvert.SerializeObject(authenticatedUser);//.Replace("\"", "\\\"");
            return View("/wwwroot/index.cshtml");
        }
    }
}
