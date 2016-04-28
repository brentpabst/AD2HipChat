using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;

namespace ActiveDirectory2HipChat.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserPrincipal>> GetAllUsers();
    }
}
