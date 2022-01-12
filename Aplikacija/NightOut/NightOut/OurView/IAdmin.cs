using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupportFragment = Android.Support.V4.App.Fragment;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using NightOut.Model;

namespace NightOut.OurView
{
   public interface IAdmin
    {
        void OpenFragment(SupportFragment ev);
    }
}