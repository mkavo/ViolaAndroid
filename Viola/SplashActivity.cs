namespace Viola
{
    using System.Threading;

    using Android.App;
    using Android.OS;

    [Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Thread.Sleep(1000); // Simulate a long loading process on app startup. original value 10000
            StartActivity(typeof(MainActivity));
        }
    }
}