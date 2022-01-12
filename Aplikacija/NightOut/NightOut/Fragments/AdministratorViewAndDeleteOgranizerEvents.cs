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
using NightOut.Model;
using NightOut.Model.Adapters;
using NightOut.Model.EventListeners;

namespace NightOut.Fragments
{
    [Activity(Label = "@string/app_name", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/Icon")]
    public class AdministratorViewAndDeleteOgranizerEvents : Android.Support.V4.App.Fragment
    {
        private AdministratorDeleteOrganiserPage _parent;

        private Button _addEvents;
        private ListView _allEvents;
        private string _orgId;


        public string OrgId { get => _orgId; set => _orgId = value; }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _parent = (AdministratorDeleteOrganiserPage)this.ParentFragment;
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            View view = inflater.Inflate(Resource.Layout.organizator_my_events_page, container, false);
            return view;
        }
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            SetupAttributes();
            //FillAttributes();
        }

        private void SetupAttributes()
        {
            _addEvents = View.FindViewById<Button>(Resource.Id.organiserMyEventsAddEvent);
            _addEvents.Visibility = ViewStates.Invisible;
            _allEvents = View.FindViewById<ListView>(Resource.Id.organiserMyEventsPageEventContainer);

            GetAllEvents(DrawEvents);
            
        }

        public void GetAllEvents(Action<List<Model.Event>> CallBack)
        {
            EventListener Event = new EventListener();
            Event.EventsRetrived += (object sender, EventListener.EventDataArgs e) => {
                CallBack(e.Events);
            };
            FirebaseCommunicator.GetDatabase().Reference
                .Child(Application.Context.Resources.GetString(Resource.String.events_table_name))
                .OrderByChild(Application.Context.Resources.GetString(Resource.String.organiser_id_column))
                .EqualTo(_orgId)
                .AddValueEventListener(Event);
        }

        public void Refresh()
        {
            GetAllEvents(DrawEvents);
        }

        public void DrawEvents(List<Model.Event> events)
        {
            DeleteEventListAdapter EventsAdapter = new DeleteEventListAdapter(Activity, events);
            EventsAdapter.ParentActivity = this;
            _allEvents.Adapter = EventsAdapter;
        }
    }
}