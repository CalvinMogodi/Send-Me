using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using SendMe.ViewModels;

namespace SendMe.Droid.Fragments
{
    public class RequestCourierFragment : Fragment
    {
        public static RequestCourierFragment NewInstance() =>  new RequestCourierFragment { Arguments = new Bundle() };

        Button getQuoteButton;
        EditText email, phone, name;
        TextView message;
        Spinner vehiclebodytype, itemSize;
        AutoCompleteTextView pickupLocation, dropLocation;
        RequestCourierViewModel requestCourierViewModel;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            View view = inflater.Inflate(Resource.Layout.activity_request_courier, container, false);
            //getQuoteButton = view.FindViewById<Button>(Resource.Id.requestCourier_getQuoteButton);
            //email = view.FindViewById<EditText>(Resource.Id.requestCourier_etemail);
            //phone = view.FindViewById<EditText>(Resource.Id.requestCourier_etphone);
            //name = view.FindViewById<EditText>(Resource.Id.requestCourier_etname);
            //pickupLocation = view.FindViewById<EditText>(Resource.Id.requestCourier_etpickup_location);
            //dropLocation = view.FindViewById<EditText>(Resource.Id.requestCourier_etdrop_location);
            //message = view.FindViewById<TextView>(Resource.Id.requestCourier_tvmessage);
            
            //requestCourierViewModel = new RequestCourierViewModel();
            //getQuoteButton.Click += GetQuoteButton_Click;
            return view;
        }
    }
}