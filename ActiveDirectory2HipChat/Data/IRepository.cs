using System.Threading.Tasks;

namespace Ad2HipChat.Data
{
    public interface IRepository<T>
    {
        Task<T> Get(int id);
        Task<T> Save(T entity);
        Task Commit();
    }
}
