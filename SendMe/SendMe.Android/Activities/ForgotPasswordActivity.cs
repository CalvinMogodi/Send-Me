using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SendMe.Models;
using SendMe.ViewModels;
using SendMe.Helpers;
using Android.Content.PM;

namespace SendMe.Droid.Activities
{
    [Activity(Label = "Forgot Password")]
    public class ForgotPasswordActivity : Activity
    {
        Button changePasswordButton;
        EditText username, password, confirmPassword;
        TextView message;
        public bool FormIsValid { get; set; }

        public User User { get; set; }
        public LoginViewModel ViewModel { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Initialize();
        }

        private async void ChangePasswordButton_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            MessageDialog messageDialog = new MessageDialog();
            messageDialog.ShowLoading();

            message.Text = "";
            var _user = new User()
            {
                Username = username.Text,
                Password = password.Text,
            };

           // await ViewModel.ChangePasswordAsync(_user);

            if (ViewModel.IsAuthenticated)
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            }
            else
                message.Text = "Invaild username";

            messageDialog.HideLoading();
        }
        private void Initialize()
        {

            // Create your application here
            SetContentView(Resource.Layout.activity_forgot_password);
            changePasswordButton = FindViewById<Button>(Resource.Id.button_changePassword);
            username = FindViewById<EditText>(Resource.Id.forgotpassword_txtUsername);
            password = FindViewById<EditText>(Resource.Id.forgotpassword_txtPassword);
            confirmPassword = FindViewById<EditText>(Resource.Id.forgotpassword_confirm_password);
            message = FindViewById<TextView>(Resource.Id.forgotpassword_tvmessage);
            ViewModel = new LoginViewModel();
            changePasswordButton.Click += ChangePasswordButton_Click;
        }

        private bool ValidateForm()
        {
            Validations validation = new Validations();
            Android.Graphics.Drawables.Drawable icon = Resources.GetDrawable(Resource.Drawable.error);
            icon.SetBounds(0, 0, icon.IntrinsicWidth, icon.IntrinsicHeight);

            FormIsValid = true;

            if (!validation.IsValidEmail(username.Text))
            {
                username.SetError("Invalid email address", icon);
                FormIsValid = false;
            }

            if (!validation.IsValidPassword(password.Text))
            {
                password.SetError("Password cannot be empty and length must be greater than 6 characters", icon);
                FormIsValid = false;
            }
            if (!validation.IsValidPassword(confirmPassword.Text))
            {
                confirmPassword.SetError("Password cannot be empty and length must be greater than 6 characters", icon);
                FormIsValid = false;
            }

            if (confirmPassword.Text != password.Text)
            {
                confirmPassword.SetError("Password and confirm password don't match", icon);
                FormIsValid = false;
            }

            return FormIsValid;
        }
    }
}