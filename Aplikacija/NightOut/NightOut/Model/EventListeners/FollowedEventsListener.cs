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
    class FollowedEventsListener : Java.Lang.Object, IValueEventListener
    {
        private Dictionary<string, string> FollowedEvents = new Dictionary<string, string>();
        public event EventHandler<FollowedEventsDataArgs> FollowedEventsRetrived;

        public class FollowedEventsDataArgs : EventArgs
        {
            public Dictionary<string, string> FollowedEvents { get; set; }
        }
        public void OnCancelled(DatabaseError error)
        {}

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                var AllData = snapshot.Children.ToEnumerable<DataSnapshot>();
                foreach (DataSnapshot EventData in AllData.ToArray<DataSnapshot>())
                {
                    FollowedEvents.Add(EventData.Child(Application.Context.Resources.GetString(Resource.String.event_id_column)).Value.ToString(), EventData.Key);
                }
                FollowedEventsRetrived.Invoke(this, new FollowedEventsDataArgs { FollowedEvents = this.FollowedEvents });
            }
        }
    }
}