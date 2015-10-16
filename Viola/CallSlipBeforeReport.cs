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
using Java.Security;
using Newtonsoft.Json;
using Environment = System.Environment;

namespace Viola
{
    [Activity(Label = "", ScreenOrientation = ScreenOrientation.Portrait)]
    public class CallSlipBeforeReport : Activity
    {
        private string _filepath;

        protected override void OnCreate(Bundle bundle)
        {
            SetTitle(Resource.String.RaportBack);
            SetContentView(Resource.Layout.CalSlipRapportera);
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            _filepath = Path.Combine(path, "CallSlips.txt");
            base.OnCreate(bundle);

            TextView treatedPosterAllView = FindViewById<TextView>(Resource.Id.poster_alla);
            TextView treatedPosterNotAllView = FindViewById<TextView>(Resource.Id.poster_ej_alla);


            if (ReportedPoster() == false)
            {
                treatedPosterAllView.Visibility = ViewStates.Invisible;
                treatedPosterNotAllView.Visibility = ViewStates.Visible;                
            }
            else
            {
                treatedPosterAllView.Visibility = ViewStates.Visible;
                treatedPosterNotAllView.Visibility = ViewStates.Invisible;
            }

            //Återrapportera CallSlips till Viola
            Button rapporteraCallSlips = FindViewById<Button>(Resource.Id.RaportBackBtn);
            rapporteraCallSlips.Click += HandleButtonRapporteraCallSlips;
        }

        public void HandleButtonRapporteraCallSlips(object sender, EventArgs e)
        {
            var reportFetchActivity = new Intent(this, typeof(ReportFetched));
            StartActivity(reportFetchActivity);
        }

        public bool ReportedPoster()
        {

            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filepath = Path.Combine(path, "CallSlips.txt");
            bool finns = File.Exists(filepath);

            if (finns)
            {
                try
                {
                    string readAllText = File.ReadAllText(filepath);
                    var callSlips = JsonConvert.DeserializeObject<IEnumerable<CallSLipWithLocation>>(readAllText, new CallSlipDeliveryTypeConverter()).ToList();

                    foreach (CallSLipWithLocation a in callSlips)
                    {
                        if (a.Status != "Skall slås" && a.Status != "Finns")
                        {
                            return false;  //Minst en post inte är klar för repportering
                        }    
                    }
                }
                catch (Exception ee)
                {
                    ee.Message.ToString();
                }
            }
            return true;
        }

        //Menu
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