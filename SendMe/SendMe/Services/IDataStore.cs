using System.Collections.Generic;
using System.Threading.Tasks;

namespace SendMe.Services
{
    public interface IDataStore<T, U>
    {
        Task<U> Login(U user);
        bool UpdateUser(U user);
        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(T item);
        Task<T> GetItemAsync(string id);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);

        Task InitializeAsync();
        Task<bool> PullLatestAsync();
        Task<bool> SyncAsync();
    }
}
