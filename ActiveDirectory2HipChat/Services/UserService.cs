using ActiveDirectory2HipChat.Directory;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;

namespace ActiveDirectory2HipChat.Services
{
    public class UserService:IUserService
    {
        private readonly IDirectoryContext _directoryContext;

        public UserService(IDirectoryContext directoryContext)
        {
            if (_directoryContext == null)
                _directoryContext = directoryContext;
        }

        public Task<IEnumerable<UserPrincipal>> GetAllUsers()
        {
            using (var ctx = _directoryContext.LoadAndConnect())
            {
                var filter = new UserPrincipal(ctx) { DisplayName = "*" };
                using (var search = new PrincipalSearcher(filter))
                {
                    var users = search.FindAll().OfType<UserPrincipal>();
                    return Task.FromResult(users);
                }
            }
        }
    }
}
