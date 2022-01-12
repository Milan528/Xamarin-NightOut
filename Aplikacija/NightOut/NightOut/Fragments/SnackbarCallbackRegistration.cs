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
using Android.Support.Design.Widget;
using NightOut.OurView;

namespace NightOut.Fragments
{
    public class SnackbarCallbackRegistration : Snackbar.Callback
    {
        private IRegistrationView _parentActivity;

        public SnackbarCallbackRegistration(IRegistrationView ParentActivity)
        {
            _parentActivity = ParentActivity;
        }
        
        public override void OnDismissed(Snackbar transientBottomBar, int @event)
        {
            base.OnDismissed(transientBottomBar, @event);
            if (@event == DismissEventTimeout)
                _parentActivity.GoToLoginPage();
        }
    }
}