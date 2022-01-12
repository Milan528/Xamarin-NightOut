using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupportFragment = Android.Support.V4.App.Fragment;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using NightOut.Presenter;
using NightOut.Model;
using NightOut.Model.Adapters;
using NightOut.OurView;
using NightOut.Fragments;

namespace NightOut.Activities
{
    [Activity(Label = "@string/app_name", MainLauncher = false, LaunchMode = Android.Content.PM.LaunchMode.SingleInstance, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        Icon = "@drawable/Icon")]
    public class UserMainPageActivity : Android.Support.V4.App.FragmentActivity, IMainPageView
    {
        #region Attributes
        private ListView _listView;
        private Spinner _searchEventType;
        private EditText _searchCity;
        private ImageView _administratorPanel;
        private ImageView _myProfile;
        private ImageView _search;
        private TextView _searchDate;
        private UserEventListAdapter _listAdapter;
        private UserMainPagePresenter _userMainPagePresenter;
        private List<Event> _displayedEvents;
        private ProgressDialog _progress;

        #endregion

        #region Override

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.user_main_page);
            _userMainPagePresenter = new UserMainPagePresenter(this);
            ShowProgres();
            SetUpAttributes();
            SetUpListeners();
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (_listAdapter != null)
                _listView.Adapter = _listAdapter;
        }

        #endregion

        #region Methods

        private void SetUpAttributes()
        {
            _listView = FindViewById<ListView>(Resource.Id.userEventsView);
            _searchEventType = FindViewById<Spinner>(Resource.Id.userSearchEventType);
            _searchCity = FindViewById<EditText>(Resource.Id.userSearchCity);
            LoggedUser.SetUpUser(SetUpAdministratorPanel);
            _myProfile = FindViewById<ImageView>(Resource.Id.goToUserProfile);
            _search = FindViewById<ImageView>(Resource.Id.filteringMagnifier);
            _searchDate = FindViewById<TextView>(Resource.Id.searchDate);
            _searchEventType = FindViewById<Spinner>(Resource.Id.userSearchEventType);
            string[] EventTypes = Enum.GetNames(typeof(NightOut.Model.EventType));
            _searchEventType.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, EventTypes);
            LoggedUser.SetUpUser(GetEvents);
        }

        private void SetUpAdministratorPanel(User LoggedUser)
        {
            if (!LoggedUser.IsAdministrator)
                return;
            _administratorPanel = FindViewById<ImageView>(Resource.Id.goToAdminPanel);
            _administratorPanel.Visibility = ViewStates.Visible;
            _administratorPanel.Click += OpenAdministratorPanel;
        }

        private void OpenAdministratorPanel(object sender, EventArgs e)
        {
            StartActivity(typeof(AdministratorPanel));
        }

        private void SetUpListeners()
        {
            _myProfile.Click += OnMyProfileClicked;
            _search.Click += OnSearchClicked;
            _searchDate.Click += OnDateDisplayClicked;
        }

        private List<Event> FilterEventsDate(List<Event> events, string text)
        {
            List<Event> FilteredEvents = new List<Event>();
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].DateOfEvent.ToString(Resources.GetString(Resource.String.date_format)) == text)
                    FilteredEvents.Add(events[i]);
            }
            return FilteredEvents;
        }

        private List<Event> FilterEventsCity(List<Event> events, string text)
        {
            List<Event> FilteredEvents = new List<Event>();
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].City == text)
                    FilteredEvents.Add(events[i]);
            }
            return FilteredEvents;
        }

        private List<Event> FilterEventsType(List<Event> Events, int Type)
        {
            List<Event> FilteredEvents = new List<Event>();
            for (int i = 0; i < Events.Count; i++)
            {
                if (Events[i].TypeOfEvent == (EventType)Type)
                    FilteredEvents.Add(Events[i]);
            }
            return FilteredEvents;
        }

        private bool CheckForRefresh()
        {
            if (_searchDate.Text == "Date" && _searchCity.Text == "" && _searchEventType.SelectedItem.ToString() == "Undefined")
                return true;
            else
                return false;
        }

        private void Refresh() => GetEvents(LoggedUser.LoggedInUser);

        private void OpenFragment(SupportFragment newFragment)
        {
            if (newFragment.IsVisible)
                return;
            var Transaction = SupportFragmentManager.BeginTransaction();
            Transaction.Replace(Resource.Id.userMainPageFragmentContainer, newFragment);
            Transaction.AddToBackStack(null);
            Transaction.Commit();
        }

        private void DrawEvents(List<NightOut.Model.Event> RetrivedEvents)
        {
            _displayedEvents = RetrivedEvents;
            UserEventListAdapter EventsAdapter = new UserEventListAdapter(this, RetrivedEvents);
            EventsAdapter.ParentActivity = this;
            _listAdapter = EventsAdapter;
            _listView.Adapter = EventsAdapter;
            StopProgres();
        }

        private void GetEvents(User LogedUser) => _userMainPagePresenter.GetAllEvents(DrawEvents);

        public void OpenFragment(NightOut.Model.Event EventToBeOpened)
        {
            UserViewEventFragment fragment = new UserViewEventFragment();
            fragment.ViewEvent = EventToBeOpened;
            OpenFragment(fragment);
        }

        #endregion

        #region Listeners

        private void OnDateDisplayClicked(object sender, EventArgs e)
        {
            DatePickerDialog Dialog = new DatePickerDialog(this, Android.Resource.Style.ThemeHoloLightDialogMinWidth, OnDatePickerChosed, DateTime.Today.Year, DateTime.Today.Month - 1, DateTime.Today.Day);
            Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
            Dialog.Show();
        }

        private void OnDatePickerChosed(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            if (e.Date < DateTime.Today)
                Toast.MakeText(this, Resource.String.date_of_event_invalid_input, ToastLength.Short).Show();
            else
            {
                DateTime Proba = e.Date.ToUniversalTime().AddDays(1);
                _searchDate.Text = Proba.ToString(Resources.GetString(Resource.String.date_format));
            }
        }

        private void OnSearchClicked(object sender, EventArgs e)
        {
            if (CheckForRefresh())
                Refresh();
            else
            {
                if (_searchDate.Text != "Date")
                    _displayedEvents = FilterEventsDate(_displayedEvents, _searchDate.Text);
                if (_searchCity.Text != "")
                    _displayedEvents = FilterEventsCity(_displayedEvents, _searchCity.Text);
                if (_searchEventType.SelectedItemPosition != (int)EventType.Undefined)
                    _displayedEvents = FilterEventsType(_displayedEvents, _searchEventType.SelectedItemPosition);
                DrawEvents(_displayedEvents);

            }
        }

        private void OnMyProfileClicked(object sender, EventArgs e) => StartActivity(typeof(UserProfileActivity));
        
        public void ShowProgres()
        {
            _progress = new ProgressDialog(this);
            _progress.SetTitle("Loading...");
            _progress.SetMessage("Please wait.");
            _progress.SetCancelable(false);
            _progress.Show();
        }

        public void StopProgres()
        {
            _progress.Dismiss();
        }
        #endregion

    }
}