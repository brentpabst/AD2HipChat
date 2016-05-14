using System.Threading.Tasks;

namespace Ad2HipChat.Data
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> Get(string principalName);
    }
}
