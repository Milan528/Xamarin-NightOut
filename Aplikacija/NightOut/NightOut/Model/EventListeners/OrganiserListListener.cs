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
    class OrganiserListListener : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<OrganiserDataEventArgs> OrganisersRetrived;
        private List<Organiser> OrganisersList=new List<Organiser>();
        private int NumOfOrganisers=0;

        public class OrganiserDataEventArgs : EventArgs
        {
            public List<Organiser> Organisers { get; set; }
        }

        public void OnCancelled(DatabaseError error)
        {
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                DataSnapshot[] AllData = snapshot.Children.ToEnumerable<DataSnapshot>().ToArray<DataSnapshot>();
                NumOfOrganisers = AllData.Length;
                foreach (DataSnapshot SingleOrganiserData in AllData)
                {
                    ReadOrganiser(SingleOrganiserData);
                }
            }
        }

        private void ReadOrganiser(DataSnapshot OrganiserData)
        {
            Organiser ResultOrganiser = new Organiser(OrganiserData.Key)
            {
                Email = OrganiserData.Child(Application.Context.Resources.GetString(Resource.String.organiser_email_column)).Value.ToString(),
                UserName = OrganiserData.Child(Application.Context.Resources.GetString(Resource.String.organiser_name_column)).Value.ToString(),
                Rating = int.Parse(OrganiserData.Child(Application.Context.Resources.GetString(Resource.String.organiser_rating_column)).Value.ToString()),
                NumberOfVotes = int.Parse(OrganiserData.Child(Application.Context.Resources.GetString(Resource.String.organiser_number_of_votes_column)).Value.ToString()),
                ContactPhone = OrganiserData.Child(Application.Context.Resources.GetString(Resource.String.organiser_contact_column)).Value.ToString(),
                Description = OrganiserData.Child(Application.Context.Resources.GetString(Resource.String.organiser_description_column)).Value.ToString(),
                Image = OrganiserData.Child(Application.Context.Resources.GetString(Resource.String.organiser_profile_image_column)).Value.ToString()
            };
            ResultOrganiser.Password = null;
            string localId = OrganiserData.Child(Application.Context.Resources.GetString(Resource.String.local_id_column)).Value.ToString();
            LocalListener Event = new LocalListener();
            Event.LocalRetrived += (object sender, LocalListener.LocalDataEventArgs e) =>
            {
                ResultOrganiser.Local = e.Local;
                OrganisersList.Add(ResultOrganiser);
                if (OrganisersList.Count == NumOfOrganisers)
                    OrganisersRetrived.Invoke(this, new OrganiserDataEventArgs { Organisers = OrganisersList });
            };
            FirebaseCommunicator.GetDatabase()
                .Reference
                .Child(Application.Context.Resources.GetString(Resource.String.locals_table_name)).Child(localId)
                .AddValueEventListener(Event);
        }
    }
}