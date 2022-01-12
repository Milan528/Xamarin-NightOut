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
    class ViewFollowedEventsFragment : Android.Support.V4.App.Fragment
    {
        private NightOut.Model.Event _viewEvent;

        private TextView _city;
        private TextView _address;
        private TextView _organiser;
        private TextView _eventType;
        private TextView _dateOfEvent;
        private TextView _description;
        private Button _followButton;
        private ImageView _eventImage;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.user_events_view_event, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            SetUpAttributes();
            FillOutAttributes();
            _city = view.FindViewById<TextView>(Resource.Id.userEventCityText);
            _city.Text = _viewEvent.City;
        }

        public void SetUpAttributes()
        {
            _city = View.FindViewById<TextView>(Resource.Id.userEventCityText);
            _city.Click += OnAddressClicked;
            _address = View.FindViewById<TextView>(Resource.Id.userEventAddressText);
            _address.Click += OnAddressClicked;
            _organiser = View.FindViewById<TextView>(Resource.Id.userEventOrganiserText);
            _organiser.Click += OnOrganiserClicked;
            _eventType = View.FindViewById<TextView>(Resource.Id.userEventTypeText);
            _description = View.FindViewById<TextView>(Resource.Id.userEventDescription);
            _dateOfEvent = View.FindViewById<TextView>(Resource.Id.userEventDateText);
            _followButton = View.FindViewById<Button>(Resource.Id.userEventsViewEventUnFollowEvent);
            SetUpButton();
            _followButton.Click += OnFollowButtonClicked;
            _eventImage = View.FindViewById<ImageView>(Resource.Id.UserEventImage);
        }

        private void OnOrganiserClicked(object sender, EventArgs e)
        {
            var Transaction = FragmentManager.BeginTransaction();
            UserViewOrganiserFragment NewFragment = new UserViewOrganiserFragment
            {
                OrganiserId = _viewEvent.OrganiserId
            };
            Transaction.Replace(Resource.Id.UserProfileFragmentContainer, NewFragment);
            Transaction.AddToBackStack(null);
            Transaction.Commit();
        }

        private void SetUpButton()
        {
            if (Model.LoggedUser.LoggedInUser != null)
            {
                NightOut.Model.EventListeners.FollowedEventsListener Listener = new NightOut.Model.EventListeners.FollowedEventsListener();
                Listener.FollowedEventsRetrived += (object sender, NightOut.Model.EventListeners.FollowedEventsListener.FollowedEventsDataArgs e) => {
                    if (!e.FollowedEvents.ContainsKey(_viewEvent.Id))
                        _followButton.Text = "Follow";
                    else
                    {
                        _followButton.Text = "Unfollow event";
                        e.FollowedEvents.TryGetValue(_viewEvent.Id, out string Key);
                        _followButton.Tag = Key;
                    }
                };
                Model.FirebaseCommunicator.GetDatabase().Reference
                    .Child(Resources.GetString(Resource.String.user_followed_events_table_name))
                    .OrderByChild(Resources.GetString(Resource.String.user_id_column))
                    .EqualTo(Model.LoggedUser.LoggedInUser.Id)
                    .AddListenerForSingleValueEvent(Listener);
            }
        }

        private void OnFollowButtonClicked(object sender, EventArgs e)
        {
            if (_followButton.Text == "Follow")
            {
                _followButton.Text = "Unfollow event";
                Java.Util.HashMap localMap = new Java.Util.HashMap();
                localMap.Put(Resources.GetString(Resource.String.event_id_column), _viewEvent.Id);
                localMap.Put(Resources.GetString(Resource.String.user_id_column), Model.LoggedUser.LoggedInUser.Id);

                Firebase.Database.DatabaseReference localReference = Model.FirebaseCommunicator.GetDatabase()
                    .GetReference(Resources.GetString(Resource.String.user_followed_events_table_name)).Push();
                _followButton.Tag = localReference.Key;
                localReference.SetValue(localMap);
            }
            else
            {
                _followButton.Text = "Follow";
                Model.FirebaseCommunicator.GetDatabase().Reference
                    .Child(Resources.GetString(Resource.String.user_followed_events_table_name))
                    .Child((string)_followButton.Tag).RemoveValue();
                _followButton.Tag = null;
            }

        }

        private void OnAddressClicked(object sender, EventArgs e) => OpenMaps(_city.Text + "+" + _address.Text);

        private void FillOutAttributes()
        {
            _city.Text = _viewEvent.City;
            _address.Text = _viewEvent.Address;
            _eventType.Text = _viewEvent.TypeOfEvent.ToString();
            _description.Text = _viewEvent.Description;
            _dateOfEvent.Text = _viewEvent.DateOfEvent.ToString(Resources.GetString(Resource.String.date_format));
            NightOut.Model.EventListeners.OrganiserListener Listener = new Model.EventListeners.OrganiserListener();
            if (_viewEvent.Image != String.Empty)
                _eventImage.SetImageBitmap(DecodeImg(_viewEvent.Image));
            Listener.OrganiserRetrived += (object sender, Model.EventListeners.OrganiserListener.OrganiserDataEventArgs e) =>
            {
                _organiser.Text = e.Organiser.UserName;
            };
            NightOut.Model.FirebaseCommunicator.GetDatabase()
                .Reference.Child(Resources.GetString(Resource.String.organisers_table_name)).Child(_viewEvent.OrganiserId)
                .AddListenerForSingleValueEvent(Listener);
        }

        public Android.Graphics.Bitmap DecodeImg(string profilePictureString)
        {
            byte[] decodedByteArray = Android.Util.Base64.Decode(profilePictureString, Android.Util.Base64Flags.Default);
            return Android.Graphics.BitmapFactory.DecodeByteArray(decodedByteArray, 0, decodedByteArray.Length);
        }

        public NightOut.Model.Event ViewEvent
        {
            get => _viewEvent;
            set => _viewEvent = value;
        }

        public void OpenMaps(string Address)
        {
            Android.Net.Uri AddressURI = Android.Net.Uri.Parse(Resources.GetString(Resource.String.maps_query) + Address);
            Intent mapIntent = new Intent(Intent.ActionView, AddressURI);
            mapIntent.SetPackage(Resources.GetString(Resource.String.maps_package));
            if (mapIntent.ResolveActivity(Activity.PackageManager) != null)
            {
                StartActivity(mapIntent);
            }
        }
    }
}