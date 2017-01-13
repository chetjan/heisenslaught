﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Identity;

namespace Heisenslaught.Models.Users
{
        
        
        
    public class HSUserValidator : IUserValidator<HSUser>
    {
        private static readonly Regex BattleTagRegexp = new Regex("^\\d+#\\d+", RegexOptions.Compiled | RegexOptions.IgnoreCase);


        public Task<IdentityResult> ValidateAsync(UserManager<HSUser> manager, HSUser user)
        {
            var errors = new List<IdentityError>();
            if (!BattleTagRegexp.IsMatch(user.BattleTag))
            {
               /* errors.Add(new IdentityError {
                    Code = "BATTLETAG_INVALID",
                    Description = "Invalid BattleTag."
                });*/
            }

            return errors.Any()
                ? Task.FromResult(IdentityResult.Failed(errors.ToArray()))
                : Task.FromResult(IdentityResult.Success);
        }
    }
}
