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
using Firebase.Auth;
using Firebase.Database;
using NightOut.Model.EventListeners;

namespace NightOut.Model
{
    public static class LoggedUser
    {
        private static User _logedInUser;
       
        public static User LoggedInUser
        {
            get
            {
                return _logedInUser;
            }
        }

        public static void SetUpUser()
        {
            FirebaseUser user = FirebaseAuth.Instance.CurrentUser;
            if (_logedInUser == null)
            {
                UserListener Event = new UserListener();
                Event.UserRetrived += (object sender, UserListener.UserDataEventArgs e) =>
                {
                    _logedInUser = e.Users.First();
                };

                DatabaseReference OurDatabase = FirebaseCommunicator.GetDatabase().Reference.Child("users").Child(user.Uid);
                OurDatabase.AddListenerForSingleValueEvent(Event);
            }
        }

        public static void SetUpUser(Action<User> Callback)
        {
            FirebaseUser user = FirebaseAuth.Instance.CurrentUser;
            if (_logedInUser == null)
            {
                UserListener Event = new UserListener();
                Event.UserRetrived += (object sender, UserListener.UserDataEventArgs e) =>
                {
                    _logedInUser = e.Users.First();
                    Callback(_logedInUser);
                };

                DatabaseReference OurDatabase = FirebaseCommunicator.GetDatabase().Reference.Child(Application.Context.Resources.GetString(Resource.String.users_table_name)).Child(user.Uid);
                OurDatabase.AddListenerForSingleValueEvent(Event);
            }
            else
            {
                Callback(_logedInUser);
            }
        }


    }
}