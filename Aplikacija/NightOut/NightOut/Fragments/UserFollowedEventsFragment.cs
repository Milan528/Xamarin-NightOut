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

namespace NightOut.Fragments
{
    class UserFollowedEventsFragment : Android.Support.V4.App.Fragment
    {

        #region Attributes

        private NightOut.OurView.IUserProfileView _parentActivity;
        private ListView _followedEvents;
        private string _userId;

        #endregion

        #region Overriden methodes

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _parentActivity = (NightOut.OurView.IUserProfileView)Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Android.Views.View view = inflater.Inflate(Resource.Layout.user_events_page, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            SetUpAttributes();
            FillOutList();
        }

        #endregion

        #region Methodes

        public void SetUpAttributes()
        {
            _followedEvents = View.FindViewById<ListView>(Resource.Id.userEventsPageEventContainer);
            _userId = this.Arguments.GetString(Resources.GetString(Resource.String.user_id_column));
        }

        private void FillOutList()
        {
            Model.EventListeners.UserFollowedEventsListener EventsRetriver = new Model.EventListeners.UserFollowedEventsListener();
            EventsRetriver.EventsRetrived += OnEventsRetrived;
            NightOut.Model.FirebaseCommunicator.GetDatabase()
                .Reference
                .Child(Resources.GetString(Resource.String.user_followed_events_table_name))
                .OrderByChild(Resources.GetString(Resource.String.user_id_column))
                .EqualTo(_userId)
                .AddListenerForSingleValueEvent(EventsRetriver);
        }

        private void OnEventsRetrived(object sender, Model.EventListeners.UserFollowedEventsListener.EventsDataArgs e)
        {
            Model.Adapters.UserFollowedEventsAdapter FollowedEvents = new Model.Adapters.UserFollowedEventsAdapter(Activity, e.Events)
            {
                ParentActivity = Activity
            };
            _followedEvents.Adapter = FollowedEvents;
        }

        #endregion

    }
}