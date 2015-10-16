using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Widget;
using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using Android.Views;
using Encoding = System.Text.Encoding;
using Environment = System.Environment;

namespace Viola
{
    [Activity(Label = "", ScreenOrientation = ScreenOrientation.Portrait)]
    public class FetchCallSlips : Activity
    {
        private ListView _callSlipView;
        string _filepath;
        private string _getDaysOnShelfFilePath;
        private bool _isLoggedIn;
        
        protected override void OnCreate(Bundle bundle)
        {
            SetTitle(Resource.String.fetchcallslips);
            SetContentView(Resource.Layout.Main);
           
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            _filepath = Path.Combine(path, "CallSlips.txt");
            _getDaysOnShelfFilePath = Path.Combine(path, "GetDaysOnShelf.txt");
           

            base.OnCreate(bundle);
            var prefs = Application.Context.GetSharedPreferences("Violadev", FileCreationMode.Private);
            _isLoggedIn = prefs.GetBoolean("Authorized", false);
            Button buttonFetchCallSlips = FindViewById<Button>(Resource.Id.fetchCallSlipsButton);
            

            if (_isLoggedIn)
            {
                if (System.IO.File.Exists(_filepath) && _filepath != null)
                {
                    SetContentView(Resource.Layout.Main);
                    _callSlipView = FindViewById<ListView>(Resource.Id.myList);
                    var callSlipAdapter = new CallSlipAdapter(this);
                    _callSlipView.Adapter = callSlipAdapter;
                }
                else
                {
                    GetList();
                }
            }
            else
            {
                var loginActivity = new Intent(this, typeof(LogIn));
                StartActivity(loginActivity);
            }
        }
        
