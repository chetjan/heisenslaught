using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.Identity.MongoDB;
using MongoDB.Driver;
using AspNetCore.Identity.MongoDB;

namespace Heisenslaught.Models
{
    public class HSIdentityUser: MongoIdentityUser
    {
        public string BattleTag { get; set; }

        public HSIdentityUser(string userName) : base(userName){ }
        public HSIdentityUser(string userName, string email) : base(userName, email) { }
    }

   
    /*
    public class HSIdentityDbContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<HSIdentity, HSIdentityRole, string>
    {
        public HSIdentityDbContext(DbContextOptions<HSIdentityDbContext> options):base(options)
        {
            
        }
    }
    */
}
