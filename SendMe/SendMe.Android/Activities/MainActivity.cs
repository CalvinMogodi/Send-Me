using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using SendMe.Droid.Activities;
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

        //ViewPager pager;
        TabsAdapter adapter;
        AlertDialog dialog;
        Button rcButton, acButton, loginButton, loginCancelButton;
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
            courierIsActive = FindViewById<Switch>(Resource.Id.main_switchActivateCourier);

            rcButton.Click += RCButton_Click;
            acButton.Click += ACButton_Click;

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
            loginButton.Click += LoginButton_Click;
            loginCancelButton.Click += LoginCancelButton_Click;
            forgotpassword.Click += ForgotPassword_Click;
            LoginViewModel = new LoginViewModel();
        }

        public void ForgotPassword_Click(object sender, EventArgs e)
        {
            var intent = new Intent();
            intent = new Intent(this, typeof(ForgotPasswordActivity));
            StartActivity(intent);
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            courierIsActive.Checked = IsAuthenticated;
            if (!ValidateForm())
            {
                return;
            }

            message.Text = "";
            var _user = new User()
            {
                Username = username.Text,
                Password = password.Text,
            };

            var permissionCheck = CheckSelfPermission("ACCESS_FINE_LOCATION").ToString();
            if (permissionCheck.Equals("Denied"))
            {
                LocationManager locationManager = (LocationManager)GetSystemService(LocationService);
                var currentLocation = locationManager.GetLastKnownLocation(LocationManager.GpsProvider);
                _user.CurrentLocation = new SendMe.Models.Location()
                {
                    Latitude = currentLocation.Latitude,
                    Longitude = currentLocation.Longitude,
                };
            }
            //LoginViewModel.LoginUser(_user);

            if (LoginViewModel.IsAuthenticated)
            {
                IsAuthenticated = LoginViewModel.IsAuthenticated;
                courierIsActive.Checked = IsAuthenticated;
                dialog.Cancel();
            }
            else
            {
                message.Text = "Invaild username or password";
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


