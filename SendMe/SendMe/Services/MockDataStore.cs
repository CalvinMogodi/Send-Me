using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SendMe.Model;
using Firebase.Xamarin.Database;
using SendMe.Models;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Collections.Concurrent;
using Firebase.Xamarin.Database.Query;

namespace SendMe.Services
{
    public class MockDataStore : IDataStore<Item, User, Respond, Quote, Request>
    {
        bool isInitialized;
        List<Item> items;
        List<User> users;
        FirebaseClient firebase = new FirebaseClient("https://sendme-7253e.firebaseio.com/");
        private ConcurrentDictionary<string, User> productDictionary;

        public async Task<User> Login(User user)
        {
            User userDetails = null;
            productDictionary = new ConcurrentDictionary<string, User>();
            try
            {
                var users = await firebase.Child("User").OnceAsync<User>();
                foreach (var item in users)
                {                   
                    item.Object.Id = item.Key;
                    while (!productDictionary.TryAdd(item.Object.Id, item.Object)) ;
                    if (item.Object.Password == user.Password && item.Object.Username.ToLower().Trim() == user.Username.ToLower().Trim())
                    {
                        await firebase.Child("User").Child(item.Object.Id).Child("isActive").PutAsync(true);
                        await firebase.Child("User").Child(item.Object.Id).Child("currentLocation").PutAsync(user.CurrentLocation);
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
        
        public async Task Logout(User user)
        {
            User userDetails = null;
            productDictionary = new ConcurrentDictionary<string, User>();
            try
            {
                var users = await firebase.Child("User").OnceAsync<User>();
                foreach (var item in users)
                {
                    item.Object.Id = item.Key;
                    while (!productDictionary.TryAdd(item.Object.Id, item.Object)) ;
                    if (item.Object.Password == user.Password && item.Object.Username.ToLower().Trim() == user.Username.ToLower().Trim())
                    {
                        await firebase.Child("User").Child(item.Object.Id).Child("isActive").PutAsync(false);
                        await firebase.Child("User").Child(item.Object.Id).Child("currentLocation").PutAsync(user.CurrentLocation);
                        break;
                    }
                }
                
            }
            catch (Exception ex)
            {
            }
        }
        public async Task<Respond> AddUserAsync(User user)
        {
            Respond respond = new Respond();

            try {               
                var users = await firebase.Child("User").OnceAsync<User>();
                var thisUser = users.FirstOrDefault(u => u.Object.Username.ToLower().Trim() == user.Username.ToLower().Trim());
                if (thisUser != null)
                {
                    respond.ErrorOccurred = true;
                    respond.IsSuccessful = true;
                    respond.Error = new Error()
                    {
                        UserExist = true,
                        Message = "Username already exist.",
                    };
                }
                else {
                    respond.ErrorOccurred = false;
                    respond.IsSuccessful = true;
                    await firebase.Child("User").PostAsync(user);
                    await Task.FromResult(true);
                }
                   
            }
            catch (Exception ex)
            {
                respond.ErrorOccurred = true;
                respond.IsSuccessful = true;
                respond.Error = new Error()
                {
                    DatabaseError = true,
                    Message = "Error Occurred: Please try again.",
                };

                return respond;
            }

            return respond;
        }

        public async Task<Respond> UpdateUser(User user)
        {
            Respond respond = new Respond();
            productDictionary = new ConcurrentDictionary<string, User>();

            try
            {
                firebase.Child("User").AsObservable<User>().Where(u => !productDictionary.ContainsKey(u.Object.Id) && u.EventType == Firebase.Xamarin.Database.Streaming.FirebaseEventType.InsertOrUpdate)
                .Subscribe(User =>
                {
                    if (User.EventType == Firebase.Xamarin.Database.Streaming.FirebaseEventType.InsertOrUpdate)
                    {
                        while (!productDictionary.TryAdd(User.Object.Id, User.Object)) ;
                    }
                });
            }
            catch (Exception ex)
            {
                respond.ErrorOccurred = true;
                respond.IsSuccessful = true;
                respond.Error = new Error()
                {
                    DatabaseError = true,
                    Message = "Error Occurred: Please try again.",
                };

                return respond;
            }

            return respond;
        }

        public async Task<IEnumerable<Quote>> GetQuotesAsync(Request request)
        {

            ObservableCollection<Quote> quotes = new ObservableCollection<Quote>();
            
            if (users != null)
            {
                IEnumerable<User> couriers = users.Where(u => u.UserTypeId == 3);
                foreach (var user in couriers)
                {
                    decimal thisKmDistance = kmDistance(request.FromLocation.Latitude, request.Tolocation.Latitude, request.FromLocation.Longitude, request.Tolocation.Longitude);
                    decimal courierKmDistance = kmDistance(request.FromLocation.Latitude, user.CurrentLocation.Latitude, request.FromLocation.Longitude, user.CurrentLocation.Longitude);
                    decimal price = thisKmDistance * Convert.ToDecimal(user.Courier.PricePerKM);
                    switch (request.PackageSize)
                    {
                        case "More Items":
                            price = price + Convert.ToDecimal(user.Courier.ExtraCharges);
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
    }
}
