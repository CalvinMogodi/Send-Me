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
using SendMe.ViewModels;
using SendMe.Models;
using SendMe.Droid.Helpers;
using System.IO;
using Android.Util;
using Android.Graphics;
using SendMe.Helpers;

namespace SendMe.Droid.Activities
{
    [Activity(Label = "ManageProfile")]
    public class ManageProfileActivity : Activity
    {
        Button signUpButton, cancelButton;
        public string Logo { get; set; }
        public static readonly int PickImageId = 1000;
        EditText username, password, displayName, confirmPassword, courierMobileNumber, pricePerKM, vehiclebodytype;
        TextView courierCharges, message;
        AlertDialog vehiclebodytypeDialog;
        List<string> mSelectedItems;
        public bool FormIsValid { get; set; }
        public User User { get; set; }
        public LoginViewModel ViewModel { get; set; }
        ImageView profilePicture;

        string[] items = { "Motorcycle", "Passenger", "Bakkie - Single Cab", "Bakkie - Tipper", "Panel Van", "Bus", "Minibus", "Truck - Drop Side", "Truck", };
        protected override async void OnCreate(Bundle savedInstanceState)
        {
           
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_manage_profile);
            MessageDialog messageDialog = new MessageDialog();
            messageDialog.ShowLoading();
            string userId = Intent.GetStringExtra("userId");
            ViewModel = new LoginViewModel();
            await ViewModel.GetUserById(userId);
            Initialize();
            messageDialog.HideLoading();
            // Create your application here
        }
        
        private async void CancelButton_Click(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(MainActivity));
            StartActivity(i);
        }
            private async void SignUpButton_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            MessageDialog messageDialog = new MessageDialog();
            messageDialog.ShowLoading();
            message.Text = "";

            var _user = new User()
            {
                Username = User.Username,
                DisplayName = displayName.Text.Trim(),
                ProfilePicture = Logo,
                Courier = new Courier()
                {
                    VehicleBodyTypes = mSelectedItems,
                    MobileNumber = courierMobileNumber.Text.Trim(),
                    PricePerKM = Convert.ToDouble(pricePerKM.Text.Trim()),
                },
            };

            await ViewModel.UpdateUserAsync(_user);
            if (ViewModel.Respond.ErrorOccurred)
                message.Text = ViewModel.Respond.Error.Message;
            else
            {
                messageDialog.SendToast("Profile is updated successfully.");
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
           
            signUpButton = FindViewById<Button>(Resource.Id.manage_profile_loginbutton);
            cancelButton = FindViewById<Button>(Resource.Id.manage_profile_cancelbutton);
            vehiclebodytype = FindViewById<EditText>(Resource.Id.manage_profile_vehicle_body_type);
            courierMobileNumber = FindViewById<EditText>(Resource.Id.manage_profile_courier_mobile_number);
            displayName = FindViewById<EditText>(Resource.Id.manage_profile_etdisplay_name);
            profilePicture = FindViewById<ImageView>(Resource.Id.manage_profile);
            pricePerKM = FindViewById<EditText>(Resource.Id.manage_profile_etPricePerKM);
            message = FindViewById<TextView>(Resource.Id.manage_profile_tvmessage);

            //Set Date
            bool[] checkedItems = new bool[9];
            if (ViewModel.User.Courier.VehicleBodyTypes != null)
            {
                foreach (var item in ViewModel.User.Courier.VehicleBodyTypes)
                {
                    int position = Array.IndexOf(items, item);
                    if (position != -1)
                    {
                        checkedItems[position] = true;
                        if (vehiclebodytype.Text.Length > 30)
                        {
                            vehiclebodytype.Text = vehiclebodytype.Text.Substring(0, 31);
                            vehiclebodytype.Text = vehiclebodytype.Text + "...";
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(vehiclebodytype.Text))
                                vehiclebodytype.Text = item;
                            else
                                vehiclebodytype.Text = vehiclebodytype.Text + ", " + item;
                        }
                    }

                }
            }
            User = new User();
            User = ViewModel.User;
            Context mContext = Android.App.Application.Context;
            ImageManager imageManager = new ImageManager(mContext);
            courierMobileNumber.Text = ViewModel.User.Courier.MobileNumber;
            displayName.Text = ViewModel.User.DisplayName;
            pricePerKM.Text = ViewModel.User.Courier.PricePerKM.ToString();
            if (ViewModel.User.ProfilePicture == null)
            {
                profilePicture.SetImageResource(Resource.Drawable.profile_generic);
            }
            else {
                profilePicture.SetImageBitmap(imageManager.ConvertStringToBitMap(ViewModel.User.ProfilePicture));
            }
        
            // set OnCheckedChanged
            vehiclebodytype.Touch += OnVehiclebodytype_TextChanged;
            var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.choosevehiclebodytypes, Android.Resource.Layout.SimpleSpinnerDropDownItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            profilePicture.Click += SelectProfilePicture_Click;

            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Choose Your Vehicles Body Type");

            builder.SetMultiChoiceItems(Resource.Array.vehiclebodytypeSignUp_array, checkedItems, delegate {


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
            cancelButton.Click += CancelButton_Click;

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