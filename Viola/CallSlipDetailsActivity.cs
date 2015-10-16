using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Nfc;
using Android.Nfc.Tech;
using Android.OS;
using Android.Widget;
using Android.Views;
using Java.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Android.Net;
using Environment = System.Environment;

namespace Viola
{
    [Activity(Label = "", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class CallSlipDetailsActivity : Activity
    {
        int _position;
        int _numberOfCallSlips;
        IEnumerable<CallSLipWithLocation> _callSlipsToFetch;
        string _filepath;
        Button _previousButton;
        Button _nextButton;
        Button _needsLookupButton;
        Button _barcodeButton;
        private NfcAdapter _nfcAdapter;
        Button _buttonChip;

        protected override void OnCreate(Bundle bundle)
        {
            var path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            _filepath = Path.Combine(path, "CallSlips.txt");
            _position = Intent.GetIntExtra("position", 0);

            base.OnCreate(bundle);
            SetContentView(Resource.Layout.CallSlipDetails);

            _buttonChip = FindViewById<Button>(Resource.Id.chipButton);
            _buttonChip.Click += HandleButtonChipClick;

            var buttonPrint = FindViewById<Button>(Resource.Id.printButton);
            buttonPrint.Click += HandleButtonPrintClick;

            var buttonFound = FindViewById<Button>(Resource.Id.foundButton);
            buttonFound.Click += buttonFound_Click;

            _nfcAdapter = NfcAdapter.GetDefaultAdapter(this);

            _previousButton = FindViewById<Button>(Resource.Id.previousButton);
            _previousButton.Click += HandlePreviousButtonClick;

            _nextButton = FindViewById<Button>(Resource.Id.nextButton);
            _nextButton.Click += HandleNextButtonClick;

            _needsLookupButton = FindViewById<Button>(Resource.Id.needsLookup);
            _needsLookupButton.Click += HandleNeedsLookupButtonClick;

            _barcodeButton = FindViewById<Button>(Resource.Id.barcodeButton);
            _barcodeButton.Click += HandleBarcodeButtonClick;

            FillPage();
        }

        void buttonFound_Click(object sender, EventArgs e)
        {
            try
            {
                CurrentCallSlip.Status = "Finns";
                File.WriteAllText(_filepath, JsonConvert.SerializeObject(_callSlipsToFetch, new CallSlipDeliveryTypeConverter()));
                ((TextView)FindViewById(Resource.Id.twStatus)).Text = CurrentCallSlip.Status;
            }
            catch (Exception ee)
            {
                ee.Message.ToString();
            }
        }

        void HandleNeedsLookupButtonClick(object sender, EventArgs e)
        {
            try
            {
                CurrentCallSlip.Status = "Skall slås";
                File.WriteAllText(_filepath, JsonConvert.SerializeObject(_callSlipsToFetch, new CallSlipDeliveryTypeConverter()));
                ((TextView)FindViewById(Resource.Id.twStatus)).Text = CurrentCallSlip.Status;
            }
            catch (Exception ee)
            {
                ee.Message.ToString();
            }
        }

        async void HandleBarcodeButtonClick(object sender, EventArgs e)
        {
            //var options = new ZXing.Mobile.MobileBarcodeScanningOptions {AutoRotate = true};
            var scanner = new ZXing.Mobile.MobileBarcodeScanner(this);
            var barcode = string.Empty;
            await scanner.Scan().ContinueWith(t =>
            {
                if (t.Result != null)
                    barcode = t.Result.Text;
            });
            if (!string.IsNullOrWhiteSpace(barcode))
                SetBarcode(barcode);
        }


        int NumberOfDone
        {
            get
            {
                try
                {
                    if (File.Exists(_filepath))
                    {
                        var doneStatuses = new[] { "Skall slås", "Finns" };
                        _callSlipsToFetch = JsonConvert.DeserializeObject<IEnumerable<CallSLipWithLocation>>(File.ReadAllText(_filepath), new CallSlipDeliveryTypeConverter());
                        var callSLipWithLocations = _callSlipsToFetch as IList<CallSLipWithLocation> ?? _callSlipsToFetch.ToList();
                        return callSLipWithLocations.Count(a => doneStatuses.Contains(a.Status));
                    }
                    else return -1;
                }
                catch
                {
                    return -2;
                }
            }
        }

        CallSLipWithLocation CurrentCallSlip
        {
            get
            {
                if (File.Exists(_filepath))
                {
                    _callSlipsToFetch =
                        JsonConvert.DeserializeObject<IEnumerable<CallSLipWithLocation>>(File.ReadAllText(_filepath), new CallSlipDeliveryTypeConverter());
                    var callSLipWithLocations = _callSlipsToFetch as IList<CallSLipWithLocation> ?? _callSlipsToFetch.ToList();
                    _numberOfCallSlips = callSLipWithLocations.Count();
                    if (_position > -1 && _position < _numberOfCallSlips)
                    {
                        return callSLipWithLocations.ElementAt(_position);
                    }
                    return null;
                }
                return null;
            }
        }

        void FillPage()
        {
            var itemYear = string.Empty;
            if (!string.IsNullOrWhiteSpace(CurrentCallSlip.ItemYear))
                itemYear = CurrentCallSlip.ItemYear;
            else if (!string.IsNullOrWhiteSpace(CurrentCallSlip.PublicationYear))
            {
                itemYear = CurrentCallSlip.PublicationYear;
            }
            var itemCron = string.IsNullOrWhiteSpace(CurrentCallSlip.ItemChronology) ? string.Empty : CurrentCallSlip.ItemChronology;
            var itemEnum = string.IsNullOrWhiteSpace(CurrentCallSlip.ItemEnum) ? string.Empty : CurrentCallSlip.ItemEnum;
            var yearVolumeIssue = string.Format("{0} {1} {2}", itemYear, itemCron, itemEnum);
            if (CurrentCallSlip != null)
            {
                ((TextView)FindViewById(Resource.Id.twArea)).Text = CurrentCallSlip.AreaName;
                ((TextView)FindViewById(Resource.Id.twDeliveryType)).Text = CurrentCallSlip.DeliveryType.ToString();
                ((TextView)FindViewById(Resource.Id.twCSnote)).Text = CurrentCallSlip.CSNote;
                ((TextView)FindViewById(Resource.Id.twCallNumber)).Text = CurrentCallSlip.CallNumber;
                ((TextView)FindViewById(Resource.Id.twTitle)).Text = CurrentCallSlip.Title;
                ((TextView)FindViewById(Resource.Id.twSeriesStatement)).Text = CurrentCallSlip.SeriesStatement;
                ((TextView)FindViewById(Resource.Id.twBarCode)).Text = CurrentCallSlip.ItemBarcode;
                ((TextView)FindViewById(Resource.Id.twYearVolumeIssue)).Text = yearVolumeIssue;
                ((TextView)FindViewById(Resource.Id.twStatus)).Text = CurrentCallSlip.Status;
                ((TextView)FindViewById(Resource.Id.twNumberOfDone)).Text = NumberOfDone + " klara";
                FindViewById(Resource.Id.twYearVolumeIssue).Visibility = string.IsNullOrWhiteSpace(yearVolumeIssue) ? ViewStates.Gone : ViewStates.Visible;
                FindViewById(Resource.Id.textViewYvi).Visibility = string.IsNullOrWhiteSpace(yearVolumeIssue) ? ViewStates.Gone : ViewStates.Visible;
                _nextButton.Enabled = _position < _numberOfCallSlips - 1;
                _previousButton.Enabled = _position > 0;
                Title = Resources.GetString(Resource.String.callSlipDetails) + " - " + (_position + 1) + " av " + _callSlipsToFetch.Count();
            }
        }

        void HandleNextButtonClick(object sender, EventArgs e)
        {
            _position++;
            FillPage();
        }

        void HandlePreviousButtonClick(object sender, EventArgs e)
        {
            _position--;
            FillPage();
        }

        private string ZPLTransform(string valueToTransform)
        {
            var returnString = valueToTransform;
            var transformations = new Dictionary<string, string>
            {
                {"å", "_86"},
                {"ä", "_84"},
                {"ö", "_94"},
                {"Å", "_8F"},
                {"Ä", "_8E"},
                {"Ö", "_99"}
            };
            return transformations.Aggregate(returnString, (current, t) => current.Replace(t.Key, t.Value));
        }

        async void HandleButtonPrintClick(object sender, EventArgs e)
        {
            try
            {
                var deliveryType = (CallSlipDeliveryTypes)CurrentCallSlip.DeliveryType;
                object[] args =
                {
                    ZPLTransform(deliveryType ==  CallSlipDeliveryTypes.ILLOUT ? "Fjärrut":"Framtag"),
                    ZPLTransform(CurrentCallSlip.CallNumber?? string.Empty),
                    ZPLTransform(CurrentCallSlip.Title?? string.Empty),
                    ZPLTransform(CurrentCallSlip.SeriesStatement ?? string.Empty),
                    ZPLTransform(CurrentCallSlip.Author?? string.Empty),
                    ZPLTransform(CurrentCallSlip.ItemYear?? CurrentCallSlip.PublicationYear ?? string.Empty) +
                    ZPLTransform(CurrentCallSlip.ItemEnum?? string.Empty),
                    ZPLTransform(CurrentCallSlip.CSNote + CurrentCallSlip),
                    ZPLTransform(CurrentCallSlip.CallSlipId.ToString(CultureInfo.InvariantCulture)),
                    ZPLTransform(CurrentCallSlip.CallSlipId.ToString(CultureInfo.InvariantCulture)),
                    ZPLTransform(CurrentCallSlip.PatronName?? string.Empty),
                    ZPLTransform(GenerateFooter(CurrentCallSlip))
                };
                //TODO: ersätt med sträng från viola
                var zplCommand = string.Format(Resources.GetString(Resource.String.printslipmodel), args);
                // Create a buffer with the command
                byte[] buffer = StringToAscii(zplCommand);

                var adapter = BluetoothAdapter.DefaultAdapter;
                if (adapter == null)
                {
                    Toast.MakeText(this, "Bluetooth is not available", ToastLength.Long).Show();
                    Finish();
                    return;
                }
                var device = adapter.BondedDevices.FirstOrDefault(p => p.Name.StartsWith("5"));
                if (device != null)
                {
                    var uid = UUID.FromString(device.GetUuids()[0].ToString());
                    BluetoothSocket socket = device.CreateRfcommSocketToServiceRecord(uid);
                    socket.Connect();
                    if (socket.IsConnected)
                    {
                        Toast.MakeText(this, socket.RemoteDevice.BondState.ToString(), ToastLength.Long).Show();
                        await socket.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                        socket.OutputStream.Close();
                        socket.Close();
                    }
                    else
                    {
                        Toast.MakeText(this, socket.RemoteDevice.BondState.ToString(), ToastLength.Long).Show();
                    }
                    socket.Dispose();
                }
            }
            catch (Exception ee)
            {
                Toast.MakeText(this, "Det gick inte att skriva ut" + ee.Message + ee.StackTrace, ToastLength.Long).Show();
            }
        }

        private static string GenerateFooter(CallSLipWithLocation currentCallSlip)
        {
            var deliveryType = (CallSlipDeliveryTypes)currentCallSlip.DeliveryType;
            if (deliveryType == CallSlipDeliveryTypes.STACKCALL)
            {
                return "På köhylla " + DateTime.Now.AddDays(4).ToShortDateString();
            }
            if (deliveryType == CallSlipDeliveryTypes.DIRECTDELIVERY)
            {
                return "Direktleverans";
            }
            if (deliveryType == CallSlipDeliveryTypes.ILLOUT)
            {
                return currentCallSlip.RequestNumber;
            }
            return "Felaktig DeliveryType " + deliveryType;
        }

        void HandleButtonChipClick(object sender, EventArgs e)
        {
            StartTagListening();
        }

        public static byte[] StringToAscii(string s)
        {
            var retval = new byte[s.Length];
            for (var ix = 0; ix < s.Length; ++ix)
            {
                var ch = s[ix];
                if (ch <= 0x7f)
                    retval[ix] = (byte)ch;
                else
                    retval[ix] = (byte)'?';
            }
            return retval;
        }

        private void StartTagListening()
        {
            // Create an intent filter for when an NFC tag is discovered.  When
            // the NFC tag is discovered, Android will u
            var tagDetected = new IntentFilter(NfcAdapter.ActionTagDiscovered);
            var filters = new[] { tagDetected };

            // When an NFC tag is detected, Android will use the PendingIntent to come back to this activity.
            // The OnNewIntent method will invoked by Android.
            var intent = new Intent(this, GetType()).AddFlags(ActivityFlags.SingleTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, 0);
            _nfcAdapter.EnableForegroundDispatch(this, pendingIntent, filters, null);
        }

        /// <summary>
        /// This method is called when an NFC tag is discovered by the application.
        /// </summary>
        /// <param name="intent"></param>
        protected override void OnNewIntent(Intent intent)
        {
            var cmd = new[] { (byte)0x00, (byte)0x23, (byte)0x00, (byte)0x28 };
            try
            {
                var tag = intent.GetParcelableExtra(NfcAdapter.ExtraTag) as Tag;

                if (tag == null)
                    return;
                var nfcVTag = NfcV.Get(tag);
                nfcVTag.Connect();
                var rawResponse = nfcVTag.Transceive(cmd);
                var response = Encoding.ASCII.GetString(rawResponse);
                var barcode = response.Substring(4, 14);
                SetBarcode(barcode);
            }
            catch (Exception ee)
            {
                Toast.MakeText(this, ee.Message, ToastLength.Long).Show();
            }
        }

        private void SetBarcode(string barcode)
        {
            double barcodeInt;
            CurrentCallSlip.ItemBarcode = !double.TryParse(barcode, out barcodeInt) ? string.Empty : barcodeInt.ToString(CultureInfo.InvariantCulture);
            File.WriteAllText(_filepath, JsonConvert.SerializeObject(_callSlipsToFetch, new CallSlipDeliveryTypeConverter()));
            CurrentCallSlip.Status = "Finns";
            File.WriteAllText(_filepath, JsonConvert.SerializeObject(_callSlipsToFetch, new CallSlipDeliveryTypeConverter()));
            FillPage();
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

