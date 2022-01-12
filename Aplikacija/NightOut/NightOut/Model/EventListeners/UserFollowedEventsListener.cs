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
    class UserFollowedEventsListener : Java.Lang.Object, IValueEventListener
    {
        private List<NightOut.Model.Event> _events = new List<NightOut.Model.Event>();
        private int _numOfEvents = 0;
        public event EventHandler<EventsDataArgs> EventsRetrived;

        public class EventsDataArgs : EventArgs
        {
            public List<NightOut.Model.Event> Events { get; set; }
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                var AllData = snapshot.Children.ToEnumerable<DataSnapshot>().ToArray<DataSnapshot>();
                _numOfEvents = AllData.Count();
                foreach (DataSnapshot EventData in AllData)
                {
                    SingleEventListener EventRetriver = new SingleEventListener();
                    EventRetriver.EventRetrived += OnEventRetrived;
                    NightOut.Model.FirebaseCommunicator.GetDatabase().Reference
                        .Child(Application.Context.Resources.GetString(Resource.String.events_table_name))
                        .Child(EventData.Child(Application.Context.Resources.GetString(Resource.String.event_id_column)).Value.ToString())
                        .AddListenerForSingleValueEvent(EventRetriver);
                }
            }
        }

        private void OnEventRetrived(object sender, SingleEventListener.EventDataArgs e)
        {
            _events.Add(e.Event);
            if (_events.Count == _numOfEvents)
                EventsRetrived.Invoke(this, new EventsDataArgs { Events = this._events });
        }

        public void OnCancelled(DatabaseError error)
        {
        }
    }
}