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

using Java.Util;

namespace NightOut.Model
{
    public enum EventType
    {
        Undefined,
        Party,
        Festival
    }
    public class Event
    {
        #region Attributes
        private string _id;
        private string _name;
        private string _description;
        private string _address;
        private string _city;
        private EventType _typeOfEvent;
        private string _timeOfEvent;
        private DateTime _dateOfEvent;
        private string _image;

        private string _organiserId;
        #endregion

        #region Properties

        public string Image
        {
            get => _image;
            set => _image = value;
        }
        
        public string City
        {
            get => _city;
            set => _city = value;
        }

        public string OrganiserId
        {
            get => _organiserId;
            set => _organiserId = value;
        }

        public string Address
        {
            get => _address;
            set => _address = value;
        }

        public DateTime DateOfEvent
        {
            get
            {
                return this._dateOfEvent;
            }
            set
            {
                this._dateOfEvent = value;
            }
        }

        public string TimeOfEvent
        {
            get
            {
                return this._timeOfEvent.ToString();
            }
            set
            {
                this._timeOfEvent = value;
            }
        }

        public string Description
        {
            get
            {
                return this._description.ToString();
            }
            set
            {
                this._description = value;
            }
        }

        public EventType TypeOfEvent
        {
            get
            {
                return this._typeOfEvent;
            }
            set
            {
                this._typeOfEvent = value;
            }
        }

        public string Name
        {
            get
            {
                return this._name.ToString();
            }
            set
            {
                this._name = value;
            }
        }

        public string Id
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
            }
        }
        #endregion

        #region Constructors
        public Event()
        {
            _id = null;
            _name = null;
            _dateOfEvent =DateTime.MaxValue;
            _timeOfEvent = null;
            _description = null;
            _typeOfEvent = EventType.Undefined;
            _image = string.Empty;
        }
        public Event(string NewId)
        {
            _id = NewId;
            _name = null;
            _dateOfEvent = DateTime.MaxValue;
            _timeOfEvent = null;
            _description = null;
            _typeOfEvent = EventType.Undefined;
            _image = string.Empty;
        }

        public Event(string NewId,string NewName, string NewDescription, DateTime NewDateOfEvent,
            string NewTimeOfEvent, EventType NewEventType)
        {
            _id = NewId;
            _name = NewName.ToString();
            _dateOfEvent = NewDateOfEvent;
            _timeOfEvent = NewTimeOfEvent.ToString();
            _description = NewDescription.ToString();
            _typeOfEvent = NewEventType;
            _image = string.Empty;
        }

        public Event(Event CopyEvent)
        {
            _id = CopyEvent._id;
            _name = CopyEvent._name;
            _dateOfEvent = CopyEvent._dateOfEvent;
            _timeOfEvent = CopyEvent._timeOfEvent;
            _description = CopyEvent._description;
            _typeOfEvent = CopyEvent._typeOfEvent;
            _image = string.Empty;
        }

        #endregion

        #region Methods

        public static HashMap ToHashMap(Event Event)
        {
            HashMap eventMap = new HashMap();

            if (Event.Address != null)
                eventMap.Put(
                    Application.Context.Resources.GetString(Resource.String.event_address_column),
                    Event.Address);
            else
                eventMap.Put(
                    Application.Context.Resources.GetString(Resource.String.event_address_column),
                    String.Empty);

            if (Event.City != null)
                eventMap.Put(
                    Application.Context.Resources.GetString(Resource.String.event_city_column),
                    Event.City);
            else
                eventMap.Put(
                    Application.Context.Resources.GetString(Resource.String.event_city_column),
                    String.Empty);

            if (Event.DateOfEvent != DateTime.MaxValue)
                eventMap.Put(
                    Application.Context.Resources.GetString(Resource.String.event_date_of_event_column),
                    Event.DateOfEvent.ToString(Application.Context.Resources.GetString(Resource.String.date_format)));
            else
                eventMap.Put(
                    Application.Context.Resources.GetString(Resource.String.event_date_of_event_column),
                    DateTime.MaxValue.ToString(Application.Context.Resources.GetString(Resource.String.date_format)));

            if (Event.Description != null)
                eventMap.Put(Application.Context.Resources.GetString(Resource.String.event_description_column), Event.Description);
            else eventMap.Put(Application.Context.Resources.GetString(Resource.String.event_description_column), String.Empty);

            if (Event.Image != string.Empty)
                eventMap.Put(Application.Context.Resources.GetString(Resource.String.event_image_column), Event.Image);
            else eventMap.Put(Application.Context.Resources.GetString(Resource.String.event_image_column), String.Empty);

            if (Event.OrganiserId != null)
                eventMap.Put(Application.Context.Resources.GetString(Resource.String.organiser_id_column), Event.OrganiserId);
            else eventMap.Put(Application.Context.Resources.GetString(Resource.String.organiser_id_column), String.Empty);

            if (Event.TypeOfEvent != 0)
                eventMap.Put(Application.Context.Resources.GetString(Resource.String.event_type_of_event_column), ((int)Event.TypeOfEvent).ToString());
            else eventMap.Put(Application.Context.Resources.GetString(Resource.String.event_type_of_event_column), "0");

            return eventMap;
        }

        #endregion
    }
}