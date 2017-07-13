using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SendMe.Model;
using Firebase.Xamarin.Database;
using SendMe.Models;
using System.Collections.ObjectModel;

namespace SendMe.Services
{
    public class MockDataStore : IDataStore<Item, User>
    {
        bool isInitialized;
        List<Item> items;
        List<User> users;
        FirebaseClient firebase = new FirebaseClient("https://sendme-7253e.firebaseio.com/");

        public async Task<User> Login(User user)
        {
            User userDetails = null;
            try
            {
                var users = await firebase.Child("User").OnceAsync<User>();
                foreach (var item in users)
                {
                    item.Object.Id = item.Key;
                    if (item.Object.Password == user.Password && item.Object.Username.ToLower().Trim() == user.Username.ToLower().Trim())
                    {
                        userDetails = item.Object;
                        break;
                    }
                }

                return userDetails;
            }
            catch (Exception ex)
            {
                return userDetails;
            }
        }

        public bool UpdateUser(User user)
        {
            bool IsUpdated = false;

            //User thisUser = users.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);
            //if (thisUser != null)
            //{
            //    thisUser.IsActive = true;
            //    thisUser.CurrentLocation = user.CurrentLocation;
            //}
            return IsUpdated;
        }

        public async Task<IEnumerable<Quote>> GetQuotesAsync(Request request)
        {

            ObservableCollection<Quote> quotes = new ObservableCollection<Quote>();

            MockedCouriers();

            if (users != null)
            {
                IEnumerable<User> couriers = users.Where(u => u.UserTypeId == 3);
                foreach (var user in couriers)
                {
                    decimal thisKmDistance = kmDistance(request.FromLocation.Latitude, request.Tolocation.Latitude, request.FromLocation.Longitude, request.Tolocation.Longitude);
                    decimal courierKmDistance = kmDistance(request.FromLocation.Latitude, user.CurrentLocation.Latitude, request.FromLocation.Longitude, user.CurrentLocation.Longitude);
                    decimal price = thisKmDistance * Convert.ToDecimal(user.CourierPrice.PricePerKM);
                    switch (request.PackageSize)
                    {
                        case "More Items":
                            price = price + Convert.ToDecimal(user.CourierPrice.ExtraCharges);
                            break;
                    }


                    Quote quote = new Quote
                    {
                        CourierKmDistance = courierKmDistance,
                        CourierName = user.DisplayName,
                        Price = price,
                    };

                    quotes.Add(quote);
                }
            }

            return await Task.FromResult(quotes);
        }

        private decimal kmDistance(double lat1, double lon1, double lat2, double lon2)
        { // generally used geo measurement function: 
            double R = 6378.137;            // Radius of earth in KM 
            double dLat = lat2 * Math.PI / 180 - lat1 * Math.PI / 180;
            double dLon = lon2 * Math.PI / 180 - lon1 * Math.PI / 180;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c;
            return (decimal)d; // Km returned 
        }

        //To Be Removed
        public async Task<bool> AddItemAsync(Item item)
        {
            await InitializeAsync();

            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            await InitializeAsync();

            var _item = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(_item);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(Item item)
        {
            await InitializeAsync();

            var _item = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(_item);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            await InitializeAsync();

            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            await InitializeAsync();

            return await Task.FromResult(items);
        }

        public Task<bool> PullLatestAsync()
        {
            return Task.FromResult(true);
        }


        public Task<bool> SyncAsync()
        {
            return Task.FromResult(true);
        }

        public async Task InitializeAsync()
        {
            if (isInitialized)
                return;

            items = new List<Item>();
            var _items = new List<Item>
            {
                new Item { Id = Guid.NewGuid().ToString(), Text = "Buy some cat food", Description="The cats are hungry"},
                new Item { Id = Guid.NewGuid().ToString(), Text = "Learn F#", Description="Seems like a functional idea"},
                new Item { Id = Guid.NewGuid().ToString(), Text = "Learn to play guitar", Description="Noted"},
                new Item { Id = Guid.NewGuid().ToString(), Text = "Buy some new candles", Description="Pine and cranberry for that winter feel"},
                new Item { Id = Guid.NewGuid().ToString(), Text = "Complete holiday shopping", Description="Keep it a secret!"},
                new Item { Id = Guid.NewGuid().ToString(), Text = "Finish a todo list", Description="Done"},
            };

            foreach (Item item in _items)
            {
                items.Add(item);
            }

            isInitialized = true;
        }

        private void MockedCouriers()
        {

            users = new List<User>();
            var _users = new List<User>
            {
                new User { Id = Guid.NewGuid().ToString(),
                    DisplayName = "Calvin Mogodi",
                    Password = "1234567", Username="calvin@gmail.com",
                    UserTypeId = 3,
                    CourierPrice = new CourierPrice(){
                        PricePerKM = 100,
                        MobileNumber = "0761234568",
                        VehicleBodyTypes = new List<string>(){"",""},
                        Courier = new User(){ },
                        ExtraCharges = 90,
                    }
                    },
                 new User { Id = Guid.NewGuid().ToString(),
                    DisplayName = "Given N",
                    Password = "1234567", Username="given@gmail.com",
                    UserTypeId = 3,
                    CourierPrice = new CourierPrice(){
                        PricePerKM = 515,
                        MobileNumber = "0632589632",
                        VehicleBodyTypes = new List<string>(){"",""},
                        Courier = new User(){ },
                        ExtraCharges = 50,
                    }
                    },
            };

            foreach (User user in _users)
            {
                users.Add(user);
            }

        }
    }
}
