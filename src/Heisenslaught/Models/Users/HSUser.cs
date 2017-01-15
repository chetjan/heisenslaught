using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;


namespace Heisenslaught.Models.Users
{
    public class HSUser : IEquatable<HSUser>
    {
        private List<HSUserLogin> _logins;
        private List<string> _roles;

        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; private set; }
        public string BattleNetId { get; private set; }
        public string BattleTag { get; private set; }
        public string BattleTagNormaized { get; private set; }
        public string BattleTagDisplay { get; private set; }

        // for later
        public string PasswordHash;
        public List<HSEmailAddress> Emails;
        public HSEmailAddress PrimaryEmail;


        public HSUser()
        {
            _logins = new List<HSUserLogin>();
            _roles = new List<string>();
        }

        public HSUser(string battleId, string battleTag) : this()
        {
            Id = ObjectId.GenerateNewId().ToString();
            BattleNetId = battleId;
            BattleTag = battleTag;
            SetBattleTag(battleTag);
        }

        public void SetBattleTag(string battleTag)
        {
            BattleTag = battleTag;
            var parts = battleTag.Split(new char[] { '#' }, 2);
            BattleTagDisplay = parts[0];
        }

        public void SetBattleTagNormaized(string battleTagNormailzed)
        {
            BattleTagNormaized = battleTagNormailzed;
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

        public void AddLogin(HSUserLogin login)
        {
            _logins.Add(login);
        }

        public bool RemoveLogin(HSUserLogin login)
        {
            return this._logins.Remove(login);
        }

        public IEnumerable<string> Roles
        {
            get
            {
                return _roles;
            }

            private set
            {
                if (value != null)
                {
                    _roles.AddRange(value);
                }
            }
        }

        public void AddRole(string role)
        {
            this._roles.Add(role);
        }

        public bool RemoveRole(string role)
        {
            return this._roles.Remove(role);
        }

        public bool HasRole(string role)
        {
            return this._roles.Contains(role);
        }

        public bool RequiresSetup()
        {
            return PrimaryEmail == null || PasswordHash == null;
        }

        public bool RequiresEmailValidation()
        {
            if (RequiresSetup())
            {
                return true;
            }
            return !PrimaryEmail.IsConfirmed();
        }

        public bool Equals(HSUser other)
        {
            return other.Id == Id;
        }
    }
}
