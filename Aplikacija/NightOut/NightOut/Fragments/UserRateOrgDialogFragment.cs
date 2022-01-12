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

namespace NightOut.Fragments
{
    class UserRateOrgDialogFragment : Android.Support.V4.App.DialogFragment
    {
        #region Attributes
        private RatingBar _ratingBar;
        private Button _btnConfirm;

        public event EventHandler<RatedDialogArgs> _rated;
        #endregion

        public string RatingKey { get; set; }
        
        public class RatedDialogArgs : EventArgs
        {
            public float UserRate { get; set; }
        }
   

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.user_rate_organiser, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            _ratingBar = View.FindViewById<RatingBar>(Resource.Id.userRateOrgBar);
            _ratingBar.NumStars = 5;
            if (RatingKey != null)
            {
                Model.EventListeners.RateListener RateRetriver = new Model.EventListeners.RateListener();
                RateRetriver.RateRetrived += OnRatingRetrived;
                Model.FirebaseCommunicator.GetDatabase().Reference
                    .Child(Resources.GetString(Resource.String.user_rate_organiser_table_name))
                    .Child(RatingKey)
                    .AddListenerForSingleValueEvent(RateRetriver);
            }
            
            _btnConfirm = View.FindViewById<Button>(Resource.Id.userRateOrgBtnConfirm);
            _btnConfirm.Click += OnBtnConfirmClicked;
        }

        private void OnRatingRetrived(object sender, Model.EventListeners.RateListener.RatingEventArgs e)
        {
            _ratingBar.Rating = (float)e.UserRate;
        }

        private void OnBtnConfirmClicked(object sender, EventArgs e)
        {
            _rated.Invoke(this, new RatedDialogArgs { UserRate = _ratingBar.Rating });
            this.Dismiss();
        }
        
    }
}