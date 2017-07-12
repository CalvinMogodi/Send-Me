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

namespace SendMe.Droid.Activities
{
    [Activity(Label = "@string/sing_up_header")]
    public class SignUpActivity : Activity
    {
        Button signUpButton;
        public string Logo { get; set; }
        public static readonly int PickImageId = 1000;
        Spinner vehiclebodytype;
        EditText username, password, displayName, confirmPassword, courierMobileNumber, pricePerKM, extraCharges;
        TextView courierCharges, message;
        AlertDialog vehiclebodytypeDialog;
        List<string> mSelectedItems;
        public bool FormIsValid { get; set; }
        public User User { get; set; }
        public SignUpViewModel ViewModel { get; set; }
        public BaseViewModel BaseModel { get; set; }
        ImageView profilePicture;

        string[] items = {"Motorcycle","Passenger","Bakkie - Single Cab","Bakkie - Tipper", "Panel Van","Bus","Minibus","Truck - Drop Side", "Truck",};
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Initialize();
        }

        private void SignUpButton_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            MessageDialog messageDialog = new MessageDialog();
            messageDialog.ShowLoading();
            message.Text = "";

            var _user = new User(){
                Username = username.Text.Trim(),
                DisplayName = displayName.Text.Trim(),
                Password = password.Text.Trim(),
                UserTypeId = 3,
                ProfilePicture = Logo,
                CourierPrice = new CourierPrice()
                {
                    VehicleBodyTypes = mSelectedItems,
                    MobileNumber = courierMobileNumber.Text.Trim(),
                    PricePerKM = Convert.ToDouble(pricePerKM.Text.Trim()),
                    ExtraCharges = Convert.ToDouble(extraCharges.Text.Trim()),
                },
            };
            
            ViewModel.SignUpUser(_user);
            if (ViewModel.IsSignUp)
            {
                messageDialog.SendToast("You are now registered to provide your service, Login to start making money");
                Finish();
            }
               
            else
                message.Text = "Unable to sign you up, please try again";

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
                if (!validation.IsRequired(pricePerKM.Text))
                {
                    pricePerKM.SetError("This field is required", icon);
                    FormIsValid = false;
                }
                if (!validation.IsRequired(extraCharges.Text))
                {
                    extraCharges.SetError("This field is required", icon);
                    FormIsValid = false;
                }
           

            return FormIsValid;
        }

       
        private void Initialize()
        {
            // Create your application here
            SetContentView(Resource.Layout.activity_sign_up);
            signUpButton = FindViewById<Button>(Resource.Id.button_sign_up);
            vehiclebodytype = FindViewById<Spinner>(Resource.Id.signup_vehicle_body_type);
            username = FindViewById<EditText>(Resource.Id.signup_etUsername);
            courierMobileNumber = FindViewById<EditText>(Resource.Id.signup_courier_mobile_number);
            displayName = FindViewById<EditText>(Resource.Id.signup_etdisplay_name);
            confirmPassword = FindViewById<EditText>(Resource.Id.signup_confirm_password);
            password = FindViewById<EditText>(Resource.Id.signup_password);
            profilePicture = FindViewById<ImageView>(Resource.Id.signup_profile_picture);
            courierCharges = FindViewById<TextView>(Resource.Id.signup_tvCourierCharges);
            pricePerKM = FindViewById<EditText>(Resource.Id.signup_etPricePerKM);
            extraCharges = FindViewById<EditText>(Resource.Id.signup_etExtraCharges);
            message = FindViewById<TextView>(Resource.Id.signup_tvmessage);
             
            ViewModel = new SignUpViewModel();

            // set OnCheckedChanged
            vehiclebodytype.Touch += OnVehiclebodytype_TextChanged;
            var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.choosevehiclebodytypes, Android.Resource.Layout.SimpleSpinnerDropDownItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            vehiclebodytype.Adapter = adapter;
            profilePicture.Click += SelectProfilePicture_Click;

            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Choose Your Vehicles Body Type");

            builder.SetMultiChoiceItems(Resource.Array.vehiclebodytypeSignUp_array, null, delegate {
               
                
            });
            builder.SetPositiveButton("OK", delegate {
                var sads = vehiclebodytypeDialog.ListView.CheckedItemPositions;
                List<string> selectedItems = new List<string>();
                for (int i = 0; i < items.Length; i++)
                {                   
                    var isChecked = sads.Get(i);
                    if (isChecked)
                    {                       
                        var fd = items.ElementAt(i);
                        selectedItems.Add(fd);
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