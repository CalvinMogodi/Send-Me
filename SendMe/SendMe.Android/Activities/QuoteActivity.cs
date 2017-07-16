﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Provider;
using Java.Lang;
using Android.Telephony;
using SendMe.Droid.Helpers;
using System.Globalization;
using SendMe.ViewModels;
using SendMe.Helpers;
using SendMe.Models;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SendMe.Droid.Activities
{
    [Activity(Label = "QuoteActivity")]
    public class QuoteActivity : Activity
    {
        QuotesViewModel quotesViewModel;
        Request Request;
        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_quote);

            MessageDialog messageDialog = new MessageDialog();
            messageDialog.ShowLoading();
            var request = Intent.GetStringExtra("request");
            Request item = JsonConvert.DeserializeObject<Request>(request);

            Request = new Request();
            Request = item;
            quotesViewModel = new QuotesViewModel();
            await quotesViewModel.GetQuotes(Request);

            if (quotesViewModel.Quotes.Count > 0)
            {
                var contactsAdapter = new ContactsAdapter(this, quotesViewModel.Quotes);
                var contactsListView = FindViewById<ListView>(Resource.Id.ContactsListView);
                contactsListView.Adapter = contactsAdapter;
                contactsListView.ItemClick += ItemClick;
                SaveQuoteRequest(quotesViewModel.Quotes, Request, quotesViewModel);
            }
            else
            {
                messageDialog.SendToast("There are no couriers available within 50KM from your pick up location.");
            }
            messageDialog.HideLoading();
        }

        private void ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var quote = quotesViewModel.Quotes[e.Position];

            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            // 2. Chain together various setter methods to set the dialog characteristics
            builder.SetMessage("Please note that the quote might change base on courier rates.")
                .SetTitle("Contact Me")
                .SetPositiveButton("Call Me", delegate
                {
                    var request = Request;
                    string smstext = string.Format("Send Me: Hi {0}, {1} would like you to contact him/her regarding your courier service.Request Details : Pick Up Location - {2}. Drop Location - {3}. Contact Person - {4}. Email - {5}. Mobile Number - {6}",
                                        quote.CourierName, request.Name, request.FromLocation.Description, request.Tolocation.Description, request.Name, request.Email, Request.MobileNumber);

                    MessageDialog messageDialog = new MessageDialog();

                    messageDialog.SendSMS(quote.CourierMobileNumber, smstext);

                }).SetNegativeButton("Not Now", delegate
                {

                });

            // 3. Get the AlertDialog from create()
            AlertDialog dialog = builder.Create();
            dialog.Show();
        }

        public async void SaveQuoteRequest(ObservableRangeCollection<Quote> quotes, Request request, QuotesViewModel quotesViewModel) {

            ObservableRangeCollection<Quote> ada = quotesViewModel.Quotes;
            string df = string.Format("{0} - {1}", Build.Manufacturer, Build.Model);
            string ddf = Build.VERSION.Release.ToString();
            DateTime dt = DateTime.Now;
            QuoteRequest quoteRequest = new QuoteRequest()
            {
                Phone = df,
                OSVersion = ddf,
                RequestDatetime = dt,
                Request = request,
                Quotes = ada,
            };

             quotesViewModel.SaveQuoteRequest(quoteRequest);
        }
        
    }

    public class Category
    {
        private QuoteRequest objects;

        public Category()
        {
        }

        public QuoteRequest getObjects()
        {
            return objects;
        }

        public void setObjects(QuoteRequest objects)
        {
            this.objects = objects;
        }
    }

    public class ContactsAdapter : BaseAdapter
    {
        List<Contact> _contactList;
        List<Quote> _quoteList;
        Activity _activity;

        //public QuotesAdapter(Activity activity, ObservableRangeCollection<Quote> quotes)
        //{
        //    _activity = activity;
        //    FillContacts();
        //    //_quoteList = quotes.ToList();
        //}
        public ContactsAdapter(Activity activity, ObservableRangeCollection<Quote> quotes)
        {
            _activity = activity;
            _quoteList = quotes.ToList();
            FillContacts();
        }

        public override int Count
        {
            get { return _quoteList.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            // could wrap a Contact in a Java.Lang.Object
            // to return it here if needed
            return null;
        }

        public override long GetItemId(int position)
        {
            return position;
        }
       
        public override View GetView(int position, View convertView, ViewGroup parent)
        {

            var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.item_browse, parent, false);
            var contactName = view.FindViewById<TextView>(Resource.Id.contactName);
            var contactImage = view.FindViewById<ImageView>(Resource.Id.imageView1);
            var courierDescription = view.FindViewById<TextView>(Resource.Id.courierDescription);

            //if (_contactList[position].PhotoId == null)
            //{
             //   contactImage = view.FindViewById<ImageView>(Resource.Id.ContactImage);
            //    contactImage.SetImageResource(Resource.Drawable.profile_generic);
            //}
            //else
            //{
            //    var contactUri = ContentUris.WithAppendedId(
            //        ContactsContract.Contacts.ContentUri, _contactList[position].Id);
            //    var contactPhotoUri = Android.Net.Uri.WithAppendedPath(contactUri,
            //        Contacts.Photos.ContentDirectory);
            //    contactImage.SetImageURI(contactPhotoUri);
            //}

            //var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.quote_listItem, parent, false);
            //var quote = _quoteList[position];

            var quotePriceAndCourierKM = view.FindViewById<TextView>(Resource.Id.quotePriceAndCourierKM);
           
            var courierImage = view.FindViewById<ImageView>(Resource.Id.courierImage);

            Context mContext = Android.App.Application.Context;
            ImageManager imageManager = new ImageManager(mContext);


            TelephonyManager tm = (TelephonyManager)mContext.GetSystemService(Context.TelephonyService);
            string countryCode = string.Format("en-{0}", tm.SimCountryIso.ToUpper());
            System.Globalization.RegionInfo RegionInfo = new System.Globalization.RegionInfo(new CultureInfo(countryCode, false).LCID);

            //// Replace the contents of the view with that element
            ////var myHolder = holder as MyViewHolder;
            courierDescription.Text = _quoteList[position].CourierName;
            contactName.Text = string.Format("{0} {1} - {2} KM", RegionInfo.CurrencySymbol, _quoteList[position].Price.ToString("F"), _quoteList[position].CourierKmDistance.ToString("F"));
            contactImage.SetImageBitmap(imageManager.ConvertStringToBitMap(_quoteList[position].CourierProfilePicture));

            return view;
        }
        
        void FillContacts()
        {
            var uri = ContactsContract.Contacts.ContentUri;

            string[] projection = {
                ContactsContract.Contacts.InterfaceConsts.Id,
                ContactsContract.Contacts.InterfaceConsts.DisplayName,
                ContactsContract.Contacts.InterfaceConsts.PhotoId
            };

            var cursor = _activity.ManagedQuery(uri, projection, null,null, null);

            _contactList = new List<Contact>();

            if (cursor.MoveToFirst())
            {
                do
                {
                    _contactList.Add(new Contact
                    {
                        Id = cursor.GetLong(
                    cursor.GetColumnIndex(projection[0])),
                        DisplayName = cursor.GetString(
                    cursor.GetColumnIndex(projection[1])),
                        PhotoId = cursor.GetString(
                    cursor.GetColumnIndex(projection[2]))
                    });
                } while (cursor.MoveToNext());
            }
        }

        class Contact
        {
            public long Id { get; set; }
            public string DisplayName { get; set; }
            public string PhotoId { get; set; }
        }
    }
}