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
    public class OrganiserListener : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<OrganiserDataEventArgs> OrganiserRetrived;
        private Organiser ResultOrganiser;

        public class OrganiserDataEventArgs : EventArgs
        {
            public Organiser Organiser { get; set; }
        }

        public void OnCancelled(DatabaseError error)
        {
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                ResultOrganiser = new Organiser(snapshot.Key)
                {
                    Email = snapshot.Child(Application.Context.Resources.GetString(Resource.String.organiser_email_column)).Value.ToString(),
                    UserName = snapshot.Child(Application.Context.Resources.GetString(Resource.String.organiser_name_column)).Value.ToString(),
                    Rating = int.Parse(snapshot.Child(Application.Context.Resources.GetString(Resource.String.organiser_rating_column)).Value.ToString()),
                    NumberOfVotes = int.Parse(snapshot.Child(Application.Context.Resources.GetString(Resource.String.organiser_number_of_votes_column)).Value.ToString()),
                    ContactPhone = snapshot.Child(Application.Context.Resources.GetString(Resource.String.organiser_contact_column)).Value.ToString(),
                    Description = snapshot.Child(Application.Context.Resources.GetString(Resource.String.organiser_description_column)).Value.ToString(),
                    Image = snapshot.Child(Application.Context.Resources.GetString(Resource.String.organiser_profile_image_column)).Value.ToString()
                };
                ResultOrganiser.Password = null;
                string localId = snapshot.Child(Application.Context.Resources.GetString(Resource.String.local_id_column)).Value.ToString();
                LocalListener Event = new LocalListener();
                Event.LocalRetrived += OnLocalRetrived;
                FirebaseCommunicator.GetDatabase()
                    .Reference
                    .Child(Application.Context.Resources.GetString(Resource.String.locals_table_name)).Child(localId)
                    .AddValueEventListener(Event);
            }
        }

        private void OnLocalRetrived(object sender, LocalListener.LocalDataEventArgs e)
        {
            ResultOrganiser.Local = e.Local;
            OrganiserRetrived.Invoke(this, new OrganiserDataEventArgs { Organiser = ResultOrganiser });
        }
    }
}
 