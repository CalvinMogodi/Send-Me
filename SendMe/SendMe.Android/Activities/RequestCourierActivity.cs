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
using Android.Support.Design.Widget;
using Java.Net;
using Java.IO;
using Org.Json;
using Newtonsoft.Json.Linq;

namespace SendMe.Droid.Activities
{
    [Activity(Label = "@string/request_courier_header",
        LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class RequestCourierActivity : Activity
    {
        Button getQuoteButton;
        AppBarLayout appBar;
        EditText email, phone, name;
        TextView message;
        Spinner vehiclebodytype, itemSize;
        AutoCompleteTextView pickupLocation, dropLocation, autocompleteTextView;
        List<Location> PickUpLocations = new List<Location>();
        List<Location> DropLocations = new List<Location>();

        private String PLACES_API_BASE = "https://maps.googleapis.com/maps/api/place";
        private String TYPE_AUTOCOMPLETE = "/autocomplete";
        private String OUT_JSON = "/json";
        private String API_KEY = "AIzaSyBxzMOzDddAIUKR3RlINgbhtTReEGCvEKI";

        //private static string LOG_TAG = "Google Places Autocomplete";
        //private static string PLACES_API_BASE = "https://maps.googleapis.com/maps/api/place";
        //private static string TYPE_AUTOCOMPLETE = "/autocomplete";
        //private static string OUT_JSON = "/json";
        //private static string API_KEY = "AIzaSyCvXMXsHtLL2zCDR_wb6nhrW_iwO6xWy2g";

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
            appBar = FindViewById<AppBarLayout>(Resource.Id.appbar);
            
            requestCourierViewModel = new RequestCourierViewModel();
            getQuoteButton.Click += GetQuoteButton_Click;

            //set vehicle body type drop down
            List<string> mylist = new List<string>();
            var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.vehiclebodytypes_array, Android.Resource.Layout.SimpleSpinnerDropDownItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            vehiclebodytype.Adapter = adapter;
            vehiclebodytype.ItemSelected += VehiclebodytypeItemSelected;

            //set item size drop down
            itemSize = FindViewById<Spinner>(Resource.Id.requestCourier_ItemSize);
            var itemSizeAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.itemSize_array, Android.Resource.Layout.SimpleSpinnerDropDownItem);
            itemSizeAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            itemSize.Adapter = itemSizeAdapter;

            pickupLocation = FindViewById<AutoCompleteTextView>(Resource.Id.requestCourier_actvpickup_location);
            dropLocation = FindViewById<AutoCompleteTextView>(Resource.Id.requestCourier_actvdrop_location);

            pickupLocation.TextChanged += PickupLocationAutocomplete;
            dropLocation.TextChanged += DropLocationAutocomplete;
            itemSize.Visibility = ViewStates.Gone;
        }


        public void VehiclebodytypeItemSelected(object sender, EventArgs e)
        {
            var selectedItem = vehiclebodytype.SelectedItem.ToString();
            if (selectedItem == "Motorcycle" || selectedItem == "Passenger")
                itemSize.Visibility = ViewStates.Gone;
            else
                itemSize.Visibility = ViewStates.Visible;
        }
        public void PickupLocationAutocomplete(object sender, EventArgs e)
        {
            string input = pickupLocation.Text.Trim();
            if (input.Length > 1)
            {

           
            List<String> resultList = null;
            HttpURLConnection conn = null;
            StringBuilder jsonResults = new StringBuilder();
            try
            {
                if (Convert.ToInt32(Android.OS.Build.VERSION.SdkInt) > 9)
                {
                    StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().PermitAll().Build();
                    StrictMode.SetThreadPolicy(policy);
                }

                StringBuilder sb = new StringBuilder(PLACES_API_BASE + TYPE_AUTOCOMPLETE + OUT_JSON);
                sb.Append("?key=" + API_KEY);
                sb.Append("&libraries=places");
                sb.Append("&input=" + URLEncoder.Encode(input, "utf8"));

                URL url = new URL(sb.ToString());
                conn = (HttpURLConnection)url.OpenConnection();
                conn.Connect();
                InputStreamReader ist = new InputStreamReader(conn.InputStream);

                int read;
                char[] buff = new char[1024];
                while ((read = ist.Read(buff)) != -1)
                    jsonResults.Append(buff, 0, read);
            }
            catch (MalformedURLException error)
            {

            }
            catch (IOException error)
            {

            }
            finally
            {
                if (conn != null)
                    conn.Disconnect();
            }

            try
            {
                JSONObject jsonObj = new JSONObject(jsonResults.ToString());
                JSONArray predsJsonArray = jsonObj.GetJSONArray("predictions");

                resultList = new List<String>(predsJsonArray.Length());
                PickUpLocations = new List<Location>();
                for (int i = 0; i < predsJsonArray.Length(); ++i)
                {

                    Location location = new Location()
                    {
                        Description = predsJsonArray.GetJSONObject(i).GetString("description"),
                        PlaceId = predsJsonArray.GetJSONObject(i).GetString("place_id"),
                    };
                    PickUpLocations.Add(location);
                    resultList.Add(predsJsonArray.GetJSONObject(i).GetString("description"));
                }                    
            }
            catch (JSONException error)
            {

            }

            string[] resultLists = resultList.ToArray();
            ArrayAdapter autoCompleteAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, resultLists);
                pickupLocation.Adapter = autoCompleteAdapter;
            }
        }

