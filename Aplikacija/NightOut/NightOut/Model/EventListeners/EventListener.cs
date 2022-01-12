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
    public class EventListener : Java.Lang.Object, IValueEventListener
    {

        private List<Event> _eventList = new List<Event>();

        public event EventHandler<EventDataArgs> EventsRetrived;

        public class EventDataArgs : EventArgs
        {
            public List<Event> Events { get; set; }
        }

        public void OnCancelled(DatabaseError error)
        {}

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                DataSnapshot[] AllData = snapshot.Children.ToEnumerable<DataSnapshot>().ToArray<DataSnapshot>();
                foreach (DataSnapshot EventData in AllData)
                {
                    _eventList.Add(ReadFromData(EventData));
                }
                EventsRetrived.Invoke(this, new EventDataArgs { Events = _eventList });
            }
        }

        public static Event ReadFromData(DataSnapshot EventData)
        {
            Event ResaultEvent = new Event(EventData.Key)
            {
                Description = EventData.Child(Application.Context.Resources
                        .GetString(Resource.String.event_description_column)).Value.ToString(),

                Address = EventData.Child(Application.Context.Resources
                        .GetString(Resource.String.event_address_column)).Value.ToString(),

                City = EventData.Child(Application.Context.Resources
                        .GetString(Resource.String.event_city_column)).Value.ToString(),

                TypeOfEvent = (EventType)int.Parse(EventData.Child(Application.Context.Resources
                        .GetString(Resource.String.event_type_of_event_column)).Value.ToString()),

                OrganiserId = EventData.Child(Application.Context.Resources
                        .GetString(Resource.String.organiser_id_column)).Value.ToString(),

                Image = EventData.Child(Application.Context.Resources
                        .GetString(Resource.String.event_image_column)).Value.ToString()
            };

            DateTime Date = DateTime.ParseExact(
                EventData.Child(Application.Context.Resources.GetString(Resource.String.event_date_of_event_column)).Value.ToString(),
                Application.Context.Resources.GetString(Resource.String.date_format), null);
            if (Date == DateTime.MaxValue)
            {
                ResaultEvent.DateOfEvent = DateTime.MaxValue;
            }
            else
            {
                ResaultEvent.DateOfEvent = Date;
            }
            return ResaultEvent;
        }
    }
}