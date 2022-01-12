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
    public class RemoveFollowedEventsListener : Java.Lang.Object, Firebase.Database.IValueEventListener
    {
        public Action Action { get; set; }
        public void OnCancelled(DatabaseError error)
        {
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value == null)
                return;
            DataSnapshot[] AllData = snapshot.Children.ToEnumerable<DataSnapshot>().ToArray<DataSnapshot>();
            foreach (DataSnapshot FollowedEventData in AllData)
            {
                Model.FirebaseCommunicator.GetDatabase().Reference
                    .Child(Application.Context.Resources.GetString(Resource.String.user_followed_events_table_name))
                    .Child(FollowedEventData.Key)
                    .RemoveValue();

            }
            if (Action != null)
                Action.Invoke();
        }
    }
}