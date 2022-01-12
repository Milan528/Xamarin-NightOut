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
using Android.Content.Res;
using Firebase.Auth;
using Firebase.Database;
using Firebase;
using NightOut.Activities;
using NightOut.Model;
using NightOut.Model.EventListeners;
using Android.Graphics;

namespace NightOut.Presenter
{
    public class OrganiserProfilePresenter : ImagePresenter
    {
        #region Attributes

        private OrganiserProfileActivity _parentActivity;
        private FirebaseAuth _firebaseAuth;
        private Organiser _logedInOrganiser;
        private FirebaseCommunicator _communicator;

        #endregion

        #region Properties

        public Organiser LogedInUser
        {
            get
            {
                FirebaseUser organiser = _firebaseAuth.CurrentUser;
                if (_logedInOrganiser == null)
                {
                    SetUpOrganiser();
                }
                return this._logedInOrganiser;
            }
        }

        #endregion

        #region Constructor

        public OrganiserProfilePresenter(OrganiserProfileActivity ParentActivity)
        {
            _parentActivity = ParentActivity;
            _firebaseAuth = FirebaseAuth.Instance;
            _communicator = new FirebaseCommunicator(_parentActivity);
            _logedInOrganiser = null;
        }

        #endregion

        #region Methodes

        public void SetUpOrganiser()
        {
            FirebaseUser organiser = _firebaseAuth.CurrentUser;
            if (organiser != null || _logedInOrganiser == null)
            {
                OrganiserListener Event = new OrganiserListener();
                Event.OrganiserRetrived += (object sender, OrganiserListener.OrganiserDataEventArgs e) => {
                    this._logedInOrganiser = e.Organiser;
                    _parentActivity.UpdateInterface();
                };
                FirebaseCommunicator.GetDatabase()
                    .Reference.Child(Application.Context.GetString(Resource.String.organisers_table_name))
                    .Child(organiser.Uid)
                    .AddValueEventListener(Event);
            }
        }

        public void SaveUserChanges() => _communicator.UpdatePasswordAndEmail(_logedInOrganiser);

        public void GetAllEvents(Action<List<Event>> CallBack)
        {
            if (_logedInOrganiser != null)
            {
                EventListener Event = new EventListener();
                Event.EventsRetrived += (object sender, EventListener.EventDataArgs e) => {
                    CallBack(e.Events);
                };
                FirebaseCommunicator.GetDatabase().Reference
                    .Child(Application.Context.Resources.GetString(Resource.String.events_table_name))
                    .OrderByChild(Application.Context.Resources.GetString(Resource.String.organiser_id_column))
                    .EqualTo(_logedInOrganiser.Id)
                    .AddValueEventListener(Event);
            }
        }

        public void CreateEvent(NightOut.Model.Event EventToBeCreated)
        {
            Java.Util.HashMap eventMap = Event.ToHashMap(EventToBeCreated);
            //eventMap.Put("Name", EventToBeCreated.Name);
            DatabaseReference localReference = FirebaseCommunicator
                .GetDatabase()
                .GetReference(Application.Context.Resources.GetString(Resource.String.events_table_name)).Push();
            localReference.SetValue(eventMap);
        }

        public void UpdateEvent(NightOut.Model.Event EventToBeEdited)
        {
            DatabaseReference DB = FirebaseCommunicator.GetDatabase()
                .Reference
                .Child(Application.Context.Resources.GetString(Resource.String.events_table_name) + "/" + EventToBeEdited.Id);
            Java.Util.HashMap EventMap = Event.ToHashMap(EventToBeEdited);
            DB.SetValueAsync(EventMap);
        }

        public void PutImageInDatabase(Bitmap Image)
        {
            LogedInUser.Image = Presenter.ImagePresenter.CompressImage(Image);

            Java.Util.HashMap organiserMap = Organiser.ToHashMap(_logedInOrganiser);

            NightOut.Model.FirebaseCommunicator.GetDatabase().Reference
                .Child(Application.Context.Resources.GetString(Resource.String.organisers_table_name))
                .Child(_logedInOrganiser.Id)
                .SetValueAsync(organiserMap);
        }

        public void GetRating(Action<double> Callback)
        {
            Model.EventListeners.OrganiserRatingListener RatingRetriver = new OrganiserRatingListener();
            RatingRetriver.RatingRetrived += (object sender, Model.EventListeners.OrganiserRatingListener.RatingArgs e) =>
            {
                _logedInOrganiser.Rating = e.Rating;
                Callback(e.Rating);
            };
            Model.FirebaseCommunicator.GetDatabase().Reference
                .Child(Application.Context.Resources.GetString(Resource.String.user_rate_organiser_table_name))
                .OrderByChild(Application.Context.Resources.GetString(Resource.String.organiser_id_column))
                .EqualTo(_logedInOrganiser.Id)
                .AddListenerForSingleValueEvent(RatingRetriver);
        }

        #endregion
    }

}