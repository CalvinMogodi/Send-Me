using SendMe.Helpers;
using SendMe.Models;
using SendMe.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMe.ViewModels
{
    public class QuotesViewModel : BaseViewModel
    {
        public ObservableRangeCollection<Quote> Quotes { get; set; }

        public QuotesViewModel()
        {
            Title = "Quotes";
        }

        public async Task GetQuotes(Request request)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Quotes = await DataStore.GetQuotesAsync(request);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async void SaveQuoteRequest(QuoteRequest quoteRequest) {
            DataStore.SaveQuoteRequest(quoteRequest);
        }

    }
}
