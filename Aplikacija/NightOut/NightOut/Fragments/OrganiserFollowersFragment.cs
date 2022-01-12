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

namespace NightOut.Fragments
{
    
    public class OrganiserFollowersFragment : Android.Support.V4.App.Fragment
    {

        #region Attributes

        private NightOut.OurView.IOrganiserProfileView _parentActivity;
        private ListView _organiserFollowersList;
        private string _organiserId;

        #endregion

        #region Overriden Methodes

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _parentActivity = (NightOut.OurView.IOrganiserProfileView)Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Android.Views.View view = inflater.Inflate(Resource.Layout.organizator_my_followers_page, container, false);
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

        private void SetUpAttributes()
        {
            _organiserFollowersList = View.FindViewById<ListView>(Resource.Id.oragniserFollowersList);
            _organiserId = this.Arguments.GetString(Resources.GetString(Resource.String.organiser_id_column));
        }

        private void FillOutList()
        {
            NightOut.Model.EventListeners.UserFollowersListener UserRetriver = new NightOut.Model.EventListeners.UserFollowersListener();
            UserRetriver.FollowersRetrived += OnFollowersRetrived;
            NightOut.Model.FirebaseCommunicator.GetDatabase()
                .Reference
                .Child(Resources.GetString(Resource.String.user_followed_organisers_table_name))
                .OrderByChild(Resources.GetString(Resource.String.organiser_id_column))
                .EqualTo(_organiserId)
                .AddValueEventListener(UserRetriver);
        }

        private void OnFollowersRetrived(object sender, Model.EventListeners.UserFollowersListener.FollowersDataArgs e)
        {
            Model.Adapters.FollowersListAdapter FollowersAdapter = new Model.Adapters.FollowersListAdapter(Activity, e.Followers)
            {
                ParentActivity = Activity
            };
            _organiserFollowersList.Adapter = FollowersAdapter;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _parentActivity.IsActive = true;
        }

        #endregion
    }
}