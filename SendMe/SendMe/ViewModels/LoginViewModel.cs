﻿using SendMe.Models;
using SendMe.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMe.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public User User { get; set; }
        public bool IsAuthenticated { get; set; }
        public LoginViewModel()
        {
            Title = "Login";
        }

        public async Task LoginUserAsync(User user)
        {
            var _user = user as User;
            User = await DataStore.Login(_user);
            if (User != null)
                IsAuthenticated = true;
            else
                IsAuthenticated = false;
        }
    }
}
