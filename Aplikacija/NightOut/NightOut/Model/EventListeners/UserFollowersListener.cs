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
using Firebase.Database;

namespace NightOut.Model.EventListeners
{
    class UserFollowersListener : Java.Lang.Object, Firebase.Database.IValueEventListener
    {
        private List<User> Followers = new List<User>();
        private int NumOfFollowers = 0;
        public event EventHandler<FollowersDataArgs> FollowersRetrived;

        public class FollowersDataArgs : EventArgs
        {
            public List<User> Followers { get; set; }
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                var AllData = snapshot.Children.ToEnumerable<DataSnapshot>().ToArray<DataSnapshot>();
                NumOfFollowers = AllData.Count();
                foreach (DataSnapshot EventData in AllData)
                {
                    UserListener UserRetriver = new UserListener();
                    UserRetriver.UserRetrived += OnUserRetrived;
                    NightOut.Model.FirebaseCommunicator.GetDatabase()
                        .Reference.Child(Application.Context.Resources.GetString(Resource.String.users_table_name))
                        .Child(EventData.Child(Application.Context.Resources.GetString(Resource.String.user_id_column)).Value.ToString())
                        .AddListenerForSingleValueEvent(UserRetriver);
                }
            }
        }

        private void OnUserRetrived(object sender, UserListener.UserDataEventArgs e)
        {
            Followers.Add(e.Users.First());
            if (Followers.Count == NumOfFollowers)
                FollowersRetrived.Invoke(this, new FollowersDataArgs { Followers = this.Followers });
        }

        public void OnCancelled(DatabaseError error)
        {}
    }
}