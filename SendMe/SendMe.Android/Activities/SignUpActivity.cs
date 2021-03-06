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
using Android.Support.Design.Widget;
using SendMe.ViewModel;
using SendMe.ViewModels;
using SendMe.Helpers;
using System.IO;
using Android.Util;
using Android.Graphics;
using SendMe.Droid.Helpers;
using Android.Content.PM;

namespace SendMe.Droid.Activities
{
    [Activity(Label = "@string/sing_up_header", LaunchMode = LaunchMode.SingleInstance, ConfigurationChanges = ConfigChanges.ScreenSize |
     ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, ParentActivity = typeof(MainActivity))]
    [MetaData("android.support.PARENT_ACTIVITY", Value = ".MainActivity")]
    public class SignUpActivity : BaseActivity
    {
        Button signUpButton;
        public string Logo { get; set; }
        public static readonly int PickImageId = 1000;
        EditText username, password, displayName, confirmPassword, courierMobileNumber, pricePerKM, vehiclebodytype;
        TextView courierCharges, message;
        AlertDialog vehiclebodytypeDialog;
        List<string> mSelectedItems;
        public bool FormIsValid { get; set; }
        public User User { get; set; }
        public SignUpViewModel ViewModel { get; set; }
        public BaseViewModel BaseModel { get; set; }
        ImageView profilePicture;
        protected override int LayoutResource => Resource.Layout.activity_sign_up;
        string[] items = {"Bakkie - Single Cab","Bakkie - Tipper", "Breakdown", "Bus", "Minibus", "Motorcycle", "Panel Van", "Passenger", "Truck", "Truck - Drop Side"};
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SupportActionBar.Title = "Sign Up As Courier";
            Initialize();
        }

        private async void SignUpButton_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            MessageDialog messageDialog = new MessageDialog();
            messageDialog.ShowLoading();
            message.Text = "";

            EncryptionHelper encryptionHelper = new EncryptionHelper();

            var _user = new User(){
                Username = username.Text.Trim(),
                DisplayName = displayName.Text.Trim(),
                Password = encryptionHelper.Encrypt(password.Text.Trim(), "Passw0rd@SendMe"),
                UserTypeId = 3,
                ProfilePicture = Logo,
                Courier = new Courier()
                {
                    VehicleBodyTypes = mSelectedItems,
                    MobileNumber = courierMobileNumber.Text.Trim(),
                    PricePerKM = Convert.ToDouble(pricePerKM.Text.Trim()),
                },
            };
            
