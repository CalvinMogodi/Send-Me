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
using SendMe.Helpers;
using Android.Content.PM;

namespace SendMe.Droid.Activities
{
    [Activity(Label = "Contact_Activity", LaunchMode = LaunchMode.SingleInstance, ConfigurationChanges = ConfigChanges.ScreenSize |
     ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, ParentActivity = typeof(MainActivity))]
    [MetaData("android.support.PARENT_ACTIVITY", Value = ".MainActivity")]
    public class ContactActivity : BaseActivity
    {
        EditText message;
        protected override int LayoutResource => Resource.Layout.activity_contact_us;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SupportActionBar.Title = "Contact Us";            
            Initialize();
        }

        private void Initialize()
        {
            // Create your application here
            Button sendButton = FindViewById<Button>(Resource.Id.contact_us_send_button);
            message = FindViewById<EditText>(Resource.Id.contact_us_message);
            sendButton.Click += SendSMSButton_Click;
        }

        void SendSMSButton_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            MessageDialog messageDialog = new MessageDialog();
            messageDialog.SendSMS(message.Text.Trim());
            //messageDialog.SendEmail(message.Text.Trim());
            message.Text = "";
        }

        private bool ValidateForm()
        {
            Validations validation = new Validations();
            Android.Graphics.Drawables.Drawable icon = Resources.GetDrawable(Resource.Drawable.error);
            icon.SetBounds(0, 0, icon.IntrinsicWidth, icon.IntrinsicHeight);

            bool formIsValid = true;
                       
            if (!validation.IsRequired(message.Text))
            {
                message.SetError("This field is required", icon);
                formIsValid = false;
            }
            return formIsValid;
        }
        }
}