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

namespace NightOut.OurView
{
    public interface ILogInView
    {
        void OnTaskCompleteLoginFailure(object sender, EventArgs e);

        void OnTaskCompleteLoginSuccess(object sender, EventArgs e);
    }
}