           await ViewModel.SignUpUser(_user);
            if (ViewModel.Respond.ErrorOccurred)
                message.Text = ViewModel.Respond.Error.Message;
            else
            {
                messageDialog.SendToast("You are now registered to provide your service, Login to start making money");
                Finish();
            }
            messageDialog.HideLoading();
        }
        
        private bool ValidateForm()
        {
            Validations validation = new Validations();
            Android.Graphics.Drawables.Drawable icon = Resources.GetDrawable(Resource.Drawable.error);
            icon.SetBounds(0, 0, icon.IntrinsicWidth, icon.IntrinsicHeight);

            FormIsValid = true;

            if (string.IsNullOrEmpty(Logo))
            {
                MessageDialog messageDialog = new MessageDialog();
                messageDialog.SendToast("Select Profile Picture");
                FormIsValid = false;
            }

            if (mSelectedItems == null)
            {
                MessageDialog messageDialog = new MessageDialog();
                messageDialog.SendToast("Please Choose Your Vehicle Body Type");
                FormIsValid = false;
            }

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

            if (!validation.IsValidPhone(courierMobileNumber.Text))
            {
                courierMobileNumber.SetError("Invaild phone number", icon);
                FormIsValid = false;
            }
            if (!validation.IsRequired(pricePerKM.Text))
            {
                pricePerKM.SetError("This field is required", icon);
                FormIsValid = false;
            }

            if (!validation.IsRequired(displayName.Text))
            {
                displayName.SetError("This field is required", icon);
                FormIsValid = false;
            }
            return FormIsValid;
        }

       
        private void Initialize()
        {
            // Create your application here
            signUpButton = FindViewById<Button>(Resource.Id.button_sign_up);
            vehiclebodytype = FindViewById<EditText>(Resource.Id.signup_vehicle_body_type);
            username = FindViewById<EditText>(Resource.Id.signup_etUsername);
            courierMobileNumber = FindViewById<EditText>(Resource.Id.signup_courier_mobile_number);
            displayName = FindViewById<EditText>(Resource.Id.signup_etdisplay_name);
            confirmPassword = FindViewById<EditText>(Resource.Id.signup_confirm_password);
            password = FindViewById<EditText>(Resource.Id.signup_password);
            profilePicture = FindViewById<ImageView>(Resource.Id.signup_profile_picture);
            courierCharges = FindViewById<TextView>(Resource.Id.signup_tvCourierCharges);
            pricePerKM = FindViewById<EditText>(Resource.Id.signup_etPricePerKM);
            message = FindViewById<TextView>(Resource.Id.signup_tvmessage);
             
            ViewModel = new SignUpViewModel();

            // set OnCheckedChanged
            vehiclebodytype.Touch += OnVehiclebodytype_TextChanged;
            var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.choosevehiclebodytypes, Android.Resource.Layout.SimpleSpinnerDropDownItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            profilePicture.Click += SelectProfilePicture_Click;

            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Choose Your Vehicles Body Type");

            builder.SetMultiChoiceItems(Resource.Array.vehiclebodytypeSignUp_array, null, delegate {
               
                
            });
            builder.SetPositiveButton("OK", delegate {
                var sads = vehiclebodytypeDialog.ListView.CheckedItemPositions;
                List<string> selectedItems = new List<string>();

                vehiclebodytype.Text = "";
                for (int i = 0; i < items.Length; i++)
                {
                    var isChecked = sads.Get(i);
                    if (isChecked)
                    {
                        var fd = items.ElementAt(i);
                        selectedItems.Add(fd);
                        if (vehiclebodytype.Text.Length > 30)
                        {
                            vehiclebodytype.Text = vehiclebodytype.Text.Substring(0, 31);
                            vehiclebodytype.Text = vehiclebodytype.Text + "...";
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(vehiclebodytype.Text))
                            {
                                vehiclebodytype.Text = fd;
                            }
                            else
                            {
                                vehiclebodytype.Text = vehiclebodytype.Text + ", " + fd;
                            }
                        }

                    }
                }

                mSelectedItems = selectedItems;

                vehiclebodytypeDialog.Cancel();
            });
            vehiclebodytypeDialog = builder.Create();

            signUpButton.Click += SignUpButton_Click;
            
    }

        private void SelectProfilePicture_Click(object sender, EventArgs eventArgs)
        {
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);
        }
        public void OnVehiclebodytype_TextChanged(object b, EventArgs e)
        {
            vehiclebodytypeDialog.Show();
         
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
            {
                Context mContext = Android.App.Application.Context;
                ImageManager imageManager = new ImageManager(mContext);

                Android.Net.Uri uri = data.Data;
                profilePicture.SetImageURI(uri);

                profilePicture.DrawingCacheEnabled = true;

                profilePicture.BuildDrawingCache();

                Android.Graphics.Bitmap bm = profilePicture.GetDrawingCache(true);

                MemoryStream stream = new MemoryStream();
                bm.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, stream);
                byte[] byteArray = stream.ToArray();
                // String img_str = Base64.encodeToString(image, 0);
                Logo = Base64.EncodeToString(byteArray, 0);
                Bitmap bitmap = imageManager.ConvertStringToBitMap(Logo);
                profilePicture.SetImageBitmap(bitmap);
            }
        }

    }

}