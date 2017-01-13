using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heisenslaught.Models.Users
{
    public class HSEmailAddress
    {
        public string Value;
        public string ValueNormaized;
        public Nullable<DateTime> Confirmed;
        public string SecretPhrase;

        public bool IsConfirmed()
        {
            return Confirmed != null;
        }

        public void SetConfirmed()
        {
            Confirmed = new DateTime();
        }

        public void SetUnconfirmed()
        {
            Confirmed = null;
        }

        public bool Equals(HSEmailAddress other)
        {
            return other.ValueNormaized.Equals(ValueNormaized);
        }

        public string GetContactValue()
        {
            if(SecretPhrase == null)
            {
                return Value;
            }
            var parts = Value.Split(new char[] { '@' }, 2);
            return parts[0] + '+' + SecretPhrase + '@' + parts[1];
        }
    }
}
