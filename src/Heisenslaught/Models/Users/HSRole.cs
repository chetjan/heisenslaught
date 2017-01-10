using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Heisenslaught.Models.Users
{
    public class HSRole :IdentityRole<string>
    {
        public HSRole()
        {
            
        }
    }
}
