using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using System.IO;
using Newtonsoft.Json;

namespace Viola
{
    class CallSlipAdapter : BaseAdapter
    {
        private List<CallSlipDisplay> _callSlipList;
        private Activity _activity;

        public CallSlipAdapter(Activity avtivity)
        {
            _activity = avtivity;
            FillCallSlips();
        }

        public void FillCallSlips()
        {
            _callSlipList = new List<CallSlipDisplay>();
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filepath = Path.Combine(path, "CallSlips.txt");

            if (File.Exists(filepath))
            {
                try
                {
                    var callSlips = JsonConvert.DeserializeObject<IEnumerable<CallSLipWithLocation>>(File.ReadAllText(filepath), new CallSlipDeliveryTypeConverter()).ToList();

                foreach (CallSLipWithLocation a in callSlips)
                {
                        _callSlipList.Add(new CallSlipDisplay {DisplayName = a.Title, DisplayStatus = a.Status, PhotoId = "2", Id = a.CallSlipId});
                    }
                }
                catch (Exception ee)
                {
                    ee.Message.ToString();
                }
            }
        }


        public override int Count
        {
            get { return _callSlipList.Count(); }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return _callSlipList[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? _activity.LayoutInflater.Inflate(
            Resource.Layout.CallSlipShort, parent, false);
            var callSlipImage = view.FindViewById<ImageView>(Resource.Id.CallSlipImage);
            var callSlipTitle = view.FindViewById<TextView>(Resource.Id.CallSlipTitle);
            var callSlipStatus = view.FindViewById<TextView>(Resource.Id.CallSlipStatus);

            callSlipTitle.Text = _callSlipList[position].DisplayName;
            callSlipStatus.Text = _callSlipList[position].DisplayStatus;

            if (callSlipStatus.Text == "Skall slÅÂss")
            {
                callSlipImage.SetImageResource(Resource.Drawable.ListLookup);
            }
            else if (callSlipStatus.Text == "Finns")
            {
                callSlipImage.SetImageResource(Resource.Drawable.ListFound);
            }
            else
            {
                callSlipImage.SetImageResource(Resource.Drawable.ListUndone);
            }
            return view;
        }
    }

    class CallSlipDisplay
    {
        public long Id { get; set; }
        public string DisplayName { get; set; }
        public string DisplayStatus { get; set; }
        public string PhotoId { get; set; }
    }
}