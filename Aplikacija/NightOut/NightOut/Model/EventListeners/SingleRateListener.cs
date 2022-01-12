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
    class SingleRateListener : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<RatingEventArgs> RateRetrived;
        public string OrganiserId;

        public class RatingEventArgs : EventArgs
        {
            public string UserRateId { get; set; }
        }

        public void OnCancelled(DatabaseError error)
        {
            throw new NotImplementedException();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot == null)
            {
                RateRetrived.Invoke(this, new RatingEventArgs { UserRateId = null });
            }
            else
            {
                List<DataSnapshot> allData = snapshot.Children.ToEnumerable<DataSnapshot>().ToList<DataSnapshot>();
                foreach(DataSnapshot Data in allData)
                {
                    if(Data.Child(Application.Context.Resources.GetString(Resource.String.organiser_id_column)).Value.ToString() == OrganiserId)
                    {
                        RateRetrived.Invoke(this, new RatingEventArgs { UserRateId = Data.Key });
                        break;
                    }
                }
            }
        }
    }

    class RateListener : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<RatingEventArgs> RateRetrived;

        public class RatingEventArgs : EventArgs
        {
            public double UserRate { get; set; }
        }


        public void OnCancelled(DatabaseError error)
        {
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot == null)
            {
                RateRetrived.Invoke(this, new RatingEventArgs { UserRate = 0 });
            }
            else
            {
                double Rate =Convert.ToDouble(snapshot.Child(Application.Context.Resources.GetString(Resource.String.rating_column)).Value.ToString());
                RateRetrived.Invoke(this, new RatingEventArgs { UserRate = Rate });
            }
        }
    }
}