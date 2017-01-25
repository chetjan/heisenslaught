﻿using System.Collections.Generic;
using System.Linq;

namespace Heisenslaught.Users
{
    public class AuthenticatedUserDTO : UserBaseDTO
    {
        public string username;
        public string usernameNormailzed;
        public List<string> roles;
        public bool requiresSetup;
        public bool requiresEmailValidation;

        public AuthenticatedUserDTO(HSUser user) : base(user)
        {
            username = user.BattleTag;
            usernameNormailzed = user.BattleTagNormaized;
            roles = user.Roles.ToList();
            requiresSetup = user.RequiresSetup();
            requiresEmailValidation = user.RequiresEmailValidation();
        }
    }
}
