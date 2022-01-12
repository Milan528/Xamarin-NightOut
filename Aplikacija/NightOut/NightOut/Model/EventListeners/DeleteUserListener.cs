using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;

namespace NightOut.Model.EventListeners
{
    public class DeleteUserListener : Java.Lang.Object, IOnCompleteListener
    {
        public string UserId { get; set; }
    
        public void OnComplete(Task task)
        {
            Model.FirebaseCommunicator
                .GetDatabase()
                .Reference
                .Child(Application.Context.Resources.GetString(Resource.String.user_followed_events_table_name))
                .OrderByChild(Application.Context.Resources.GetString(Resource.String.user_id_column))
                .EqualTo(UserId)
                .AddListenerForSingleValueEvent(new DeleteUserFollowings { UserId = UserId });

            Model.FirebaseCommunicator
                .GetDatabase()
                .Reference
                .Child(Application.Context.Resources.GetString(Resource.String.user_rate_organiser_table_name))
                .OrderByChild(Application.Context.Resources.GetString(Resource.String.user_id_column))
                .EqualTo(UserId)
                .AddListenerForSingleValueEvent(new DeleteUserRatings { UserId = UserId });

            Model.FirebaseCommunicator
                .GetDatabase()
                .Reference
                .Child(Application.Context.Resources.GetString(Resource.String.user_followed_organisers_table_name))
                .OrderByChild(Application.Context.Resources.GetString(Resource.String.user_id_column))
                .EqualTo(UserId)
                .AddListenerForSingleValueEvent(new DeleteUserFollowedOrganisers { UserId = UserId });
        }
    }

    public class DeleteUserFollowings: Java.Lang.Object, IValueEventListener
    {
        public string UserId
        {
            get;set;
        }

        public void OnCancelled(DatabaseError error)
        {
            throw new NotImplementedException();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot == null)
                return;
            DataSnapshot[] AllFollowingsData = snapshot.Children.ToEnumerable<DataSnapshot>().ToArray<DataSnapshot>();
            foreach (DataSnapshot SingleFollowingData in AllFollowingsData)
            {
                FirebaseCommunicator.GetDatabase().Reference
                    .Child(Application.Context.Resources.GetString(Resource.String.user_followed_events_table_name))
                        .Child(SingleFollowingData.Key)
                        .RemoveValueAsync();
            }
        }
    }

    public class DeleteUserRatings : Java.Lang.Object, IValueEventListener
    {
        public string UserId
        {
            get; set;
        }

        public void OnCancelled(DatabaseError error)
        {
            throw new NotImplementedException();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot == null)
                return;
            DataSnapshot[] AllRatingData = snapshot.Children.ToEnumerable<DataSnapshot>().ToArray<DataSnapshot>();
            foreach (DataSnapshot SingleRatingData in AllRatingData)
            {
                FirebaseCommunicator.GetDatabase().Reference
                    .Child(Application.Context.Resources.GetString(Resource.String.user_rate_organiser_table_name))
                        .Child(SingleRatingData.Key)
                        .RemoveValueAsync();
            }
        }
    }

    public class DeleteUserFollowedOrganisers : Java.Lang.Object, IValueEventListener
    {
        public string UserId
        {
            get; set;
        }

        public void OnCancelled(DatabaseError error)
        {
            throw new NotImplementedException();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot == null)
                return;
            DataSnapshot[] AllFollowingData = snapshot.Children.ToEnumerable<DataSnapshot>().ToArray<DataSnapshot>();
            foreach (DataSnapshot SingleFollowigData in AllFollowingData)
            {
                FirebaseCommunicator.GetDatabase().Reference
                    .Child(Application.Context.Resources.GetString(Resource.String.user_followed_organisers_table_name))
                        .Child(SingleFollowigData.Key)
                        .RemoveValueAsync();
            }
        }
    }
}