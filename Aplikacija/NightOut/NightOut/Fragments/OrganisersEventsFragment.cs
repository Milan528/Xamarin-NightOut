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
using SupportFragment = Android.Support.V4.App.Fragment;

namespace NightOut.Fragments
{
    public class OrganisersEventsFragment : Android.Support.V4.App.Fragment
    {
        #region Attributes

        private NightOut.OurView.IOrganiserProfileView _parentActivity;
        private Button _addEvents;
        private ListView _allEvents;
        private string _organiserId;

        #endregion

        #region Overriden methodes

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _parentActivity = (NightOut.OurView.IOrganiserProfileView)this.Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Android.Views.View view = inflater.Inflate(Resource.Layout.organizator_my_events_page, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            SetUpAttributes();
        }

        #endregion

        public void SetUpAttributes()
        {
            _addEvents = View.FindViewById<Button>(Resource.Id.organiserMyEventsAddEvent);
            _addEvents.Click += OnAddEventsClicked;
            _allEvents = View.FindViewById<ListView>(Resource.Id.organiserMyEventsPageEventContainer);
            _organiserId = this.Arguments.GetString(Resources.GetString(Resource.String.organiser_id_column));
            _parentActivity.OrganiserProfilePresenter.GetAllEvents(DrawEvents);
        }

        private void DrawEvents(List<NightOut.Model.Event> RetrivedEvents)
        {
            NightOut.Model.Adapters.EventListAdapter EventsAdapter = new NightOut.Model.Adapters.EventListAdapter(Activity, RetrivedEvents);
            EventsAdapter.ParentActivity = Activity;
            _allEvents.Adapter=EventsAdapter;
        }

        private void UpdateList() => _parentActivity.OrganiserProfilePresenter.GetAllEvents(DrawEvents);

        public override void OnDestroy()
        {
            base.OnDestroy();
            _parentActivity.IsActive = true;
        }

        private void OnAddEventsClicked(object sender, EventArgs e)
        {
            var Transaction = FragmentManager.BeginTransaction();
            Transaction.Replace(Resource.Id.organizatorProfileFragmentContainer, new CreateEventFragment());
            Transaction.AddToBackStack(null);
            Transaction.Commit();
        }


    }
}