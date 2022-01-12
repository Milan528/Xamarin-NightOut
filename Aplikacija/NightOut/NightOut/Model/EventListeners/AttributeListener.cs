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
    public class AttributeListener : Java.Lang.Object, IValueEventListener
    {

        public event EventHandler<AttributeDataEventArgs> AttributesRetrived;
        public event EventHandler<AttributeDataEventArgs> AttributesNotRetrived;
        public class AttributeDataEventArgs : EventArgs
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public void OnCancelled(DatabaseError error)
        {}

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                AttributesRetrived.Invoke(this, new AttributeDataEventArgs
                {
                    Name = snapshot.Key,
                    Value = snapshot.Value.ToString()
                });
            }
            else
            {
                AttributesNotRetrived.Invoke(this, new AttributeDataEventArgs
                {
                    Name = null,
                    Value = null
                });
            }
        }
    }
}