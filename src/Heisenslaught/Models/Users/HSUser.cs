﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Heisenslaught.Models.Users
{
    public class HSUser
    {
        private List<HSUserLogin> _logins;
   
        public string Id { get; private set; }
        public string BattleTag { get; private set; }
        public string BattleTagNormaized { get; private set; }
        public string BattleTagDisplay { get; private set; }

        public string PasswordHash;
        public List<HSEmailAddress> Emails;
        public HSEmailAddress PrimaryEmail;


        public HSUser()
        {
            _logins = new List<HSUserLogin>();
        }

        public HSUser(string id, string battleTag) : this()
        {
            Id = id;
            BattleTag = battleTag;
            SetBattleTag(battleTag);
        }

        public void SetBattleTag(string battleTag)
        {
            BattleTag = battleTag;
            var parts = battleTag.Split(new char[] { '#' }, 2);
            BattleTagDisplay = parts[0];
        }

        public IEnumerable<HSUserLogin> Logins
        {
            get
            {
                return _logins;
            }

            private set
            {
                if (value != null)
                {
                    _logins.AddRange(value);
                }
            }
        }

    }
}
