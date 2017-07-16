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
using Android.Support.V4.Widget;
using System.Threading.Tasks;
using Android.Support.V7.Widget;

namespace SendMe.Droid.Activities
{
    [Activity(Label = "ViewQuoteActivity")]
    public class ViewQuoteActivity : Activity
    {
        BrowseItemsAdapter adapter;
        SwipeRefreshLayout refresher;
        Task loadItems;

        ProgressBar progress;
        public QuotesViewModel ViewModel
        {
            get;
            set;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var request = Intent.GetStringExtra("request");
            SendMe.Models.Request item = Newtonsoft.Json.JsonConvert.DeserializeObject<SendMe.Models.Request>(request);
            

            Initialize();
        }

        private void Initialize()
        {
            // Create your application here
            SetContentView(Resource.Layout.activity_view_quote);
            var recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            recyclerView.HasFixedSize = true;
            recyclerView.SetAdapter(adapter = new BrowseItemsAdapter(this, ViewModel));

            refresher = FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);

            refresher.SetColorSchemeColors(Resource.Color.accent);

            if (ViewModel.Quotes.Count == 0)
                loadItems.Wait();

            adapter.ItemClick += Adapter_ItemClick;
        }
        private void Adapter_ItemClick(object sender, RecyclerClickEventArgs e)
        {
            var quote = ViewModel.Quotes[e.Position];

            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            // 2. Chain together various setter methods to set the dialog characteristics
            builder.SetMessage("Please note that the quote might change base on courier rates.")
                .SetTitle("Contact Me")
                .SetPositiveButton("Call Me", delegate
                {
                    string smstext = String.Format("Send Me: Hi {0}, {1} would like you to contact him/her regarding your courier service.Request Details : Pick Up Location - {2}. Drop Location - {3}. Contact Person - {4}. Email - {5}. Mobile Number - {6}" + quote.CourierName, quote.Request.Name, quote.Request.FromLocation, quote.Request.Tolocation, quote.Request.Name, quote.Request.Email, quote.Request.MobileNumber);

                    MessageDialog messageDialog = new MessageDialog();

                    messageDialog.SendSMS(quote.CourierMobileNumber, smstext);

                }).SetNegativeButton("Not Now", delegate
                {

                });

            // 3. Get the AlertDialog from create()
            AlertDialog dialog = builder.Create();
            dialog.Show();
        }
    }
}