using System.Data.Entity;
using NLog;

namespace Ad2HipChat.Data
{
    public class DataContext : DbContext
    {
        private static readonly Logger Logger = LogManager.GetLogger("Ad2HipChat");

        public DataContext() : base("DataContext")
        {

        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserHistory> UserHistory { get; set; }
    }
}
