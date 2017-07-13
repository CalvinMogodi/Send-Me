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
            task.Wait();
        }

        public async Task ExecuteLoadQuotesCommand(Request request)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Quotes.Clear();
                var quotes = MockedQuotes();
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


        private ObservableRangeCollection<Quote> MockedQuotes()
        {

            var quotes = new ObservableRangeCollection<Quote>(){
                new Quote { Id = Guid.NewGuid(),
                    CourierName = "Calvin Mogodi",
                    CourierMobileNumber = "1234567",
                    CourierKmDistance = 3.36m,
                    Price = 100,
                    CourierProfilePicture = ""
                    },
                 new Quote { Id = Guid.NewGuid(),
                    CourierName = "Given N",
                    CourierMobileNumber = "1234567",
                    CourierKmDistance = 15.6m,
                    Price = 515,
                    CourierProfilePicture = ""
                    },
                  new Quote { Id = Guid.NewGuid(),
                    CourierName = "Given N",
                    CourierMobileNumber = "1234567",
                    CourierKmDistance = 15.6m,
                    Price = 515,
                    CourierProfilePicture = ""
                    },
                   new Quote { Id = Guid.NewGuid(),
                    CourierName = "Given N",
                    CourierMobileNumber = "1234567",
                    CourierKmDistance = 15.6m,
                    Price = 515,
                    CourierProfilePicture = ""
                    },
            };
            Quotes = quotes;
            return quotes;
        }

    }
}
