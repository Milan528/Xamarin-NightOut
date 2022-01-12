using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace NightOut.Activities
{
    [Activity(Label = "TestActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class TestActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.user_main_page);
            // ACTIVITY ZA PROVERU FUNKCIONALNOSTI LOGIN-A
        }
    }
}