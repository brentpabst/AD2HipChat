using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveDirectory2HipChat.Data
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> Get(string principalName);
    }
}
