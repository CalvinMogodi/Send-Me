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

        RequestCourierViewModel requestCourierViewModel;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

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
            //getQuoteButton.Click += GetQuoteButton_Click;

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
    }
}