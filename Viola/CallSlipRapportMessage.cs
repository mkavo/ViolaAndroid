using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Web;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using System.Xml.Linq;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json;
using Android.Net;
using Android.Test.Mock;
using Android.Views;
using Java.Lang;
using Environment = System.Environment;
using Exception = System.Exception;
using String = System.String;

namespace Viola
{
    [Activity(Label = "", ScreenOrientation = ScreenOrientation.Portrait)]
    public class CallSlipRapportMessage : Activity
    {
        string _filepath;
        protected override void OnCreate(Bundle bundle)
        {
            SetTitle(Resource.String.RaportBack);
            SetContentView(Resource.Layout.Main);

            base.OnCreate(bundle);
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle(Resource.String.reportcollecting);
            builder.SetMessage("Återrpportering är klart, Ha en bra dag!");
            builder.SetPositiveButton("OK", OkClicked);
            builder.SetCancelable(false);
            builder.Create();
            builder.Show();
        }

        private void OkClicked(object sender, DialogClickEventArgs dialogClickEventArgs)
        {
            var dialog = sender as AlertDialog;

            if (dialog != null)
            {
                var connectionEdit = dialog.FindViewById(Resource.Id.textView10) as EditText;

            if (connectionEdit != null)
                Console.WriteLine("Connection String: {0}", connectionEdit.Text);
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
                            logoutActivity.AddFlags(ActivityFlags.ClearTop);
                            logoutActivity.AddFlags(ActivityFlags.NewTask);
                            logoutActivity.AddFlags(ActivityFlags.ClearTask);
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
                                fetchCallSlipsActivity.AddFlags(ActivityFlags.ClearTop);
                                fetchCallSlipsActivity.AddFlags(ActivityFlags.NewTask);
                                fetchCallSlipsActivity.AddFlags(ActivityFlags.ClearTask);
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
                                callSlipBeforeReportFetchActivity.AddFlags(ActivityFlags.ClearTop);
                                callSlipBeforeReportFetchActivity.AddFlags(ActivityFlags.NewTask);
                                callSlipBeforeReportFetchActivity.AddFlags(ActivityFlags.ClearTask);
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