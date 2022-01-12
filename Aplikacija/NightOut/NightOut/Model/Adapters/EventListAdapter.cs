using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace NightOut.Model.Adapters
{
    class EventListAdapter : BaseAdapter<IListAdapter>
    {
        private Context _listContext;
        private List<NightOut.Model.Event> _eventsList;
        private FragmentActivity _parentActivity;

        public FragmentActivity ParentActivity
        {
            set => _parentActivity = value;
        }

        #region Constructors
    
        public EventListAdapter(Context listContext, List<Event> eventsList)
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
            View newView = View.Inflate(_listContext, Resource.Layout.organizator_event_container,null);

            newView.FindViewById<TextView>(Resource.Id.organiserEventContainerEventDate).Text 
                = _eventsList.ElementAt(position).DateOfEvent.ToString(_listContext.Resources
                        .GetString(Resource.String.date_format));

            newView.FindViewById<TextView>(Resource.Id.organiserEventContainerEventCity).Text
                = _eventsList.ElementAt(position).City;

            newView.FindViewById<TextView>(Resource.Id.organiserEventContainerEventAddress).Text
                = _eventsList.ElementAt(position).Address;

            Button EditEventButton = newView.FindViewById<Button>(Resource.Id.organiserEventContainerEdit);

            EditEventButton.Tag = position;
            EditEventButton.Click += OnEditEventClicked;
            return newView;
        }

        private void OnEditEventClicked(object sender, EventArgs e)
        {
            var Transaction = _parentActivity.SupportFragmentManager.BeginTransaction();
            Fragments.EditEventFragment EditFragment = new Fragments.EditEventFragment
            {
                EditEvent = _eventsList.ElementAt((int)((Button)sender).Tag)
            };
            Transaction.Replace(Resource.Id.organizatorProfileFragmentContainer, EditFragment);
            Transaction.AddToBackStack(null);
            Transaction.Commit();
        }
    }
}