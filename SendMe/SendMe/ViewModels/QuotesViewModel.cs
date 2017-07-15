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

        public QuotesViewModel(Request request)
        {
            Title = "Quotes";
            Quotes = new ObservableRangeCollection<Quote>();
            var task = ExecuteLoadQuotesCommand(request);
            //task.Wait();
        }

        public async Task ExecuteLoadQuotesCommand(Request request)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Quotes.Clear();
                var quotes = MockedQuotesAsync(request);
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


        private async Task<ObservableRangeCollection<Quote>> MockedQuotesAsync(Request request)
        {

           var quotes = await DataStore.GetQuotesAsync(request);            
            Quotes = quotes;
            return quotes;
        }

    }
}
