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
using Android.Gms.Tasks;
using Firebase.Database;

namespace NightOut.Model.EventListeners
{
    public class DeleteOrganiserListener : Java.Lang.Object, IOnCompleteListener
    {
        public Organiser Organiser { get; set; }
        public void OnComplete(Task task)
        {
            Model.FirebaseCommunicator
                .GetDatabase()
                .Reference
                .Child(Application.Context.Resources.GetString(Resource.String.locals_table_name))
                .Child(Organiser.Local.Id)
                .RemoveValue()
                .AddOnCompleteListener(new DeleteAllOrganisersOcurances { Organiser = Organiser});
        }
    }

    public class DeleteAllOrganisersOcurances : Java.Lang.Object, IOnCompleteListener
    {
        public Organiser Organiser { get; set; }
        public void OnComplete(Task task)
        {
            Model.FirebaseCommunicator
                .GetDatabase()
                .Reference
                .Child(Application.Context.Resources.GetString(Resource.String.events_table_name))
                .OrderByChild(Application.Context.Resources.GetString(Resource.String.organiser_id_column))
                .EqualTo(Organiser.Id)
                .AddListenerForSingleValueEvent(new DeleteAllOrganiserEvents { Organiser = Organiser });

            Model.FirebaseCommunicator
                .GetDatabase()
                .Reference
                .Child(Application.Context.Resources.GetString(Resource.String.user_followed_organisers_table_name))
                .OrderByChild(Application.Context.Resources.GetString(Resource.String.organiser_id_column))
                .EqualTo(Organiser.Id)
                .AddListenerForSingleValueEvent(new DeleteOrganiserFollowings { Organiser = Organiser });

            Model.FirebaseCommunicator
                .GetDatabase()
                .Reference
                .Child(Application.Context.Resources.GetString(Resource.String.user_rate_organiser_table_name))
                .OrderByChild(Application.Context.Resources.GetString(Resource.String.organiser_id_column))
                .EqualTo(Organiser.Id)
                .AddListenerForSingleValueEvent(new DeleteOrganiserRatings { Organiser = Organiser });
        }
    }

    public class DeleteAllOrganiserEvents : Java.Lang.Object, Firebase.Database.IValueEventListener
    {
        public Organiser Organiser { get; set; }
        public void OnCancelled(DatabaseError error)
        {
            throw new NotImplementedException();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value == null)
                return;
            DataSnapshot[] AllEventsData = snapshot.Children.ToEnumerable<DataSnapshot>().ToArray<DataSnapshot>();
            foreach (DataSnapshot SingleEventData in AllEventsData)
            {
                FirebaseCommunicator.GetDatabase().Reference
                    .Child(Application.Context.Resources.GetString(Resource.String.user_followed_events_table_name))
                    .OrderByChild(Application.Context.Resources.GetString(Resource.String.event_id_column))
                    .EqualTo(SingleEventData.Key)
                    .AddValueEventListener(new DeleteAllCascadeEvents { Organiser = Organiser });

                FirebaseCommunicator.GetDatabase().Reference
                    .Child(Application.Context.Resources.GetString(Resource.String.events_table_name))
                    .Child(SingleEventData.Key)
                    .RemoveValueAsync();
            }
        }
    }

    public class DeleteAllCascadeEvents : Java.Lang.Object, Firebase.Database.IValueEventListener
    {
        public Organiser Organiser { get; set; }
        public void OnCancelled(DatabaseError error)
        {
            throw new NotImplementedException();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value == null)
                return;
            DataSnapshot[] AllEventsData = snapshot.Children.ToEnumerable<DataSnapshot>().ToArray<DataSnapshot>();
            foreach (DataSnapshot SingleEventData in AllEventsData)
            {
                FirebaseCommunicator.GetDatabase().Reference
                    .Child(Application.Context.Resources.GetString(Resource.String.user_followed_events_table_name))
                    .Child(SingleEventData.Key)
                    .RemoveValueAsync();
            }
        }
    }

    public class DeleteOrganiserFollowings : Java.Lang.Object, Firebase.Database.IValueEventListener
    {
        public Organiser Organiser { get; set; }
        public void OnCancelled(DatabaseError error)
        {
            throw new NotImplementedException();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value == null)
                return;
            DataSnapshot[] AllFollowingData = snapshot.Children.ToEnumerable<DataSnapshot>().ToArray<DataSnapshot>();
            foreach (DataSnapshot SingleFollowingData in AllFollowingData)
            {
                FirebaseCommunicator.GetDatabase().Reference
                    .Child(Application.Context.Resources.GetString(Resource.String.user_followed_organisers_table_name))
                    .Child(SingleFollowingData.Key)
                    .RemoveValueAsync();
            }
        }
    }

    public class DeleteOrganiserRatings : Java.Lang.Object, Firebase.Database.IValueEventListener
    {
        public Organiser Organiser { get; set; }
        public void OnCancelled(DatabaseError error)
        {
            throw new NotImplementedException();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value == null)
                return;
            DataSnapshot[] AllOrganiserRatings = snapshot.Children.ToEnumerable<DataSnapshot>().ToArray<DataSnapshot>();
            foreach (DataSnapshot SingleRatingData in AllOrganiserRatings)
            {
                FirebaseCommunicator.GetDatabase().Reference
                    .Child(Application.Context.Resources.GetString(Resource.String.user_rate_organiser_table_name))
                    .Child(SingleRatingData.Key)
                    .RemoveValueAsync();
            }
        }
    }

}