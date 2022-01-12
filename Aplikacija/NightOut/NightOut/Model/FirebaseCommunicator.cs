using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firebase.Database;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.Res;
using Firebase.Auth;

using Firebase;
using Java.Lang;
using Java.Util;
using Android.Support.V4.App;
using NightOut.OurView;
using NightOut.Activities;
using Android.Gms.Tasks;
using NightOut.Model.EventListeners;

namespace NightOut.Model
{
    public class FirebaseCommunicator
    {
        #region Attributes
        
        private FirebaseDatabase _firebaseDatabase;
        private TaskCompleteListener _taskComplete = new TaskCompleteListener();
        private readonly Activity _activity;
        private Person _person;
        private Local _local;
        private Event _event;

        #endregion

        #region Propreties

        public Person Person
        {
            get { return this._person; }
            set { this._person = value; }
        }

        public Local Local
        {
            get { return this._local; }
            set { this._local = value; }
        }

        public Event Event
        {
            get { return this._event; }
            set { this._event = value; }
        }
        
        #endregion

        #region Constructor

        public FirebaseCommunicator(Activity activity) => this._activity = activity;

        #endregion

        #region LoginMethods

        public void InitializeDatabaseLogin()
        {
            var app = FirebaseApp.InitializeApp(_activity);
            if (app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetApplicationId(Resources.System.GetString(Resource.String.firebase_aplication_id))
                    .SetApiKey(Resources.System.GetString(Resource.String.firebase_api_key))
                    .SetDatabaseUrl(Resources.System.GetString(Resource.String.firebase_database_url))
                    .SetStorageBucket(Resources.System.GetString(Resource.String.firebase_storage_bucket))
                    .Build();
                app = FirebaseApp.InitializeApp(_activity, options);
            }  
        }

        public void Login(string email, string password)
        {
            _taskComplete.Success += OnTaskCompleteSuccessLogin;
            _taskComplete.Failure += OnTaskCompleteFailureLogin;
            FirebaseAuth.Instance.SignInWithEmailAndPassword(email, password)
                .AddOnSuccessListener(_activity, _taskComplete)
                .AddOnFailureListener(_activity, _taskComplete);
        }

        private void OnTaskCompleteFailureLogin(object sender, EventArgs e)
            => ((OurView.ILogInView)_activity).OnTaskCompleteLoginFailure(sender, e);

        private void OnTaskCompleteSuccessLogin(object sender, EventArgs e)
            => ((OurView.ILogInView)_activity).OnTaskCompleteLoginSuccess(sender, e);

        #endregion

        #region RegistrationMethods

        public void InitializeDatabaseRegistration()
        { 
            var app = FirebaseApp.InitializeApp(_activity);
            if (app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetApplicationId(Resources.System.GetString(Resource.String.firebase_aplication_id))
                    .SetApiKey(Resources.System.GetString(Resource.String.firebase_api_key))
                    .SetDatabaseUrl(Resources.System.GetString(Resource.String.firebase_database_url))
                    .SetStorageBucket(Resources.System.GetString(Resource.String.firebase_storage_bucket))
                    .Build();

                app = FirebaseApp.InitializeApp(_activity, options);
            }
            _firebaseDatabase = FirebaseDatabase.GetInstance(app);
        }
       
        public void RegisterOrganizator(Organiser organiser)
        {
            this._person = organiser;
            _taskComplete.Success += OnTaskCompleteSuccessOrg;
            _taskComplete.Failure += OnTaskCompleteFailureOrg;
            FirebaseAuth.Instance.CreateUserWithEmailAndPassword(this._person.Email, this._person.Password)
                .AddOnSuccessListener(_activity, _taskComplete)
                .AddOnFailureListener(_activity, _taskComplete);
        }

        public void RegisterUser(User user)
        {
            this._person = user;
            _taskComplete.Success += OnTaskCompleteSuccessUser;
            _taskComplete.Failure += OnTaskCompleteFailureUser;
            FirebaseAuth.Instance.CreateUserWithEmailAndPassword(this._person.Email, this._person.Password)
                .AddOnSuccessListener(_activity, _taskComplete)
                .AddOnFailureListener(_activity, _taskComplete);
        }

        private void OnTaskCompleteFailureOrg(object sender, EventArgs e)
            => ((IRegistrationView)_activity).OnOrganizatorRegistrationFailed(sender, e);

        private void OnTaskCompleteFailureUser(object sender, EventArgs e)
            => ((IRegistrationView)_activity).OnUserRegistrationFailed(sender, e);

