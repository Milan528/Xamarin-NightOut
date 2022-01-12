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
using Firebase.Database;

namespace NightOut.Model.EventListeners
{
    public class LocalListener : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<LocalDataEventArgs> LocalRetrived;

        public class LocalDataEventArgs : EventArgs
        {
            public Local Local { get; set; }
        }

        public void OnCancelled(DatabaseError error)
        {}

        public void OnDataChange(DataSnapshot snapshot)
        {
            if(snapshot.Value != null)
            {
                Local ResaultLocal = new Local(snapshot.Key)
                {
                    Address = snapshot.Child(Application.Context.Resources.GetString(Resource.String.local_address_column)).Value.ToString(),
                    City = snapshot.Child(Application.Context.Resources.GetString(Resource.String.local_city_column)).Value.ToString()
                };
                DateTime Date = DateTime.ParseExact(snapshot.Child(Application.Context.Resources.GetString(Resource.String.local_foundation_date_column)).Value.ToString(),
                    Application.Context.Resources.GetString(Resource.String.date_format), null);
                if (Date == DateTime.MaxValue)
                    ResaultLocal.FoundationDate = DateTime.MaxValue;
                else
                    ResaultLocal.FoundationDate = Date;
                LocalRetrived.Invoke(this, new LocalDataEventArgs
                {
                    Local = ResaultLocal
                });
            }
        }
    }
}