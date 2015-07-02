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
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Android.Net;
using Environment = System.Environment;


namespace Viola
{
    [Activity(Label = "", ScreenOrientation = ScreenOrientation.Portrait/*Resources.GetString(Resource.String.app_name) + " - " +Resources.GetString(Resource.String.reportcollecting)*/)]			
	public class ReportFetch : Activity
	{
		//XElement callSlipsX;
		string _filepath;
		XNamespace xmlns = "http://schemas.datacontract.org/2004/07/Viola.CallSlipFetching";

		protected override void OnCreate (Bundle bundle)
		{
			SetTitle(Resource.String.reportcollecting);
			var path = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			_filepath = Path.Combine (path, "CallSlips.txt");
			base.OnCreate (bundle);
			//ShowAlert ();
			SetContentView (Resource.Layout.login);

			var prefs = Application.Context.GetSharedPreferences ("Viola", FileCreationMode.Private);  
			var storedUserName = prefs.GetString ("UserName", "");
			((EditText)FindViewById<EditText> (Resource.Id.editTextUserId)).Text = storedUserName;
            var storedPassWord = prefs.GetString("PassWord", "");
            ((EditText)FindViewById<EditText>(Resource.Id.editTextPassword)).Text = storedPassWord;
            var storedPhoneCommunicationUrl = prefs.GetString("PhoneCommunicationUrl", "");
            ((EditText)FindViewById<EditText>(Resource.Id.editTextPhoneCommunicationUrl)).Text = storedPhoneCommunicationUrl;
			var buttonLogin = FindViewById<Button> (Resource.Id.buttonLogin);
			buttonLogin.Click += HandleButtonLoginClick;

			// Create your application here
		}

        async void HandleButtonLoginClick(object sender, EventArgs e)
        {
            var progressDialog = ProgressDialog.Show(this, "VÅ‰nta...", "Å≈terrapporterar plocklista", true);
            var mainActivity = new Intent(this, typeof(Activity1));
            try
            {
                if (!HasInternetConnection)
                    throw new Exception("Det saknas InternetÅÂtkomst.");

                var report = GetFetchReport();
                var stringtoPost = HttpUtility.UrlEncode(report.ToString());
                var userId = ((EditText)FindViewById<EditText>(Resource.Id.editTextUserId)).Text;
                var password = ((EditText)FindViewById<EditText>(Resource.Id.editTextPassword)).Text;
                var phoneCommunicationUrl = new System.Uri(((EditText) FindViewById<EditText>(Resource.Id.editTextPhoneCommunicationUrl)).Text);
                var uri = new System.Uri(phoneCommunicationUrl, "/api/fetchreporting");
                ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback((s, ce, ch, ssl) => true);
                using (var handler = new HttpClientHandler { Credentials = new NetworkCredential(userId, password, "INTRANET"), ClientCertificateOptions = ClientCertificateOption.Automatic })
                using (var client = new HttpClient(handler))
                {
                    var result = await client.PostAsync(uri, new StringContent(stringtoPost, Encoding.Default, "application/x-www-form-urlencoded"));
                    if (result.StatusCode == HttpStatusCode.NoContent)
                    {
                        var prefs = Application.Context.GetSharedPreferences("Viola", FileCreationMode.Private);
                        var prefEditor = prefs.Edit();
                        prefEditor.PutString("UserName", userId);
                        prefEditor.PutString("PhoneCommunicationUrl", phoneCommunicationUrl.OriginalString);
                        prefEditor.Commit();
                        File.Delete(_filepath);
                        Toast.MakeText(this, "Hurra!", ToastLength.Long).Show();
                    }
                    if (result.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        Toast.MakeText(this, "Å≈tkomst saknas!", ToastLength.Long).Show();
                    }
                    else { Toast.MakeText(this, result.StatusCode.ToString(), ToastLength.Long).Show(); }
                }
            }
            catch (Exception ee)
            {
                Toast.MakeText(this, "Fel: " + ee.Message, ToastLength.Long).Show();
            }
            finally
            {
                progressDialog.Hide();
                StartActivity(mainActivity);
            }
        }


		XDocument GetFetchReport ()
		{
            var s = File.ReadAllText(_filepath);
            var callSlips = JsonConvert.DeserializeObject<IEnumerable<CallSLipWithLocation>>(s, new CallSlipDeliveryTypeConverter());
            
			var retrievalReport = new XDocument ();
			var rootelem = new XElement ("retrievalreport");
			var fReport = callSlips.Select(x => new XElement ("callslip", 
					                           new XElement ("barcode", x.ItemBarcode ?? string.Empty),
					                           new XElement ("status", x.Status ?? string.Empty),
					                           new XElement ("id", x.CallSlipId)));               
			
			foreach (var el in fReport) {
				rootelem.Add (el);
			}
            retrievalReport.Add (rootelem);
			return retrievalReport;
			
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

