using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static Android.Media.Audiofx.Equalizer;

namespace SendMe.Droid.Helpers
{
    public class GPSTracker : Android.Locations.ILocationListener
    {
        private Context mContext;

        

        // flag for GPS status
        bool isGPSEnabled = false;

        // flag for network status
        bool isNetworkEnabled = false;

        // flag for GPS status
        bool canGetLocation = false;

        Location location = null; // location
        double latitude; // latitude
        double longitude; // longitude

        // The minimum distance to change Updates in meters
        private static long MIN_DISTANCE_CHANGE_FOR_UPDATES = 10; // 10 meters

        // The minimum time between updates in milliseconds
        private static long MIN_TIME_BW_UPDATES = 1000 * 60 * 1; // 1 minute

        // Declaring a Location Manager
        protected LocationManager locationManager;

        public IntPtr Handle => throw new NotImplementedException();

        //public GPSTracker(Context context)
        //{
        //    this.mContext = context;
        //    getLocation();
        //}

        public Location GetLocation(Context context)
        {
            try
            {
                this.mContext = context;
                locationManager = (LocationManager)mContext.GetSystemService("location");

                // getting GPS status

                isGPSEnabled = locationManager.IsProviderEnabled(LocationManager.GpsProvider);

                // getting network status
                isNetworkEnabled = locationManager.IsProviderEnabled(LocationManager.NetworkProvider);

                if (!isGPSEnabled && !isNetworkEnabled)
                {
                    return location;
                }
                else
                {
                    canGetLocation = true;
                    //if (isNetworkEnabled)
                    //{
                    //    locationManager.RequestLocationUpdates(LocationManager.NetworkProvider,MIN_TIME_BW_UPDATES,MIN_DISTANCE_CHANGE_FOR_UPDATES, this);
                    //    //Log.d("Network", "Network Enabled");
                    //    if (locationManager != null)
                    //    {
                    //        location = locationManager.GetLastKnownLocation(LocationManager.NetworkProvider);
                    //        if (location != null)
                    //        {
                    //            latitude = location.Latitude;
                    //            longitude = location.Longitude;
                    //        }
                    //    }
                    //}
                    // if GPS Enabled get lat/long using GPS Services
                    if (isGPSEnabled)
                    {
                        if (location == null)
                        {
                            //LocationManager locationManager = (LocationManager)context.GetSystemService("");
                            //var currentLocation = locationManager.GetLastKnownLocation(LocationManager.GpsProvider);

                            //locationManager.RequestLocationUpdates(
                            //        LocationManager.GpsProvider,
                            //        MIN_TIME_BW_UPDATES,
                            //        MIN_DISTANCE_CHANGE_FOR_UPDATES, this);
                            //Log.d("GPS", "GPS Enabled");
                            if (locationManager != null)
                            {
                                location = locationManager.GetLastKnownLocation(LocationManager.GpsProvider);
                                if (location != null)
                                {
                                    latitude = location.Latitude;
                                    longitude = location.Longitude;
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                //e.printStackTrace();
            }

            return location;
        }

        /**
 * Stop using GPS listener Calling this function will stop using GPS in your
 * app
 * */
        public void stopUsingGPS()
        {
            if (locationManager != null)
            {
                locationManager.RemoveUpdates(this);
            }
        }

        /**
         * Function to get latitude
         * */
        public double getLatitude()
        {
            if (location != null)
            {
                latitude = location.Latitude;
            }

            // return latitude
            return latitude;
        }

        /**
         * Function to get longitude
         * */
        public double getLongitude()
        {
            if (location != null)
            {
                longitude = location.Longitude;
            }

            // return longitude
            return longitude;
        }

        /**
 * Function to check GPS/wifi enabled
 * 
 * @return boolean
 * */


        /**
         * Function to show settings alert dialog On pressing Settings button will
         * lauch Settings Options
         * */
        

        public void OnLocationChanged(Location location)
        {
            throw new NotImplementedException();
        }

        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }  
    }
}