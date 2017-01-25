namespace Heisenslaught.Users
{
    public class UserBaseDTO
    {
        public string id;
        public string displayName;

        public UserBaseDTO(HSUser user)
        {
            id = user.Id;
            displayName = user.BattleTagDisplay;
        }

    }
}
