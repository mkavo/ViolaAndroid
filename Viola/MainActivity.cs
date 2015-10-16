using System;
using Android;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Net;
using Android.Provider;
using Android.Views;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Net.Security;
using System.Text;
using Newtonsoft.Json;
using Java.IO;
using Java.Util;
using Environment = System.Environment;



namespace Viola
{
    [Activity(Label = "", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : Activity
	{
		//private ListView _callSlipView;
		string _filepath;
        private bool _isLoggedIn;
      		  
        protected override void OnCreate(Bundle bundle)
		{
            base.OnCreate(bundle);
            Rewire();
        }
        
        protected override void OnResume()
        {
            base.OnResume();
            if (!_isLoggedIn)
            {
                Rewire();
            }
        }

        private void Rewire()
        {
			SetTitle(Resource.String.app_name);
            SetContentView(Resource.Layout.Main);

            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            _filepath = Path.Combine(path, "CallSlips.txt");
            var prefs = Application.Context.GetSharedPreferences("Violadev", FileCreationMode.Private);
            _isLoggedIn = prefs.GetBoolean("Authorized", false);
            if (!_isLoggedIn)
            {
                var loginActivity = new Intent(this, typeof (LogIn));
                StartActivity(loginActivity);
            }
            else
            {
                //TODO: kolla om det finns en befintlig callslip.txt (innebär att ingen återrapportering skett) och 
                //i så fall fråga om den gamla ska visas eller återrapporteras före en ny plocklista läses in

                SetContentView(Resource.Layout.CallSlipFetchingRequest);
                var fetchinrequest = new Intent(this, typeof (CallSlipFetchingRequest));
                StartActivity(fetchinrequest);
            }        
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
                                    var fetchCallSlipsActivity = new Intent(this, typeof (CallSlipFetchingRequest));
                                    StartActivity(fetchCallSlipsActivity);
                                    //StartActivityForResult(fetchCallSlipsActivity, 1);
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
