using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SendMe.Helpers;
using SendMe.Model;
using SendMe.Models;

namespace SendMe.ViewModel
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableRangeCollection<Quote> Quotes { get; set; }

        public ItemsViewModel()
        {
            Title = "Browse";
            Quotes = new ObservableRangeCollection<Quote>();
            var task = GetAdvertsAsync();
            //task.Wait();
        }       

        public async Task GetAdvertsAsync()
        {
            //Adverts = await DataStore.GetAdvertsAsync();
            
        }

        ItemDetailViewModel detailsViewModel;
    }
}