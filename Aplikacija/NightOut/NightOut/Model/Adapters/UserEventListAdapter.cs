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
using NightOut.OurView;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace NightOut.Model.Adapters
{
    class UserEventListAdapter : BaseAdapter<IListAdapter>
    {
        private Context _listContext;
        private List<NightOut.Model.Event> _eventsList;
        private FragmentActivity _parentActivity;

        public FragmentActivity ParentActivity
        {
            set => _parentActivity = value;
        }

        public UserEventListAdapter(Context listContext, List<Event> eventsList)
        {
            _listContext = listContext;
            _eventsList = eventsList;
        }

        public override IListAdapter this[int position] => (IListAdapter)_eventsList.ElementAt(position);

        public override int Count => _eventsList.Count;

        public object SupportFragmentManager { get; private set; }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View newView = View.Inflate(_listContext, Resource.Layout.user_event_container, null);
            newView.FindViewById<TextView>(Resource.Id.userEventContainerEventAddress).Text
                = _eventsList.ElementAt(position).Address;

            newView.FindViewById<TextView>(Resource.Id.userEventContainerEventCity).Text
                = _eventsList.ElementAt(position).City;

            newView.FindViewById<TextView>(Resource.Id.userEventContainerEventDate).Text
                = _eventsList.ElementAt(position).DateOfEvent.ToString(Application.Context.GetString(Resource.String.date_format));

            if(_eventsList.ElementAt(position).Image!=String.Empty)
                newView.FindViewById<ImageView>(Resource.Id.userEventcontainerPicture)
                    .SetImageBitmap(Presenter.ImagePresenter.DecodeImg(_eventsList.ElementAt(position).Image));

            Button EditEventButton = newView.FindViewById<Button>(Resource.Id.userEventContainerView);
            EditEventButton.Tag = position;
            EditEventButton.Click += OnBtnViewClicked;
            return newView;
        }

        private void OnBtnViewClicked(object sender, EventArgs e)
        {
            var Transaction = _parentActivity.SupportFragmentManager.BeginTransaction();
            NightOut.Fragments.UserViewEventFragment ViewEventFragment = new NightOut.Fragments.UserViewEventFragment();
            ViewEventFragment.ViewEvent = _eventsList.ElementAt((int)((Button)sender).Tag);
            Transaction.Replace(Resource.Id.userMainPageFragmentContainer, ViewEventFragment);
            Transaction.AddToBackStack(null);
            Transaction.Commit();
        }
    }

}