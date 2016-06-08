using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ad2HipChat.Data
{
    public interface IUserRepository : IRepository<User>
    {
        IEnumerable<User> All();
        Task<User> Get(string principalName);
        Task<User> Get(Guid userGuid);
    }
}
