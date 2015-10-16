using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Util;
using Android.Widget;
using Environment = System.Environment;

namespace Viola
{
     [Activity(Label = "", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class LogIn : Activity
    {
         string _filepath;

         protected override void OnCreate(Bundle bundle)
         {            
             base.OnCreate(bundle);
             var prefs = Application.Context.GetSharedPreferences("Violadev", FileCreationMode.Private);
             SetContentView(Resource.Layout.login);
             string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
             _filepath = Path.Combine(path, "CallSlips.txt");

             Button buttonLogin = FindViewById<Button>(Resource.Id.buttonLogin);
             buttonLogin.Click += HandleButtonLoginClick;
             var storedUserName = prefs.GetString("UserName", "");
             var storedPassWord = prefs.GetString("PassWord", "");
             ((EditText)FindViewById<EditText>(Resource.Id.editTextUserId)).Text = storedUserName;
             ((EditText)FindViewById<EditText>(Resource.Id.editTextPassword)).Text = storedPassWord;
             
         }

         async void HandleButtonLoginClick(object sender, EventArgs e)
         {
             var prefs = Application.Context.GetSharedPreferences("Violadev", FileCreationMode.Private);
             var prefEditor = prefs.Edit();
             bool success = false;
             var progressDialog = ProgressDialog.Show(this, "V‰nta...", "H‰mtar plocklista", true);
             var phoneCommunicationUrl = new System.Uri(((EditText)FindViewById<EditText>(Resource.Id.editTextPhoneCommunicationUrl)).Text);

             try
             {
                 Toast.MakeText(this, Dns.GetHostAddresses("violadev.sub.su.se").First().ToString(), ToastLength.Long).Show();
                 if (!HasInternetConnection)
                     throw new Exception("Det saknas InternetÅÂtkomst.");

                 string userId = ((EditText)FindViewById<EditText>(Resource.Id.editTextUserId)).Text;
                 string password = ((EditText)FindViewById<EditText>(Resource.Id.editTextPassword)).Text;
                 ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback((s, ce, ch, ssl) => true);
                 using (var handler = new HttpClientHandler { Credentials = new NetworkCredential(userId, password), ClientCertificateOptions = ClientCertificateOption.Automatic })
                 using (var client = new HttpClient(handler))
                 {
                     var uri = new System.Uri(phoneCommunicationUrl, string.Format("/api/PhoneCommunication?checkloginstatus=true", userId));
                     
                     string result = client.GetStringAsync(uri).Result;
                     if (result.Length > 0)
                     {
                         //string s = result;
                         //File.WriteAllText(_filepath, s, Encoding.Default);
                         prefEditor.PutBoolean("Authorized", true);
                         prefEditor.PutString("UserName", userId);
                         prefEditor.PutString("PassWord", password);
                         prefEditor.PutString("PhoneCommunicationUrl", phoneCommunicationUrl.OriginalString);
                         prefEditor.Commit();
                         success = true;
                         bool b = prefs.Contains("Authorized");
                     }
                 }

                  //RunOnUiThread(() => Toast.MakeText(this, "Toast within progress dialog.", ToastLength.Long).Show());             
             }
             catch (Exception ee)
             {
                 Toast.MakeText(this, "ID eller lˆsenord ‰r fel", ToastLength.Long).Show(); //+ ee.Message + ee.StackTrace
             }
             finally
             {
                 progressDialog.Hide();
                 if (success)
                 {
                     var fetchinrequest = new Intent(this, typeof(CallSlipFetchingRequest));
                         fetchinrequest.AddFlags(ActivityFlags.ClearTop);
                         fetchinrequest.AddFlags(ActivityFlags.NewTask);
                         fetchinrequest.AddFlags(ActivityFlags.ClearTask);
                     StartActivity(fetchinrequest);
                 }
                 else
                 {
                     var loginActivity = new Intent(this, typeof(LogIn));
                         loginActivity.AddFlags(ActivityFlags.ClearTop);
                         loginActivity.AddFlags(ActivityFlags.NewTask);
                         loginActivity.AddFlags(ActivityFlags.ClearTask);
                    StartActivity(loginActivity);
                 }
             }
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