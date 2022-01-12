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
using NightOut.Activities;
using NightOut.Model;
using NightOut.Model.EventListeners;

namespace NightOut.Presenter
{
    class UserMainPagePresenter
    {

        private NightOut.OurView.IMainPageView _parentActivity;

        public UserMainPagePresenter(NightOut.OurView.IMainPageView Parent)
        {
            _parentActivity = Parent;
        }

        public void GetAllEvents(Action<List<NightOut.Model.Event>> CallBack)
        {
            if (LoggedUser.LoggedInUser != null)
            {
                EventListener Event = new EventListener();
                Event.EventsRetrived += (object sender, EventListener.EventDataArgs e) => {
                    CallBack(e.Events);
                };
                FirebaseCommunicator.GetDatabase()
                    .Reference
                    .Child(Application.Context.Resources.GetString(Resource.String.events_table_name))
                    .AddListenerForSingleValueEvent(Event);
            }
        }

    }
}