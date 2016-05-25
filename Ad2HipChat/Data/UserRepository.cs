using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Ad2HipChat.Data
{
    public class UserRepository : IUserRepository
    {
        public DbContext Context { get; }

        public UserRepository(DbContext context)
        {
            Context = context;
        }

        private DbSet<User> EntitySet()
        {
            return Context.Set<User>();
        }

        public virtual IEnumerable<User> All()
        {
            return EntitySet().Where(u => !u.IsSynced);
        }

        public virtual async Task<User> Get(int id)
        {
            if (default(int).Equals(id)) throw new ArgumentNullException(nameof(id));
            return await EntitySet().FindAsync(id);
        }

        public virtual async Task<User> Get(string principalName)
        {
            if (string.IsNullOrWhiteSpace(principalName)) throw new ArgumentNullException(nameof(principalName));
            return await EntitySet().FirstOrDefaultAsync(u => u.Principal == principalName);
        }

        public virtual async Task<User> Save(User entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (default(int).Equals((entity.Id)))
            {
                // Insert
                EntitySet().Add(entity);
            }
            else
            {
                // Update
                var entry = Context.Entry(entity);

                if (entry.State == EntityState.Detached)
                {
                    EntitySet().Attach(entity);
                }

                entry.State = EntityState.Modified;
            }

            return await Task.Run(() => entity);
        }

        public async Task Commit()
        {
            await Context.SaveChangesAsync();
        }
    }
}
