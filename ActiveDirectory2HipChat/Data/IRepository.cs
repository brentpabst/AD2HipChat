using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveDirectory2HipChat.Data
{
    public interface IRepository<T>
    {
        Task<T> Get(int id);
        Task<T> Save(T entity);
        Task Commit();
    }
}
