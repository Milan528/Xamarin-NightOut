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
    class FollowedOrganiserListener : Java.Lang.Object, Firebase.Database.IValueEventListener
    {
        private Dictionary<string, string> FollowedOrganisers = new Dictionary<string, string>();
        public event EventHandler<FollowedOrganisersDataArgs> FollowedOrganisersRetrived;

        public class FollowedOrganisersDataArgs : EventArgs
        {
            public Dictionary<string, string> FollowedOrganisers { get; set; }
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
                    FollowedOrganisers.Add(EventData.Child(Application.Context.Resources.GetString(Resource.String.organiser_id_column)).Value.ToString(), EventData.Key);
                }
                FollowedOrganisersRetrived.Invoke(this, new FollowedOrganisersDataArgs { FollowedOrganisers = this.FollowedOrganisers });
            }
        }
    }
}