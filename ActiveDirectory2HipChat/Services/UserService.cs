using ActiveDirectory2HipChat.Directory;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;

namespace ActiveDirectory2HipChat.Services
{
    public class UserService : IUserService
    {
        private readonly IDirectoryContext _directoryContext;

        public UserService(IDirectoryContext directoryContext)
        {
            if (_directoryContext == null)
                _directoryContext = directoryContext;
        }

        public IEnumerable<UserPrincipal> GetAllUsers()
        {
            using (var ctx = _directoryContext.LoadAndConnect())
            {
                // Only load users with a display name and valid email addresses
                var filter = new UserPrincipal(ctx) { DisplayName = "*", EmailAddress = "*@*" };
                using (var search = new PrincipalSearcher(filter))
                {
                    var users = search.FindAll().OfType<UserPrincipal>();
                    return users.ToList();
                }
            }
        }
    }
}
