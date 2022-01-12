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

namespace NightOut.Model.EventListeners
{
    class UpdateListener : Java.Lang.Object, Android.Gms.Tasks.IOnCompleteListener
    {
        public event EventHandler<UpdateData> Updated;
        private Person _updatingPerson;

        public class UpdateData : EventArgs
        {
            public Person UpdatedPerson;
        }

        public void SetAttributes(Person Person)
        {
            this._updatingPerson = Person;
        }

        public void OnComplete(Task task)
        {
            Updated.Invoke(this,new UpdateData { UpdatedPerson = _updatingPerson});
        }
    }
}