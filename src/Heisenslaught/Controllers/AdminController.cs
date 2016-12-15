using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Heisenslaught.Controllers
{
    public class AdminController : Controller
    {
        // GET: /<controller>/
        public FileResult Index()
        {
            return new PhysicalFileResult(Directory.GetCurrentDirectory() + "/wwwroot/index.html", "text/html");
        }
    }
}
