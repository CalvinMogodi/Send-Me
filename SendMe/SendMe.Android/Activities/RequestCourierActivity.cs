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
using Android.Content.PM;
using SendMe.ViewModels;
using Android.Graphics;
using SendMe.Helpers;
using SendMe.Models;

namespace SendMe.Droid.Activities
{
    [Activity(Label = "Request Courier")]
    public class RequestCourierActivity : Activity
    {
        Button getQuoteButton;
        EditText email, phone, name;
        TextView message;
        Spinner vehiclebodytype, itemSize;
        AutoCompleteTextView pickupLocation, dropLocation;
        public Toolbar Toolbar
        {
            get;
            set;
        }

        RequestCourierViewModel requestCourierViewModel;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            
            Initialize();
            
        }


        public void Initialize() {

            // Create your application here
            SetContentView(Resource.Layout.activity_request_courier);
            getQuoteButton = FindViewById<Button>(Resource.Id.requestCourier_getQuoteButton);
            email = FindViewById<EditText>(Resource.Id.requestCourier_etemail);
            phone = FindViewById<EditText>(Resource.Id.requestCourier_etphone);
            name = FindViewById<EditText>(Resource.Id.requestCourier_etname);
            message = FindViewById<TextView>(Resource.Id.requestCourier_tvmessage);
            vehiclebodytype = FindViewById<Spinner>(Resource.Id.requestCourier_vehiclebodytype);

            requestCourierViewModel = new RequestCourierViewModel();
            getQuoteButton.Click += GetQuoteButton_Click;

            //set vehicle body type drop down
            List<string> mylist = new List<string>();
            var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.vehiclebodytypes_array, Android.Resource.Layout.SimpleSpinnerDropDownItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            vehiclebodytype.Adapter = adapter;
            //vehiclebodytype.ItemSelected += VehiclebodytypeItemSelected;

            //set item size drop down
            itemSize = FindViewById<Spinner>(Resource.Id.requestCourier_ItemSize);
            var itemSizeAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.itemSize_array, Android.Resource.Layout.SimpleSpinnerDropDownItem);
            itemSizeAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            itemSize.Adapter = itemSizeAdapter;

            pickupLocation = FindViewById<AutoCompleteTextView>(Resource.Id.requestCourier_actvpickup_location);
            dropLocation = FindViewById<AutoCompleteTextView>(Resource.Id.requestCourier_actvpickup_location);
            //pickupLocation.TextChanged += OnDDTouchEvent;
        }

        private void GetQuoteButton_Click(object sender, EventArgs e)
        {
            //if (!ValidateForm())
            //    return;

            Point fromLocation = new Point()
            {
                X = 69,
                Y = 34,
            };
            Point tolocation = new Point()
            {
                X = 20,
                Y = 41,
            };

            SendMe.Models.Request request = new SendMe.Models.Request()
            {
                FromLocation = new Models.Location() {
                    Longitude = fromLocation.Y,
                    Latitude = fromLocation.X,
                       },
                Tolocation = new Models.Location()
                {
                    Longitude = tolocation.Y,
                    Latitude = tolocation.X,
                },
                PackageSize = name.Text,
                MobileNumber = phone.Text,
                Email = email.Text,
                Name = name.Text,
            };
            Intent i = new Intent(this, typeof(ViewQuoteActivity));
            i.PutExtra("request", Newtonsoft.Json.JsonConvert.SerializeObject(request));
            StartActivity(i);
        }

        private bool ValidateForm()
        {
            Validations validation = new Validations();
            Android.Graphics.Drawables.Drawable icon = Resources.GetDrawable(Resource.Drawable.error);
            icon.SetBounds(0, 0, 0, 0);

            bool formIsValid = true;

            //if (vehiclebodytype.Text == "Select Vehicle Body Type")
            //{
            //    MessageDialog messageDialog = new MessageDialog();
            //    messageDialog.SendToast("Please Select Vehicle Body Type");
            //    formIsValid = false;
            //}

            if (!validation.IsValidEmail(email.Text))
            {
                email.SetError("Invalid email address", icon);
                formIsValid = false;
            }

            if (!validation.IsValidPhone(phone.Text))
            {
                phone.SetError("Invaild phone number", icon);
                formIsValid = false;
            }
            if (!validation.IsRequired(name.Text))
            {
                name.SetError("This field is required", icon);
                formIsValid = false;
            }
            if (!validation.IsRequired(pickupLocation.Text))
            {
                pickupLocation.SetError("This field is required", icon);
                formIsValid = false;
            }
            if (!validation.IsRequired(dropLocation.Text))
            {
                dropLocation.SetError("This field is required", icon);
                formIsValid = false;
            }

            return formIsValid;
        }
    }
}