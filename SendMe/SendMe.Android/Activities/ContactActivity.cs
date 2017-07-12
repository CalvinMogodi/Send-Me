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

namespace SendMe.Droid.Activities
{
    [Activity(Label = "Contact_Activity")]
    public class ContactActivity : Activity
    {
        EditText message;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Initialize();
        }

        private void Initialize()
        {
            // Create your application here
            SetContentView(Resource.Layout.activity_contact_us);
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