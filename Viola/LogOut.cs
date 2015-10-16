using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Util;
using Android.Widget;
using Java.Lang;
using Java.Util;
using Environment = Android.Provider.Settings.System;




namespace Viola
{
    [Activity(Label = "", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class LogOut : Activity
    {
        private bool _isLoggedIn = false;

        protected override void OnCreate(Bundle bundle)
        {
            Finish();
            bool _is = base.IsFinishing;
            ClearApplicationData(this);
            clearAndExit(this);
            OnDestroy();
            //base.OnResume();
            var prefs = Application.Context.GetSharedPreferences("Violadev", FileCreationMode.Private);         
            var prefEditor = prefs.Edit();
            prefEditor.PutBoolean("Authorized", false);
            prefEditor.PutString("UserName", "");
            prefEditor.PutString("PassWord", "");
            prefEditor.Commit();
            var loginActivity = new Intent(this, typeof (LogIn));
            StartActivity(loginActivity);
        }

        public static void clearAndExit(Context ctx)
        {
            if (ctx != new MainActivity())
            {
                Intent intent = new Intent(ctx, typeof(MainActivity));
                intent.SetFlags(ActivityFlags.ClearTop);
                Bundle bundle = new Bundle();
                bundle.PutBoolean("exit", true);
                intent.PutExtras(bundle);
                ctx.StartActivity(intent);
            }
            else
            {
                ((Activity)ctx).Finish();
            }
        }

        public void ClearApplicationData(Context context)
        {
            // TODO Auto-generated method stub
            
            try
            {
                Java.IO.File Cache = context.CacheDir;
                Java.IO.File appDir = context.FilesDir;
                
                if (Cache.Exists() && Cache.IsDirectory)
                {
                            deleteDir(Cache);
                }
            }
            catch (System.Exception e)
            {
                // TODO Auto-generated catch block
                Toast.MakeText(this, "Fel: " + e.Message + e.StackTrace, ToastLength.Long).Show();
            }
        }

        public static bool deleteDir(Java.IO.File dir)
        {
            
                if (dir != null && dir.IsDirectory)
                {
                    string[] children = dir.List();
                    for (int i = 0; i < children.Length; i++)
                    {
                        bool success = deleteDir(new Java.IO.File(dir, children[i]));
                        if (!success)
                        {
                            return false;
                        }
                    }
                }
           

            return dir.Delete();
        }

        //public  void trimCache(Context contex)
        //{
        //    try
        //    {
        //       Java.IO.File f = base.CacheDir.AbsoluteFile;
        //        string path = contex.CacheDir.AbsolutePath;
        //        if (path != null)
        //        {
        //            deleteDir(path);
        //        }
        //    }
        //    catch (System.Exception e)
        //    {
        //        Toast.MakeText(this, "Fel: " + e.Message + e.StackTrace, ToastLength.Long).Show();
        //    }
        //}
    }
}