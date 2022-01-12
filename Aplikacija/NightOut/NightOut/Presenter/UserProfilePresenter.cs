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

namespace NightOut.Presenter
{
    public class UserProfilePresenter : ImagePresenter
    {

        #region Attributes

        private UserProfileActivity _parentActivity;
        private FirebaseAuth _firebaseAuth;
        private User _logedInUser;
        private FirebaseCommunicator _communicator;

        #endregion

        #region Properties

        public User LogedInUser
        {
            get
            {
                if (_logedInUser == null)
                {
                    SetUpUser();
                }
                return this._logedInUser;
            }
        }
        
        #endregion

        #region Constructor

        public UserProfilePresenter(UserProfileActivity ParentActivity)
        {
            _parentActivity = ParentActivity;
            _communicator = new FirebaseCommunicator(_parentActivity);
            _firebaseAuth = FirebaseAuth.Instance;
            _logedInUser = null;
        }

        #endregion

        #region Methodes

        public void SaveUserChanges() => _communicator.UpdatePasswordAndEmail(_logedInUser);

        public void SetUpUser()
        {
            FirebaseUser User = _firebaseAuth.CurrentUser;
            if (User != null || _logedInUser == null)
            {
                UserListener Event = new UserListener();
                Event.UserRetrived += (object sender, UserListener.UserDataEventArgs e) => {
                    this._logedInUser = e.Users.First();
                    _parentActivity.UpdateInterface();
                };
                DatabaseReference OurDatabase = FirebaseCommunicator.GetDatabase().Reference.Child("users").Child(User.Uid);
                OurDatabase.AddListenerForSingleValueEvent(Event);
            }
        }

        public void PutImageInDatabase(Android.Graphics.Bitmap Image)
        {
            LogedInUser.Image = Presenter.ImagePresenter.CompressImage(Image);
            Java.Util.HashMap userMap = User.ToHashMap(LogedInUser);
            
            NightOut.Model.FirebaseCommunicator.GetDatabase().Reference
                .Child(Application.Context.Resources.GetString(Resource.String.users_table_name))
                .Child(LogedInUser.Id)
                .SetValueAsync(userMap);
        }

        #endregion
    }
}