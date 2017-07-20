using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.Net;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using SendMe.Droid.Activities;
using SendMe.Droid.Helpers;
using SendMe.Helpers;
using SendMe.Models;
using SendMe.ViewModels;
using System;

namespace SendMe.Droid
{
    [Activity(Label = "@string/app_name",
        LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : BaseActivity
    {

        protected override int LayoutResource => Resource.Layout.activity_main;
        AlertDialog settingsAlert;
        //ViewPager pager;
        TabsAdapter adapter;
        AlertDialog dialog;
        ProgressBar progressBar;
        Button rcButton, acButton, loginButton, loginCancelButton, manageProfileButton;
        EditText username, password;
        bool IsAuthenticated = false;
        TextView message, forgotpassword;
        Switch courierIsActive;
        public User User { get; set; }
        public LoginViewModel LoginViewModel { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            rcButton = FindViewById<Button>(Resource.Id.main_requestCourier);
            acButton = FindViewById<Button>(Resource.Id.main_activateCourier);
            manageProfileButton = FindViewById<Button>(Resource.Id.main_manageProfile);
            courierIsActive = FindViewById<Switch>(Resource.Id.main_switchActivateCourier);

            courierIsActive.Click += SwitchActivateCourier_Click;

            rcButton.Click += RCButton_Click;
            acButton.Click += ACButton_Click;
            manageProfileButton.Click += ManageProfileButton_Click;
            
            var networkInfo =  (ConnectivityManager)GetSystemService(ConnectivityService);
            
            if (networkInfo.ActiveNetworkInfo == null)
            {
                rcButton.Visibility = ViewStates.Gone;
                acButton.Visibility = ViewStates.Gone;
                courierIsActive.Visibility = ViewStates.Gone;
                Toolbar.Visibility = ViewStates.Gone;
                MessageDialog messageDialog = new MessageDialog();
                messageDialog.SendToast("Failed to connect to network.");
            }

            if (courierIsActive.Checked)
                manageProfileButton.Visibility = ViewStates.Visible;
            else
                manageProfileButton.Visibility = ViewStates.Gone;

            Toolbar.MenuItemClick += (sender, e) =>
            {
                var itemTitle = e.Item.TitleFormatted;
                var intent = new Intent();
                switch (itemTitle.ToString())
                {                    
                    case "Sign Up":
                        intent = new Intent(this, typeof(SignUpActivity));
                        break;
                    case "Contact Us":
                        intent = new Intent(this, typeof(ContactActivity));
                        break;
                    case "Manage My Profile":
                        intent = new Intent(this, typeof(ManageProfileActivity));
                        break;
                }

                StartActivity(intent);
            };
        }

        private void RCButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent();
            intent = new Intent(this, typeof(RequestCourierActivity));
            StartActivity(intent);
        }

        private void ManageProfileButton_Click(object sender, EventArgs e)
        {
            if (LoginViewModel != null)
            {
                if (LoginViewModel.User != null)
                {
                    var intent = new Intent();
                    intent = new Intent(this, typeof(ManageProfileActivity));
                    intent.PutExtra("userId", LoginViewModel.User.Id);
                    StartActivity(intent);
                }
                else {
                    MessageDialog messageDialog = new MessageDialog();
                    messageDialog.SendToast("Please log in to manage your profile.");
                }
            }
        }        

        private void ACButton_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            // Get the layout inflater
            LayoutInflater inflater = this.LayoutInflater;
            builder.SetView(inflater.Inflate(Resource.Layout.dialog_login, null));
            dialog = builder.Create();

            dialog.Show();
            dialog.SetCanceledOnTouchOutside(false);
            InitializeLogin(dialog);
        }

        private void InitializeLogin(AlertDialog dialog)
        {

            // Create your application here
            loginButton = dialog.FindViewById<Button>(Resource.Id.dialog_login_loginbutton);
            loginCancelButton = dialog.FindViewById<Button>(Resource.Id.dialog_login_cancelbutton);
            username = dialog.FindViewById<EditText>(Resource.Id.dialog_login_username);
            password = dialog.FindViewById<EditText>(Resource.Id.dialog_login_password);
            message = dialog.FindViewById<TextView>(Resource.Id.dialog_login_tvmessage);
            forgotpassword = dialog.FindViewById<TextView>(Resource.Id.login_forgot_password);
            progressBar = dialog.FindViewById<ProgressBar>(Resource.Id.progress_bar);
            loginButton.Click += LoginButton_Click;
            loginCancelButton.Click += LoginCancelButton_Click;
            forgotpassword.Click += ForgotPassword_Click;
            progressBar.Indeterminate = true;
            progressBar.Visibility = ViewStates.Gone;

            LoginViewModel = new LoginViewModel();
        }

        public void ForgotPassword_Click(object sender, EventArgs e)
        {
            var intent = new Intent();
            intent = new Intent(this, typeof(ForgotPasswordActivity));
            StartActivity(intent);
        }

        public void SwitchActivateCourier_Click(object sender, EventArgs e) {           
            if (courierIsActive.Checked)
            {
                ACButton_Click(sender, e);
            }
            else {
                manageProfileButton.Visibility = ViewStates.Gone;
                courierIsActive.Checked = false;
                if (LoginViewModel != null)
                    {
                        if (LoginViewModel.User != null)
                        {
                            LoginViewModel.LogoutUserAsync(LoginViewModel.User);
                        }
                    }
            }
        }

        public void ShowSettingsAlert()
        {
            AlertDialog.Builder alertDialog = new AlertDialog.Builder(Application.Context);
            Intent intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
            StartActivity(intent);
            // Setting Dialog Title
            //alertDialog.SetTitle("GPS is settings").SetMessage("GPS is not enabled. Do you want to go to settings menu?").SetPositiveButton("OK", delegate {
            //    Intent intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
            //    Application.Context.StartActivity(intent);
            //}).SetNegativeButton("Cancel", delegate {
                
            //});
            //AlertDialog settingsAlertd = alertDialog.Create();
            //settingsAlertd.Show();
            //settingsAlertd.SetCanceledOnTouchOutside(false);

        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            courierIsActive.Checked = IsAuthenticated;
            if (!ValidateForm())
                return;

            MessageDialog messageDialog = new MessageDialog();
            progressBar.Visibility = ViewStates.Visible;
            loginButton.Enabled = false;
            loginCancelButton.Enabled = false;
            forgotpassword.Enabled = false;
            EncryptionHelper encryptionHelper = new EncryptionHelper();
            message.Text = "";
            var _user = new User()
            {
                Username = username.Text.Trim(),
                Password = encryptionHelper.Encrypt(password.Text, "Passw0rd@SendMe"),
            };

            LocationManager locationManager = (LocationManager)GetSystemService(LocationService);

            bool isGPSEnabled = locationManager.IsProviderEnabled(LocationManager.GpsProvider);
            bool isNetworkEnabled = locationManager.IsProviderEnabled(LocationManager.NetworkProvider);
            bool isPassiveProviderEnabled = locationManager.IsProviderEnabled(LocationManager.PassiveProvider);

            if (!isGPSEnabled && !isNetworkEnabled && !isPassiveProviderEnabled)
            {        
                dialog.Cancel();
                messageDialog.SendToast("GPS is not enabled");
                ShowSettingsAlert();                
            }
            
            else {
                Android.Locations.Location location = null; // location

                if (isGPSEnabled)
                {
                    if (location == null)
                        location = locationManager.GetLastKnownLocation(LocationManager.GpsProvider);
                }                    
                if (isNetworkEnabled)
                {
                    if (location == null)
                        location = locationManager.GetLastKnownLocation(LocationManager.NetworkProvider);
                }
                if (isPassiveProviderEnabled)
                {
                    if (location == null)
                        location = locationManager.GetLastKnownLocation(LocationManager.PassiveProvider);
                }
                if (location != null)
                {
                    _user.CurrentLocation = new SendMe.Models.Location()
                    {
                        Latitude = location.Latitude,
                        Longitude = location.Longitude,
                    };
                }
                else {
                   
                    messageDialog.SendToast("Unable to get your location please make sure your GPS is enabled");
                    return;
                }
                
            }

            await LoginViewModel.LoginUserAsync(_user);

            if (LoginViewModel.IsAuthenticated)
            {
                IsAuthenticated = LoginViewModel.IsAuthenticated;
                courierIsActive.Checked = IsAuthenticated;
                manageProfileButton.Visibility = ViewStates.Visible;
                dialog.Cancel();
            }
            else
            {
                message.Text = "Invaild username or password";
                loginButton.Enabled = true;
                loginCancelButton.Enabled = true;
                manageProfileButton.Visibility = ViewStates.Gone;
                progressBar.Visibility = ViewStates.Gone;
                forgotpassword.Enabled = false;
                dialog.Show();
            }
            
        }

        private void LoginCancelButton_Click(object sender, EventArgs e)
        {
            dialog.Cancel();
            courierIsActive.Checked = IsAuthenticated;

        }

        private bool ValidateForm()
        {
            Validations validation = new Validations();
            Android.Graphics.Drawables.Drawable icon = Resources.GetDrawable(Resource.Drawable.error);
            icon.SetBounds(0, 0, icon.IntrinsicWidth, icon.IntrinsicHeight);

            bool formIsValid = true;

            if (!validation.IsValidEmail(username.Text))
            {
                username.SetError("Invalid email address", icon);
                formIsValid = false;
            }

            if (!validation.IsValidPassword(password.Text))
            {
                password.SetError("Password cannot be empty and length must be greater than 6 characters", icon);
                formIsValid = false;
            }

            return formIsValid;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

    }

    class TabsAdapter : FragmentStatePagerAdapter
    {
        string[] titles;

        public override int Count => titles.Length;

        public TabsAdapter(Context context, Android.Support.V4.App.FragmentManager fm) : base(fm)
        {
            titles = context.Resources.GetTextArray(Resource.Array.sections);
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position) =>
                            new Java.Lang.String(titles[position]);

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0: return BrowseFragment.NewInstance();
               // case 1: return AboutFragment.NewInstance();
            }
            return null;
        }

        public override int GetItemPosition(Java.Lang.Object frag) => PositionNone;

    }

}