        public Location GetLocationDetails(string placeId, string description)
        {
            Location location = new Location();
            HttpURLConnection conn = null;
            StringBuilder jsonResults = new StringBuilder();
            try
            {
                if (Convert.ToInt32(Android.OS.Build.VERSION.SdkInt) > 9)
                {
                    StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().PermitAll().Build();
                    StrictMode.SetThreadPolicy(policy);
                }

                StringBuilder sb = new StringBuilder("https://maps.googleapis.com/maps/api/place/details/json");
                sb.Append("?key=" + API_KEY);
                sb.Append("&placeid=" + placeId);


                URL url = new URL(sb.ToString());

                conn = (HttpURLConnection)url.OpenConnection();
                conn.Connect();
                InputStreamReader un = new InputStreamReader(conn.InputStream);

                // Load the results into a StringBuilder
                int read;
                char[] buff = new char[1024];
                while ((read = un.Read(buff)) != -1)
                {
                    jsonResults.Append(buff, 0, read);
                }
            }
            catch (MalformedURLException e)
            {
               
            }
            catch (IOException e)
            {
                
            }
            finally
            {
                if (conn != null)
                {
                    conn.Disconnect();
                }
            }

            try
            {

                // Create a JSON object hierarchy from the results
                JSONObject jsonObj = new JSONObject(jsonResults.ToString());
                JObject jObject = JObject.Parse(jsonObj.ToString());
                JToken result = jObject["result"];
                JToken geometryObject = result["geometry"];
                JToken locationObject = geometryObject["location"];
                var longitude = (string)locationObject["lng"];
                var latitude = (string)locationObject["lat"];
                location = new Location()
                    {
                        Longitude = Convert.ToDouble(longitude),
                        Latitude = Convert.ToDouble(latitude),
                        Description = description,
                        PlaceId = placeId
                };
               
            }
            catch (JSONException e)
            {
            }

            return location;
        }

