using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Heisenslaught.Models.Users;

namespace Heisenslaught.DataTransfer.Users
{
    public class AuthenticatedUserDTO : UserBaseDTO
    {
        public string username;
        public string usernameNormailzed;
        public bool requiresSetup;
        public bool requiresEmailValidation;

        public AuthenticatedUserDTO(HSUser user) : base(user)
        {
            username = user.BattleTag;
            usernameNormailzed = user.BattleTagNormaized;
            requiresSetup = user.RequiresSetup();
            requiresEmailValidation = user.RequiresEmailValidation();
        }
    }
}
