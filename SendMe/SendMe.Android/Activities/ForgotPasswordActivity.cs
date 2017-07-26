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
using SendMe.Droid.Helpers;

namespace SendMe.Droid.Activities
{
    [Activity(Label = "Forgot Password", LaunchMode = LaunchMode.SingleInstance, ConfigurationChanges = ConfigChanges.ScreenSize |
     ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, ParentActivity = typeof(MainActivity))]
    [MetaData("android.support.PARENT_ACTIVITY", Value = ".MainActivity")]
    public class ForgotPasswordActivity : BaseActivity
    {
        Button changePasswordButton, sendOTPButton;
        EditText username, password, confirmPassword, oneTimePin;
        TextView message;
        public bool FormIsValid { get; set; }
        private static Random random = new Random();
        public User User { get; set; }
        public LoginViewModel ViewModel { get; set; }

        protected override int LayoutResource => Resource.Layout.activity_forgot_password;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SupportActionBar.Title = "Forgot Password";
            Initialize();
        }
        private async void SendOTPButton_Click(object sender, EventArgs e)
        {
            message.Text = "";
            Validations validation = new Validations();
            MessageDialog messageDialog = new MessageDialog();
            messageDialog.ShowLoading();

            if (!validation.IsValidEmail(username.Text.Trim()))
            {
                message.Text = "Please enter username to send one time pin";
                messageDialog.HideLoading();
                return;
            }

            int pin = Convert.ToInt32(GetRandomNumber(5));

            await ViewModel.SetOPTForUsersync(username.Text.Trim(), pin);

            if (ViewModel.Respond.ErrorOccurred)
            {
                message.Text = ViewModel.Respond.Error.Message;
                changePasswordButton.Visibility = ViewStates.Gone;
            }                
            else
            {
                messageDialog.SendToast("One time pin is sent successfully.");
                changePasswordButton.Visibility = ViewStates.Visible;
                string smsMessase = string.Format("Send Me: Confirmation OTP:{0}. Change password", pin);
                messageDialog.SendSMS(ViewModel.Respond.User.Courier.MobileNumber.Trim(), smsMessase);
            }
                

            messageDialog.HideLoading();
        }
        public string GetRandomNumber(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private async void ChangePasswordButton_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            MessageDialog messageDialog = new MessageDialog();
            messageDialog.ShowLoading();

            EncryptionHelper encryptionHelper = new EncryptionHelper();
            message.Text = ""; 
            var _user = new User()
            {
                Username = username.Text,
                Password = encryptionHelper.Encrypt(password.Text, "Passw0rd@SendMe"),
            };

            await ViewModel.ChangePasswordAsync(_user, Convert.ToInt32(oneTimePin.Text.Trim()));

            if (ViewModel.Respond.ErrorOccurred)
                message.Text = ViewModel.Respond.Error.Message;
            else
            {
                messageDialog.SendToast("Password is changed successfully.");
                Finish();
            }

            messageDialog.HideLoading();
        }
        private void Initialize()
        {

            // Create your application here
            changePasswordButton = FindViewById<Button>(Resource.Id.button_changePassword);
            username = FindViewById<EditText>(Resource.Id.forgotpassword_txtUsername);
            password = FindViewById<EditText>(Resource.Id.forgotpassword_txtPassword);
            confirmPassword = FindViewById<EditText>(Resource.Id.forgotpassword_confirm_password);
            sendOTPButton = FindViewById<Button>(Resource.Id.button_sendOTP);
            oneTimePin = FindViewById<EditText>(Resource.Id.forgotpassword_OTP);
            message = FindViewById<TextView>(Resource.Id.forgotpassword_tvmessage);
            ViewModel = new LoginViewModel();
            changePasswordButton.Click += ChangePasswordButton_Click;
            sendOTPButton.Click += SendOTPButton_Click;
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

            if (!validation.IsValidOTP(oneTimePin.Text))
            {
                oneTimePin.SetError("Invalid one time pin", icon);
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