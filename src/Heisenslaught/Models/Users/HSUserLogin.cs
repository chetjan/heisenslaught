using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Heisenslaught.Models.Users
{
    public class HSUserLogin : IEquatable<HSUserLogin>, IEquatable<UserLoginInfo>
    {
        public string LoginProvider;
        public string ProviderKey;

        public HSUserLogin() { }
        public HSUserLogin(UserLoginInfo info) : this(info.LoginProvider, info.ProviderKey) {}

        public HSUserLogin(string loginProvider, string providerKey)
        {
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
        }

        public bool Equals(UserLoginInfo other)
        {
            return other.LoginProvider.Equals(LoginProvider) && other.ProviderKey.Equals(ProviderKey);
        }

        public bool Equals(HSUserLogin other)
        {
            return other.LoginProvider.Equals(LoginProvider) && other.ProviderKey.Equals(ProviderKey);
        }
    }
}
