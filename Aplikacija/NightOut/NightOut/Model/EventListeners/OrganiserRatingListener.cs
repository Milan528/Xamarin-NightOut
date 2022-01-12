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
    class OrganiserRatingListener : Java.Lang.Object, Firebase.Database.IValueEventListener
    {
        public event EventHandler<RatingArgs> RatingRetrived;

        public class RatingArgs:EventArgs
        {
            public double Rating { get; set; }
        }

        public void OnCancelled(DatabaseError error)
        { }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if(snapshot.Value == null)
            {
                RatingRetrived.Invoke(this, new RatingArgs { Rating = 0 });
                return;
            }
            double Rating = 0;
            DataSnapshot[] allData = snapshot.Children.ToEnumerable<DataSnapshot>().ToArray<DataSnapshot>();
            foreach(DataSnapshot SingleRating in allData)
                Rating += Convert.ToDouble(SingleRating.Child(Application.Context.GetString(Resource.String.rating_column)).Value.ToString());
            Rating /= allData.Length;
            RatingRetrived.Invoke(this, new RatingArgs { Rating = Rating });
        }
    }
}