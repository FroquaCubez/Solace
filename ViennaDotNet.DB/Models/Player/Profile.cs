namespace ViennaDotNet.DB.Models.Player
{
    public sealed class Profile
    {
        public int health;
        public int experience;
        public int level;
        public Rubies rubies;

        public Profile()
        {
            health = 20;
            experience = 0;
            level = 1;
            rubies = new Rubies();
        }
    }
}
