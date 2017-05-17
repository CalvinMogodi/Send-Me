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
    public class RequestCourierViewModel : BaseViewModel
    {
        public ObservableRangeCollection<Quote> Quotes { get; set; }

        public async Task GetQuotes(Request request)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Quotes.Clear();
               // var quotes = await DataStore.GetQuotesAsync(request);
                //Quotes.ReplaceRange(quotes);
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
    }
}
