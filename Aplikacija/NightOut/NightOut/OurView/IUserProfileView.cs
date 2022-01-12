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
using NightOut.Presenter;

namespace NightOut.OurView
{
    interface IUserProfileView
    {

        void UpdateInterface();

        UserProfilePresenter UserProfilePresenter { get; }

        void CommitUserChanges();
    }
}