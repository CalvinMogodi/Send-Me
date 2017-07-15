using SendMe.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SendMe.Services
{
    public interface IDataStore<T, U, R, Q, UR>
    {
        Task<U> Login(U user);
        Task<R> UpdateUser(U user);
        Task<R> AddUserAsync(U user);
        Task<IEnumerable<Q>> GetQuotesAsync(UR request);

    }
}
