using System.Data.Entity;

namespace Ad2HipChat.Data
{
    public class DataContext : DbContext
    {
        public DataContext() : base("DataContext")
        {

        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserHistory> UserHistory { get; set; }
    }
}
