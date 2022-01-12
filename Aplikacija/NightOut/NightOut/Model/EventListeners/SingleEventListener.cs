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
    public class SingleEventListener : Java.Lang.Object, IValueEventListener
    {
        Model.Event _event;
        public event EventHandler<EventDataArgs> EventRetrived;

        public class EventDataArgs : EventArgs
        {
            public Model.Event Event { get; set; }
        }

        public void OnCancelled(DatabaseError error)
        {
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                Event ResaultEvent = new Event(snapshot.Key)
                {
                    Description = snapshot.Child("Description").Value.ToString(),
                    Address = snapshot.Child("Address").Value.ToString(),
                    City = snapshot.Child("City").Value.ToString(),
                    TypeOfEvent = (EventType)int.Parse(snapshot.Child("TypeOfEvent").Value.ToString()),
                    OrganiserId = snapshot.Child("OrganiserId").Value.ToString(),
                    Image = snapshot.Child("ImageString").Value.ToString()
                };
                string proba = snapshot.Child("DateOfEvent").Value.ToString();
                DateTime Date = DateTime.ParseExact(proba, "d/M/yyyy", null);
                if (Date == DateTime.MaxValue)
                {
                    ResaultEvent.DateOfEvent = DateTime.MaxValue;
                }
                else
                {
                    ResaultEvent.DateOfEvent = Date;
                }
                _event = ResaultEvent;
                EventRetrived.Invoke(this, new EventDataArgs { Event = ResaultEvent });
            }
        }
    }
}