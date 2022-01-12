using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Support.V7.App;
using Android.Widget;
using NightOut.OurView;

namespace NightOut.Activities
{
    [Activity(Label = "@string/app_name",Icon ="@drawable/Icon", Theme ="@style/SplashTheme.Splash", MainLauncher =true, NoHistory =true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(typeof(LoginActivity));
        }
    }
}