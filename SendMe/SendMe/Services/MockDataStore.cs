using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SendMe.Model;
using Firebase.Xamarin.Database;
using SendMe.Models;

namespace SendMe.Services
{
    public class MockDataStore : IDataStore<Item, User>
    {
        bool isInitialized;
        List<Item> items;
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
    }
}
