using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Heisenslaught.Models.Users
{
    public class HSRole
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string NameNomalized { get; private set; }
        public string Description { get; private set; }
        public HSRole(string name, string description)
        {
            Id = ObjectId.GenerateNewId().ToString();
            Name = name;
            Description = description;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetNameNormailzed(string nameNomalized)
        {
            NameNomalized = nameNomalized;
        }

        public void SetDescription(string description)
        {
            Description = description;
        }
    }
}
