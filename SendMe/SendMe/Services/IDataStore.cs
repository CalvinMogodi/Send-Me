using SendMe.Helpers;
using SendMe.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SendMe.Services
{
    public interface IDataStore<T, U, R, Q, UR>
    {
        Task<U> Login(U user);
        Task<R> AddUserAsync(U user);
        Task<ObservableRangeCollection<Q>> GetQuotesAsync(UR request);
        Task Logout(User user);
        void SaveQuoteRequest(QuoteRequest quoteRequest);
    }
}
