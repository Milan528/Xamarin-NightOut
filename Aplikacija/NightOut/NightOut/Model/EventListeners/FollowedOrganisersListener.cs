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
    class FollowedOrganisersListener : Java.Lang.Object, IValueEventListener
    {
        private List<NightOut.Model.Organiser> Organisers = new List<NightOut.Model.Organiser>();
        private int NumOfOrganisers = 0;
        public event EventHandler<OrganisersDataArgs> OrganisersRetrived;

        public class OrganisersDataArgs : EventArgs
        {
            public List<NightOut.Model.Organiser> Organisers { get; set; }
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                var AllData = snapshot.Children.ToEnumerable<DataSnapshot>().ToArray<DataSnapshot>();
                NumOfOrganisers = AllData.Count();
                foreach (DataSnapshot OrganiserData in AllData)
                {
                    OrganiserListener OrganiserRetriver = new OrganiserListener();
                    OrganiserRetriver.OrganiserRetrived += OnOrganiserRetrived;
                    NightOut.Model.FirebaseCommunicator.GetDatabase()
                        .Reference.Child(Application.Context.Resources.GetString(Resource.String.organisers_table_name))
                        .Child(OrganiserData.Child(Application.Context.Resources.GetString(Resource.String.organiser_id_column)).Value.ToString())
                        .AddListenerForSingleValueEvent(OrganiserRetriver);
                }
            }
        }

        private void OnOrganiserRetrived(object sender, OrganiserListener.OrganiserDataEventArgs e)
        {
            Organisers.Add(e.Organiser);
            if (Organisers.Count == NumOfOrganisers)
                OrganisersRetrived.Invoke(this, new OrganisersDataArgs { Organisers = this.Organisers });
        }

        public void OnCancelled(DatabaseError error)
        {}
    }
}