        private void OnTaskCompleteSuccessOrg(object sender, EventArgs e)
        {
            Organiser org = (Organiser)this._person;

            DatabaseReference localReference =
                _firebaseDatabase.GetReference(Application.Context.Resources.GetString(Resource.String.locals_table_name)).Push();
            localReference.SetValue(Local.ToHashMap(_local));

            HashMap orgMap=Organiser.ToHashMap(org);
            orgMap.Put(Application.Context.Resources.GetString(Resource.String.local_id_column), localReference.Key);

            DatabaseReference orgReference =
                _firebaseDatabase.GetReference(Application.Context.Resources.GetString(Resource.String.organisers_table_name) +"/"+ FirebaseAuth.Instance.CurrentUser.Uid);
            orgReference.SetValue(orgMap);

            ((IRegistrationView)_activity).OnOrganizatorRegistrationSucceed(sender, e);

        }

        private void OnTaskCompleteSuccessUser(object sender, EventArgs e)
        {
            User user = (User)this._person;
            HashMap userMap = User.ToHashMap(user);

            DatabaseReference userReference =
                _firebaseDatabase.GetReference(Application.Context.Resources.GetString(Resource.String.users_table_name) +"/"+ FirebaseAuth.Instance.CurrentUser.Uid);
            userReference.SetValue(userMap);

            ((IRegistrationView)_activity).OnUserRegistrationSucceed(sender, e);
        }

        public static FirebaseDatabase GetDatabase()
        {
            FirebaseApp OurApp = FirebaseApp.InitializeApp(Application.Context);
            FirebaseDatabase OurDatabase;
            if(OurApp == null)
            {
                FirebaseOptions Options = new FirebaseOptions.Builder()
                    .SetApplicationId(Resources.System.GetString(Resource.String.firebase_aplication_id))
                    .SetApiKey(Resources.System.GetString(Resource.String.firebase_api_key))
                    .SetDatabaseUrl(Resources.System.GetString(Resource.String.firebase_database_url))
                    .SetStorageBucket(Resources.System.GetString(Resource.String.firebase_storage_bucket))
                    .Build();
                OurApp = FirebaseApp.InitializeApp(Application.Context, Options);
            }
            OurDatabase = FirebaseDatabase.GetInstance(OurApp);
            return OurDatabase;
        }


        #endregion

        #region Update Methodes
        public void UpdatePasswordAndEmail(Person Person)
        {
            EventListeners.UpdateListener CallBackListener = new EventListeners.UpdateListener();
            CallBackListener.SetAttributes(Person);
            CallBackListener.Updated += OnEmailUpdated;
            FirebaseAuth.Instance.CurrentUser.UpdateEmail(Person.Email).AddOnCompleteListener(CallBackListener);
        }

        private void OnEmailUpdated(object sender, UpdateListener.UpdateData e)
        {
            UpdatePassword(e.UpdatedPerson);
        }

        public void UpdatePassword(Person Person)
        {
            EventListeners.UpdateListener CallBackListener = new EventListeners.UpdateListener();
            CallBackListener.SetAttributes(Person);
            CallBackListener.Updated += OnPasswordUpdated;
            if (Person.Password != null)
                FirebaseAuth.Instance.CurrentUser.UpdatePassword(Person.Password).AddOnCompleteListener(CallBackListener);
            else
                FirebaseAuth.Instance.CurrentUser.UpdateEmail(Person.Email).AddOnCompleteListener(CallBackListener);
        }

        private void OnPasswordUpdated(object sender, UpdateListener.UpdateData e)
        {
            UpdatePersonData(e.UpdatedPerson);
        }

        private void UpdatePersonData(Person Person)
        {
            if(Person is Organiser)
            {
                Organiser OrganiserToUpdate = (Organiser)Person;
                DatabaseReference df = FirebaseCommunicator.GetDatabase()
                    .Reference
                    .Child(Application.Context.Resources.GetString(Resource.String.organisers_table_name) + "/" + OrganiserToUpdate.Id);
                HashMap organiserMap = Organiser.ToHashMap(OrganiserToUpdate);
                df.SetValueAsync(organiserMap);

                DatabaseReference df1 = FirebaseCommunicator.GetDatabase()
                    .Reference
                    .Child(Application.Context.Resources.GetString(Resource.String.locals_table_name) + "/" + OrganiserToUpdate.Local.Id);
                HashMap localMap = Local.ToHashMap(OrganiserToUpdate.Local);
                df1.SetValueAsync(localMap);
            }
            else if(Person is User)
            {
                User UserToUpdate = (User)Person;
                DatabaseReference df = FirebaseCommunicator.GetDatabase()
                    .Reference
                    .Child(Application.Context.Resources.GetString(Resource.String.users_table_name) + "/" + UserToUpdate.Id);
                HashMap userMap = User.ToHashMap(UserToUpdate);
                df.SetValueAsync(userMap);
            }
        }

        #endregion


    }
}