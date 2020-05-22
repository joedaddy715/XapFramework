using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xap.Data.Factory.Interfaces {
    public interface IXapPoco {
        T Insert<T>(T obj);
        Task<T> InsertAsync<T>(T obj);
        T Select<T>(T obj);
        Task<T> SelectAsync<T>(T obj);
        List<T> SelectList<T>(T obj);
        Task<List<T>> SelectListAsync<T>(T obj);
        void Update<T>(T obj);
        Task UpdateAsync<T>(T obj);
        void Delete<T>(T obj);
        Task DeleteAsync<T>(T obj);
    }
}
