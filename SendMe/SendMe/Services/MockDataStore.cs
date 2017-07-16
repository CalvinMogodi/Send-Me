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
using SendMe.Helpers;

namespace SendMe.Services
{
    public class MockDataStore : IDataStore<Item, User, Respond, Quote, Request>
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
            try
            {
                var users = await firebase.Child("User").OnceAsync<User>();
                foreach (var item in users)
                {
                    item.Object.Id = item.Key;
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
       
        public async Task<ObservableRangeCollection<Quote>> GetQuotesAsync(Request request)
        {
            ObservableRangeCollection<Quote> quotes = new ObservableRangeCollection<Quote>();
            try
            {
                //if (isInitialized)
                //{
                    isInitialized = true;
                    var couriers = await firebase.Child("User").OnceAsync<User>();
                    foreach (var user in couriers)
                    {
                        if (user.Object.IsActive && user.Object.Courier.VehicleBodyTypes.Contains(request.VehicleBodyType.ToString()))
                        {
                            if (user.Object.CurrentLocation != null)
                            {
                                decimal thisKmDistance = kmDistance(request.FromLocation.Latitude, request.FromLocation.Longitude, request.Tolocation.Latitude, request.Tolocation.Longitude);
                                decimal courierKmDistance = kmDistance(request.FromLocation.Latitude, request.FromLocation.Longitude, user.Object.CurrentLocation.Latitude, user.Object.CurrentLocation.Longitude);
                                decimal price = thisKmDistance * Convert.ToDecimal(user.Object.Courier.PricePerKM);
                                switch (request.PackageSize)
                                {
                                    case "More Items":
                                        price = price + Convert.ToDecimal(user.Object.Courier.ExtraCharges);
                                        break;
                                }

                                Quote quote = new Quote
                                {
                                    CourierKmDistance = courierKmDistance,
                                    CourierName = user.Object.DisplayName,
                                    CourierMobileNumber = user.Object.Courier.MobileNumber,
                                    Price = price,
                                    CourierProfilePicture = user.Object.ProfilePicture,
                                };

                                quotes.Add(quote);
                                isInitialized = true;
                            }
                        }
                    }
                //}
                return await Task.FromResult(quotes);
            }
            catch (Exception ex)
            {
                return quotes;
            }

            
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
