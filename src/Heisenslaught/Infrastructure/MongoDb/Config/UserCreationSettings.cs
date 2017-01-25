using System.Collections.Generic;


namespace Heisenslaught.Infrastructure.MongoDb
{
    public class UserCreationSettings
    {
        public List<string> AutoGrantSuperUserToBattleTags { get; set; }
    }
}
