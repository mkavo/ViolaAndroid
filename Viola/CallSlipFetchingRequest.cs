using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Environment = System.Environment;

namespace Viola
{
    [Activity(Label = "", ScreenOrientation = ScreenOrientation.Portrait)]
    public class CallSlipFetchingRequest : Activity
    {
        private bool _isLoggedIn;
        string _filepath;
        private ListView _callSlipView;

        protected override void OnCreate(Bundle bundle)
        {
            SetTitle(Resource.String.fetchcallslips);
            SetContentView(Resource.Layout.CallSlipFetchingRequest);
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            _filepath = Path.Combine(path, "CallSlips.txt");
            base.OnCreate(bundle);
           
            //Fetch CallSlips from CallSlips.txt file
            Button fetchCallSlipsFromFile = FindViewById<Button>(Resource.Id.fetchCallSlipsFromFileButton);
            TextView message = FindViewById<TextView>(Resource.Id.textView3);
            TextView wrap1 = FindViewById<TextView>(Resource.Id.textView10);
            TextView textView12 = FindViewById<TextView>(Resource.Id.textView12);
            TextView textView13 = FindViewById<TextView>(Resource.Id.textView13);
            //TextView wrap2 = FindViewById<TextView>(Resource.Id.textView11);
            ImageView logo = FindViewById<ImageView>(Resource.Id.image1);

            Message a = new Message();
            //a.WriteToParcel();
            if (File.Exists(_filepath))
            {
                logo.Visibility = ViewStates.Gone;
                message.Visibility = ViewStates.Visible;
                textView12.Visibility = ViewStates.Visible;
                textView13.Visibility = ViewStates.Visible;
                fetchCallSlipsFromFile.Click += HandleButtonFetchCallSlipsFromFile;
            }
            else
            {
                textView12.Visibility = ViewStates.Gone;
                textView13.Visibility = ViewStates.Gone;
                //wrap1.Visibility = ViewStates.Gone;

                logo.Visibility = ViewStates.Gone;
                message.Visibility = ViewStates.Gone;
                fetchCallSlipsFromFile.Visibility = ViewStates.Invisible;
            }



            //Fetch CallSlips from Viola
            Button fetchNewCallSlips = FindViewById<Button>(Resource.Id.fetchNewCallSlipsButton);
            fetchNewCallSlips.Click += HandleButtonfetchNewCallSlips;

          
        }

        //Fetch CallSlips from CallSlips.txt file
         public void HandleButtonFetchCallSlipsFromFile(object sender, EventArgs e)
         {
            if (System.IO.File.Exists(_filepath)  && _filepath  != null)
            {
                var fetchCallSlipsActivity = new Intent(this, typeof(FetchCallSlips));
                StartActivity(fetchCallSlipsActivity);
            }
            else
            {
                Toast.MakeText(this, "Inga icke rapporterad lista", ToastLength.Long).Show();
            }
        }

        //Fetch CallSlips from Viola
        public void HandleButtonfetchNewCallSlips(object sender, EventArgs e) //samma som fethCallSlip från menu
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            _filepath = Path.Combine(path, "CallSlips.txt");
            File.Delete(_filepath);
            var fetchCallSlipsActivity = new Intent(this, typeof(FetchCallSlips));
            StartActivity(fetchCallSlipsActivity);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            base.OnCreateOptionsMenu(menu);
            MenuInflater inflater = this.MenuInflater;
            inflater.Inflate(Resource.Menu.MainMenu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            base.OnOptionsItemSelected(item);

            switch (item.ItemId)
            {
                case Resource.Id.MainMenu:
                    {
                        break;
                    }

                case Resource.Id.callLogOutButton:
                    {
                        Button buttonCallLogOut = FindViewById<Button>(Resource.Id.callLogOutButton);
                        var logoutActivity = new Intent(this, typeof(LogOut));
                        StartActivity(logoutActivity);
                        //StartActivityForResult(logoutActivity, 0);
                        //Toast.MakeText(this, "Det saknas Internetåtkomst", ToastLength.Long).Show();
                        break;
                    }

                case Resource.Id.fetchCallSlipsButton:
                    {
                        Button buttonFetchCallSlips = FindViewById<Button>(Resource.Id.fetchCallSlipsButton);
                        if (HasInternetConnection)
                        {
                            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                            _filepath = Path.Combine(path, "CallSlips.txt");
                            File.Delete(_filepath);
                            var fetchCallSlipsActivity = new Intent(this, typeof(FetchCallSlips));
                            StartActivity(fetchCallSlipsActivity);
                            
                        }
                        else
                        {
                            Toast.MakeText(this, "Det saknas Internetåtkomst", ToastLength.Long).Show();
                        }
                        break;
                    }

                case Resource.Id.reportFetchButton:
                    {
                        Button reportFetchButton = FindViewById<Button>(Resource.Id.reportFetchButton);

                        if (HasInternetConnection)
                        {
                            var callSlipBeforeReportFetchActivity = new Intent(this, typeof(CallSlipBeforeReport));
                            StartActivity(callSlipBeforeReportFetchActivity);
                            
                        }
                        else
                        {
                            Toast.MakeText(this, "Det saknas Internetåtkomst", ToastLength.Long).Show();
                        }

                        break;
                    }

                       default:
                    break;
            }

            return true;
        }

        public bool HasInternetConnection   //Can be private
        {
            get
            {
                var connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
                return connectivityManager.ActiveNetworkInfo != null && connectivityManager.ActiveNetworkInfo.IsConnectedOrConnecting;
            }
        }

    }
}