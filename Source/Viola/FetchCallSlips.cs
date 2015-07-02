
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Widget;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using Environment = System.Environment;

namespace Viola
{
    [Activity(Label = "", ScreenOrientation = ScreenOrientation.Portrait)]
    public class FetchCallSlips : Activity
    {
        string _filepath;
        private string _getDaysOnShelfFilePath;

        protected override void OnCreate(Bundle bundle)
        {
            SetTitle(Resource.String.fetchcallslips);
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            _filepath = Path.Combine(path, "CallSlips.txt");
            _getDaysOnShelfFilePath = Path.Combine(path, "GetDaysOnShelf.txt");

            base.OnCreate(bundle);
            SetContentView(Resource.Layout.login);
            Button buttonLogin = FindViewById<Button>(Resource.Id.buttonLogin);
            buttonLogin.Click += HandleButtonLoginClick;
            var prefs = Application.Context.GetSharedPreferences("Viola", FileCreationMode.Private);
            var storedUserName = prefs.GetString("UserName", "");
            ((EditText)FindViewById<EditText>(Resource.Id.editTextUserId)).Text = storedUserName;
            ((EditText)FindViewById<EditText>(Resource.Id.editTextPassword)).Text = prefs.GetString("PassWord", ""); ;
        }

        async void HandleButtonLoginClick(object sender, EventArgs e)
        {
            var prefs = Application.Context.GetSharedPreferences("Viola", FileCreationMode.Private);
            
            var prefEditor = prefs.Edit();
            var mainActivity = new Intent(this, typeof(Activity1));
            bool success = false;

            var progressDialog = ProgressDialog.Show(this, "V‰nta...", "H‰mtar plocklista", true);
            var phoneCommunicationUrl = new System.Uri(((EditText)FindViewById<EditText>(Resource.Id.editTextPhoneCommunicationUrl)).Text);
            
            
            //string url = "http://localhost:1539/api/PhoneCommunication?userid={0}";
           
            try
            {
                Toast.MakeText(this, Dns.GetHostAddresses("violadev.sub.su.se").First().ToString(), ToastLength.Long).Show();
                if (!HasInternetConnection)
                    throw new Exception("Det saknas InternetÅÂtkomst.");

                string userId = ((EditText)FindViewById<EditText>(Resource.Id.editTextUserId)).Text;
                string password = ((EditText)FindViewById<EditText>(Resource.Id.editTextPassword)).Text;
                ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback((s, ce, ch, ssl) => true);
                using (var handler = new HttpClientHandler { Credentials = new NetworkCredential(userId, password, "INTRANET"), ClientCertificateOptions = ClientCertificateOption.Automatic })
                using (var client = new HttpClient(handler))
                {
                    var uri = new System.Uri(phoneCommunicationUrl, string.Format("/api/PhoneCommunication?userid={0}", userId));
                    var result = await client.GetStringAsync(uri);
                    //Toast.MakeText(this, result + result.Length, ToastLength.Long).Show();
                    if (result.Length > 0)
                    {
                        string s = result;
                        File.WriteAllText(_filepath, s, Encoding.Default);
                        prefEditor.PutString("UserName", userId);
                        prefEditor.PutString("PassWord", password);
                        prefEditor.PutString("PhoneCommunicationUrl", phoneCommunicationUrl.OriginalString);
                        prefEditor.Commit();
                        success = true;
                    }
                }

                GetDaysOnShelf();

                // RunOnUiThread(() => Toast.MakeText(this, "Toast within progress dialog.", ToastLength.Long).Show());
                //HIDE PROGRESS DIALOG

            }
            catch (Exception ee)
            {
                Toast.MakeText(this, "Fel: " + ee.Message + ee.StackTrace , ToastLength.Long).Show();
            }
            finally
            {
                progressDialog.Hide();
                if (success)
                    StartActivity(mainActivity);
                else
                {
                    var loginActivity = new Intent(this, typeof(FetchCallSlips));
                    StartActivity(loginActivity);
                }

            }



        }

        private async void GetDaysOnShelf()
        {
            var prefs = Application.Context.GetSharedPreferences("Viola", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            try
            {
                //TODO: h‰mta zebrastr‰ngen frÂn viola - inte string.xml
                var phoneCommunicationUrl = new System.Uri(prefs.GetString("PhoneCommunicationUrl", string.Empty));
                var uri = new System.Uri(phoneCommunicationUrl, "/api/PhoneCommunication");

                var credentials = new NetworkCredential(Resources.GetString(Resource.String.username),
                    Resources.GetString(Resource.String.password), "INTRANET");

                using (var handler = new HttpClientHandler
                {
                    Credentials = credentials,                                        
                    ClientCertificateOptions = ClientCertificateOption.Automatic
                })
                using (var client = new HttpClient(handler))
                {
                    
                    var getTask = client.GetStringAsync(uri);
                    getTask.ConfigureAwait(false);
                    prefEditor.PutInt("DaysOnShelf", int.Parse(getTask.Result));
                    prefEditor.Commit();
                }
            }
            catch{}
            
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


