using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using NightOut.Model.EventListeners;

namespace NightOut.Fragments
{
    public class UserFollowedOrganisersFragment : Android.Support.V4.App.Fragment
    {

        #region Attributes

        private NightOut.OurView.IUserProfileView _parentActivity;
        private ListView _followedOrganisers;
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
            Android.Views.View view = inflater.Inflate(Resource.Layout.user_followed_organisers_page, container, false);
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
            _followedOrganisers = View.FindViewById<ListView>(Resource.Id.userFollowedOrganisersContainer);
            _userId = this.Arguments.GetString(Resources.GetString(Resource.String.user_id_column));

        }

        private void FillOutList()
        {
            NightOut.Model.EventListeners.FollowedOrganisersListener OrganisersRetriver = new NightOut.Model.EventListeners.FollowedOrganisersListener();
            OrganisersRetriver.OrganisersRetrived += OnOrganisersRetrived;
            NightOut.Model.FirebaseCommunicator.GetDatabase()
                .Reference
                .Child(Resources.GetString(Resource.String.user_followed_organisers_table_name))
                .OrderByChild(Resources.GetString(Resource.String.user_id_column))
                .EqualTo(_userId)
                .AddValueEventListener(OrganisersRetriver);
        }

        private void OnOrganisersRetrived(object sender, FollowedOrganisersListener.OrganisersDataArgs e)
        {
            Model.Adapters.OrganisersListAdapter FollowedOrganisers = new Model.Adapters.OrganisersListAdapter(Activity, e.Organisers)
            {
                ParentActivity = Activity
            };
            _followedOrganisers.Adapter = FollowedOrganisers;
        }

        #endregion

    }
}