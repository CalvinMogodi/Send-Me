using SendMe.Helpers;
using SendMe.Interfaces;
using SendMe.Services;
using SendMe.Model;
using SendMe.Models;

namespace SendMe
{
    public partial class App
    {
        public App()
        {
        }

        public static void Initialize()
        {
            ServiceLocator.Instance.Register<IDataStore<Item, User>, MockDataStore>();
        }
    }
}