using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using System.IO;
using Android.Net;



namespace Viola
{
    [Activity(Label = "", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class Activity1 : Activity
	{
		private ListView _callSlipView;
		//ArrayAdapter _callSlipArrayAdapter;
		string _filepath;
		
        protected override void OnCreate(Bundle bundle)
		{
            base.OnCreate(bundle);
            Rewire();
           
        }

        private void Rewire()
        {
			SetTitle(Resource.String.app_name);
            SetContentView(Resource.Layout.Main);
            
			_callSlipView = FindViewById<ListView>(Resource.Id.myList);
			
		    var callSlipAdapter = new CallSlipAdapter(this);
		    _callSlipView.Adapter = callSlipAdapter;

            Button buttonFetchCallSlips = FindViewById<Button>(Resource.Id.fetchCallSlipsButton);
            buttonFetchCallSlips.Click += delegate(object sender, EventArgs e)
            {
                if (HasInternetConnection)
                {
				var loginActivity = new Intent(this, typeof(FetchCallSlips));
				StartActivity(loginActivity);
                }
                else
                {
                    Toast.MakeText(this, "Det saknas Internetåtkomst", ToastLength.Long).Show();
                }

			};

            Button reportFetchButton = FindViewById<Button>(Resource.Id.reportFetchButton);

            reportFetchButton.Click += delegate(object sender, EventArgs e)
            {
                if (HasInternetConnection)
                {
				var reportFetchActivity = new Intent(this, typeof(ReportFetch));
				StartActivity(reportFetchActivity);
                }
                else
                {
                    Toast.MakeText(this, "Det saknas Internetåtkomst", ToastLength.Long).Show();
                }
				
			};


            _callSlipView.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs e)
            {
                var callSlipDetails = new Intent(this, typeof(CallSlipDetailsActivity));
                callSlipDetails.PutExtra("position", e.Position);
				//callSlipDetails.PutExtra ("CallSlips", jsonstring);
                StartActivity(callSlipDetails);
			};
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            _filepath = Path.Combine(path, "CallSlips.txt");          
		}

        protected override void OnResume()
        {
            base.OnResume();
            Rewire();
        }


        public bool HasInternetConnection
        {
            get
            {
                var connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
                return connectivityManager.ActiveNetworkInfo != null && connectivityManager.ActiveNetworkInfo.IsConnectedOrConnecting;
            }
        }
	}
}
