using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;

namespace Ad2HipChat.Services
{
    public interface IUserService
    {
        IEnumerable<UserPrincipal> GetAllUsers();
    }
}
