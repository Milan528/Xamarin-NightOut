using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace NightOut.Model
{
    public class TaskCompleteListener : Java.Lang.Object, IOnSuccessListener,IOnFailureListener
    {
        #region Attributes
        public event EventHandler Success;
        public event EventHandler Failure;
        #endregion

        #region Methods

        public void OnFailure(Java.Lang.Exception e)
        {
            Failure.Invoke(this, new EventArgs());
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            Success.Invoke(this, new EventArgs());
        }
        #endregion
    }
}