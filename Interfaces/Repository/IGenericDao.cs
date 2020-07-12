using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Interfaces.Services
{
    public interface IGenericDao<T,K>
    {
        Task CreateAsync(T t);
        Task UpdateAsync(T t);
        Task DeleteAsync(T t);
        Task<IEnumerable<T>> FindAllAsync();
        Task<T> FindByIdAsync(K key);
    }
}