        public void DropLocationAutocomplete(object sender, EventArgs e)
        {
            string input = dropLocation.Text.Trim();
            if (input.Length > 1)
            {


                List<String> resultList = null;
                HttpURLConnection conn = null;
                StringBuilder jsonResults = new StringBuilder();
                try
                {
                    if (Convert.ToInt32(Android.OS.Build.VERSION.SdkInt) > 9)
                    {
                        StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().PermitAll().Build();
                        StrictMode.SetThreadPolicy(policy);
                    }

                    StringBuilder sb = new StringBuilder(PLACES_API_BASE + TYPE_AUTOCOMPLETE + OUT_JSON);
                    sb.Append("?key=" + API_KEY);
                    sb.Append("&libraries=places");
                    sb.Append("&input=" + URLEncoder.Encode(input, "utf8"));

                    URL url = new URL(sb.ToString());
                    conn = (HttpURLConnection)url.OpenConnection();
                    conn.Connect();
                    InputStreamReader ist = new InputStreamReader(conn.InputStream);

                    int read;
                    char[] buff = new char[1024];
                    while ((read = ist.Read(buff)) != -1)
                        jsonResults.Append(buff, 0, read);
                }
                catch (MalformedURLException error)
                {

                }
                catch (IOException error)
                {

                }
                finally
                {
                    if (conn != null)
                        conn.Disconnect();
                }

                try
                {
                    JSONObject jsonObj = new JSONObject(jsonResults.ToString());
                    JSONArray predsJsonArray = jsonObj.GetJSONArray("predictions");

                    resultList = new List<String>(predsJsonArray.Length());
                    DropLocations = new List<Location>();
                    for (int i = 0; i < predsJsonArray.Length(); ++i)
                    {

                        Location location = new Location()
                        {
                            Description = predsJsonArray.GetJSONObject(i).GetString("description"),
                            PlaceId = predsJsonArray.GetJSONObject(i).GetString("place_id"),
                        };
                        DropLocations.Add(location);
                        resultList.Add(predsJsonArray.GetJSONObject(i).GetString("description"));
                    }
                }
                catch (JSONException error)
                {

                }

                string[] resultLists = resultList.ToArray();
                ArrayAdapter autoCompleteAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, resultLists);
                dropLocation.Adapter = autoCompleteAdapter;
            }
        }



        private decimal kmDistance(double lat1, double lon1, double lat2, double lon2)
        {// generally used geo measurement function: 
            double R = 6378.137;            // Radius of earth in KM 
            double dLat = lat2 * Math.PI / 180 - lat1 * Math.PI / 180;
            double dLon = lon2 * Math.PI / 180 - lon1 * Math.PI / 180;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c;
            return (decimal)d; // Km returned 
        }

        private void GetQuoteButton_Click(object sender, EventArgs e)
        {
          
            if (!ValidateForm())
                return;

            string pickupLocationPlaceId = PickUpLocations.FirstOrDefault(l => l.Description.Trim() == pickupLocation.Text.Trim()).PlaceId;
            string dropLocationPlaceId = DropLocations.FirstOrDefault(l => l.Description.Trim() == dropLocation.Text.Trim()).PlaceId;

            var fromLocation = GetLocationDetails(pickupLocationPlaceId, pickupLocation.Text.Trim());
            var tolocation = GetLocationDetails(dropLocationPlaceId, pickupLocation.Text.Trim());

            Request request = new Request()
            {
                FromLocation = fromLocation,
                Tolocation = tolocation,
                PackageSize = itemSize.SelectedItem.ToString(),
                VehicleBodyType = vehiclebodytype.SelectedItem.ToString(),
                MobileNumber = phone.Text,
                Email = email.Text,
                Name = name.Text,
            };
            Intent i = new Intent(this, typeof(QuoteActivity));
            i.PutExtra("request", Newtonsoft.Json.JsonConvert.SerializeObject(request));
            StartActivity(i);
            
        }

        private bool ValidateForm()
        {
            Validations validation = new Validations();
            Android.Graphics.Drawables.Drawable icon = Resources.GetDrawable(Resource.Drawable.error);
            icon.SetBounds(0, 0, icon.IntrinsicWidth, icon.IntrinsicHeight);

            bool formIsValid = true;

            if (vehiclebodytype.SelectedItem.ToString() == "Select Vehicle Body Type")
            {
                MessageDialog messageDialog = new MessageDialog();
                messageDialog.SendToast("Please Select Vehicle Body Type");
                formIsValid = false;
            }

            if (itemSize.Visibility == ViewStates.Visible)
            {
                if (itemSize.SelectedItem.ToString() == "Select Item Size")
                {
                    MessageDialog messageDialog = new MessageDialog();
                    messageDialog.SendToast("Please Select Item Size");
                    formIsValid = false;
                }
            }            

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