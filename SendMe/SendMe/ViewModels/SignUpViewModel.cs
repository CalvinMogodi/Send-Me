﻿using SendMe.Models;
using SendMe.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMe.ViewModels
{
    public class SignUpViewModel : BaseViewModel
    {
        public User User { get; set; }

        public bool IsSignUp { get; set; }       

        public SignUpViewModel()
        {
            Title = "Sign Up";
        }

        public async Task SignUpUser(User user)
        {
            var _user = user as User;
            Respond = await DataStore.AddUserAsync(_user);
        }
    }
}
