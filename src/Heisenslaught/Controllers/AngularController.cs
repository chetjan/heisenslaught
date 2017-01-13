using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Heisenslaught.Models.Users;
using Heisenslaught.DataTransfer.Users;
using Newtonsoft.Json;



// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Heisenslaught.Controllers
{
    public class AngularController : Controller
    {
        private readonly UserManager<HSUser> _userManager;

        public AngularController(UserManager<HSUser> userManager)
        {
            _userManager = userManager;
        }
        //public FileResult Index()

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
