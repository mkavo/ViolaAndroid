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
using Java.Lang;
using Environment = System.Environment;
using Exception = System.Exception;
using String = System.String;


namespace Viola
{
    [Activity(Label = "", ScreenOrientation = ScreenOrientation.Portrait/*Resources.GetString(Resource.String.app_name) + " - " +Resources.GetString(Resource.String.reportcollecting)*/)]			
	public class ReportFetched : Activity
	{
        //XElement callSlipsX;
        private bool mResult;
        string _filepath;
        private bool _isLoggedIn;
		XNamespace xmlns = "http://schemas.datacontract.org/2004/07/Viola.CallSlipFetching";

		protected override void OnCreate (Bundle bundle)
		{
			SetTitle(Resource.String.reportcollecting);
			var path = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			_filepath = Path.Combine (path, "CallSlips.txt");
			base.OnCreate (bundle);
			//ShowAlert ();

            var prefs = Application.Context.GetSharedPreferences("Violadev", FileCreationMode.Private);
            _isLoggedIn = prefs.GetBoolean("Authorized", false);


            if (_isLoggedIn)
            {
                PostReport();
            }
            else
            {
                var loginActivity = new Intent(this, typeof(LogIn));
                    loginActivity.AddFlags(ActivityFlags.ClearTop);
                    loginActivity.AddFlags(ActivityFlags.NewTask);
                    loginActivity.AddFlags(ActivityFlags.ClearTask);
                StartActivity(loginActivity);
            }
			

			// Create your application here
		}

        async void  PostReport()                //HandleButtonLoginClick(object sender, EventArgs e)
        {
            var prefs = Application.Context.GetSharedPreferences("Violadev", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            var progressDialog = ProgressDialog.Show(this, "VÅ‰nta...", "Å≈terrapporterar plocklista", true);
            SetContentView(Resource.Layout.Main);

            try
            {
                if (!HasInternetConnection)
                    throw new Exception("Det saknas InternetÅÂtkomst.");

                var report = GetFetchReport();
                var stringtoPost = HttpUtility.UrlEncode(report.ToString());
                var userId = prefs.GetString("UserName", "");   //((EditText)FindViewById<EditText>(Resource.Id.editTextUserId)).Text;
                var password = prefs.GetString("PassWord", ""); // ((EditText)FindViewById<EditText>(Resource.Id.editTextPassword)).Text;
                var phoneCommunicationUrl = new System.Uri(prefs.GetString("PhoneCommunicationUrl", ""));  //new System.Uri(((EditText) FindViewById<EditText>(Resource.Id.editTextPhoneCommunicationUrl)).Text);
                ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback((s, ce, ch, ssl) => true);
                using (var handler = new HttpClientHandler { Credentials = new NetworkCredential(userId, password, "INTRANET"), ClientCertificateOptions = ClientCertificateOption.Automatic })
                using (var client = new HttpClient(handler))
                {
                    var result = await client.GetAsync(phoneCommunicationUrl + "/api/fetchreporting?report=" + stringtoPost);
                    if (result.StatusCode == HttpStatusCode.NoContent)
                    {
                        prefEditor.PutString("UserName", userId);
                        prefEditor.PutString("PhoneCommunicationUrl", phoneCommunicationUrl.OriginalString);
                        prefEditor.Commit();
                        File.Delete(_filepath);
                        //Toast.MakeText(this, "Hurra!", ToastLength.Long).Show();
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
                var repportMessage = new Intent(this, typeof(CallSlipRapportMessage));
                    repportMessage.AddFlags(ActivityFlags.ClearTop);
                    repportMessage.AddFlags(ActivityFlags.NewTask);
                    repportMessage.AddFlags(ActivityFlags.ClearTask);
                StartActivity(repportMessage);
            }
        }

        //Get reported info frÂn CallSlips.txt
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

