using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using NightOut.Fragments;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace NightOut.Model.Adapters
{
   public class DeleteEventListAdapter: BaseAdapter<IListAdapter>
    {
        private Context _listContext;
        private List<NightOut.Model.Event> _eventsList;
        private SupportFragment _parentActivity;

        public SupportFragment ParentActivity
        {
            set => _parentActivity = value;
        }

        #region Constructors

        public DeleteEventListAdapter(Context listContext, List<Event> eventsList)
        {
            _listContext = listContext;
            _eventsList = eventsList;
        }

        #endregion

        public override IListAdapter this[int position] => (IListAdapter)_eventsList.ElementAt(position);

        public override int Count => _eventsList.Count;

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View newView = View.Inflate(_listContext, Resource.Layout.organizator_event_container, null);

            newView.FindViewById<TextView>(Resource.Id.organiserEventContainerEventDate).Text
                = _eventsList.ElementAt(position).DateOfEvent.ToString(_listContext.Resources
                        .GetString(Resource.String.date_format));

            newView.FindViewById<TextView>(Resource.Id.organiserEventContainerEventCity).Text
                = _eventsList.ElementAt(position).City;

            newView.FindViewById<TextView>(Resource.Id.organiserEventContainerEventAddress).Text
                = _eventsList.ElementAt(position).Address;

            Button EditEventButton = newView.FindViewById<Button>(Resource.Id.organiserEventContainerEdit);
            EditEventButton.Text = "Delete";

            EditEventButton.Tag = position;
            EditEventButton.Click += OnEditEventClicked;
            return newView;
        }

        private void OnEditEventClicked(object sender, EventArgs e)
        {
           
            Event ev = _eventsList.ElementAt((int)((Button) sender).Tag);
       

            Model.FirebaseCommunicator.GetDatabase().Reference
                .Child(Application.Context.Resources.GetString(Resource.String.events_table_name))
                .Child(ev.Id)
                .RemoveValue();

            Model.FirebaseCommunicator.GetDatabase().Reference
                .Child(Application.Context.Resources.GetString(Resource.String.user_followed_events_table_name))
                .OrderByChild(Application.Context.Resources.GetString(Resource.String.event_id_column))
                .EqualTo(ev.Id)
                .AddListenerForSingleValueEvent(new Model.EventListeners.RemoveFollowedEventsListener {
                    Action = ((AdministratorViewAndDeleteOgranizerEvents)_parentActivity).Refresh
                }) ;

            
        }
    }
}