        //H‰mtar ny list frÂn Viola
      async void GetList()    //HandleButtonLoginClick (object sender, EventArgs e)
        {
            var prefs = Application.Context.GetSharedPreferences("Violadev", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            bool success = false;
            var progressDialog = ProgressDialog.Show(this, "V‰nta...", "H‰mtar plocklista", true);
            var phoneCommunicationUrl = new System.Uri(prefs.GetString("PhoneCommunicationUrl", "")); // new System.Uri(((EditText)FindViewById<EditText>(Resource.Id.editTextPhoneCommunicationUrl)).Text);

            try
            {
               // Toast.MakeText(this, Dns.GetHostAddresses("violadev.sub.su.se").First().ToString(), ToastLength.Long).Show();
                if (!HasInternetConnection)
                    throw new Exception("Det saknas InternetÅÂtkomst.");

                string userId = prefs.GetString("UserName", ""); //((EditText) FindViewById<EditText>(Resource.Id.editTextUserId)).Text;
                string password = prefs.GetString("PassWord", ""); //((EditText) FindViewById<EditText>(Resource.Id.editTextPassword)).Text;
                ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback((s, ce, ch, ssl) => true);
                using (
                    var handler = new HttpClientHandler
                    {
                        Credentials = new NetworkCredential(userId, password, "INTRANET"), ClientCertificateOptions = ClientCertificateOption.Automatic
                    })
                using (var client = new HttpClient(handler, true))
                {
                    var uri = new System.Uri(phoneCommunicationUrl, $"/api/PhoneCommunication?userid={userId}");
                    var result =  client.GetStringAsync(uri).Result;
                    //string result = "[{\"$type\":\"Viola.CallSlipFetching.CallSLipWithLocation, Viola\",\"AreaName\":\"≈rssviten\",\"AreaId\":64,\"AreaDefaultSortOrder\":\"39\",\"CallNumberDefaultSortOrder\":\"0\",\"CallSlipId\":95136,\"SourceId\":\"DirectDelivery\",\"RequestNumber\":\"28730\",\"PatronName\":\"MEHRDAD KAVOUSI\",\"PatronEmail\":\"mehrdad.kavousi@sub.su.se\",\"PatronBarcode\":\"90002854076911\",\"ItemBarcode\":null,\"Location\":\"ARSSVIT\",\"CallNumber\":\"2015: 1.329\",\"Author\":\"Abrahamsson, Bengt, 1937-\",\"Title\":\"Avf‰llingen : Lawrence efter Arabien /\",\"ISBN\":\"9789187393242\",\"ItemEnum\":null,\"ItemChronology\":null,\"Pages\":null,\"ItemYear\":\"2015\",\"CSNote\":\"\",\"StatusDescription\":null,\"DateRequested\":null,\"MFHDNote\":null,\"SeriesStatement\":null,\"SequentialDesignation\":null,\"PublicationYear\":null,\"DeliveryType\":\"Direktleverans\",\"PickupLocation\":\"Huvudbiblioteket.\",\"ResearcherAddress\":\"IT-avdelningen (biblioteket); Huvudbiblioteket, Universitetsv‰gen 10 D\",\"CallNumberId\":21,\"BranchSigel\":\"H\",\"LibrisId\":0,\"Status\":\"I plocklista\",\"UserId\":\"mkavo\",\"Date\":\"2015-06-28T18:41:11.513Z\",\"PrintType\":null}]";

                        //client.GetStringAsync(uri).Result;
                        string res = result.ToString();
                    if (res.Length > 0)
                    {
                        //string s = result;
                        File.WriteAllText(_filepath, res, System.Text.Encoding.Default);
                        prefEditor.PutBoolean("Authorized", true);
                        //prefEditor.PutString("UserName", userId);
                        //prefEditor.PutString("PassWord", password);
                        //prefEditor.PutString("PhoneCommunicationUrl", phoneCommunicationUrl.OriginalString);
                        prefEditor.Commit();
                        success = true;
                        bool _isLoggedIn = prefs.Contains("Authorized");
                    }
                }

                   GetDaysOnShelf();

                // RunOnUiThread(() => Toast.MakeText(this, "Toast within progress dialog.", ToastLength.Long).Show());
                //HIDE PROGRESS DIALOG

            }
            catch (Exception ee)
            {
                Toast.MakeText(this, "Fel: " + ee.Message + ee.StackTrace, ToastLength.Long).Show();
            }
            finally
            {
                progressDialog.Hide();
                if (success)
                {
                    _callSlipView = FindViewById<ListView>(Resource.Id.myList); //myList => Main.axml
                    var callSlipAdapter = new CallSlipAdapter(this);
                    _callSlipView.Adapter = callSlipAdapter;
                }
        else
                {
                    var loginActivity = new Intent(this, typeof(FetchCallSlips));
                        loginActivity.AddFlags(ActivityFlags.ClearTop);
                        loginActivity.AddFlags(ActivityFlags.NewTask);
                        loginActivity.AddFlags(ActivityFlags.ClearTask);
                    StartActivity(loginActivity);
                }
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            _callSlipView.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs e)
            {
                var callSlipDetails = new Intent(this, typeof(CallSlipDetailsActivity));
                callSlipDetails.PutExtra("position", e.Position);
                //callSlipDetails.AddFlags(ActivityFlags.ClearTop);
                //callSlipDetails.AddFlags(ActivityFlags.NewTask);
                //callSlipDetails.AddFlags(ActivityFlags.ClearTask);
                StartActivity(callSlipDetails);
                
            };
        }

        private async void GetDaysOnShelf()
        {
            var prefs = Application.Context.GetSharedPreferences("Violadev", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            try
            {
                //TODO: h‰mta zebrastr‰ngen frÂn viola - inte string.xml
                var phoneCommunicationUrl = new System.Uri(prefs.GetString("PhoneCommunicationUrl", string.Empty));
               // var uri = new System.Uri(phoneCommunicationUrl, "/api/PhoneCommunication");

                string userId = prefs.GetString("UserName", "");  //((EditText)FindViewById<EditText>(Resource.Id.editTextUserId)).Text;
                string password = prefs.GetString("PassWord", ""); //((EditText)FindViewById<EditText>(Resource.Id.editTextPassword)).Text;
                var credentials = new NetworkCredential(userId, password, "INTRANET");
                var uri = new System.Uri(phoneCommunicationUrl, string.Format("/api/PhoneCommunication"));
                using (var handler = new HttpClientHandler
                {
                    Credentials = credentials,                                        
                    ClientCertificateOptions = ClientCertificateOption.Automatic
                })
                using (var client = new HttpClient(handler))
                {
                    var getTask =  client.GetStringAsync(uri);
                    string s = getTask.Result;
                    getTask.ConfigureAwait(false);
                    prefEditor.PutInt("DaysOnShelf", int.Parse(s));
                    prefEditor.Commit();
                }
            }
            catch (Exception e)
            {
                Toast.MakeText(this, "DaysOnShelf ‰r noll", ToastLength.Long).Show();  // + e.Message + e.StackTrace
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
                        //Toast.MakeText(this, "Det saknas InternetÂtkomst", ToastLength.Long).Show();
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
                            Toast.MakeText(this, "Det saknas InternetÂtkomst", ToastLength.Long).Show();
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
                            Toast.MakeText(this, "Det saknas InternetÂtkomst", ToastLength.Long).Show();
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


