using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace NightOut.Fragments
{
    public class UserViewOrganiserFragment : Android.Support.V4.App.Fragment
    {
        #region Attributes

        private TextView _organiserName;
        private TextView _organiserEmail;
        private TextView _organiserContact;
        private TextView _organiserCity;
        private TextView _organiserAddress;
        private TextView _organiserDateOfFundation;
        private TextView _organiserProfileDescription;
        private Button _userFollowOrganiserBtn;
        private ImageView _organiserRateStar;
        private TextView _organiserRating;
        private ImageView _organiserImage;

        private string _organiserId;
        private ProgressDialog _progress;
        #endregion

        #region Properties

        public string OrganiserId { set => _organiserId = value; }
        
        #endregion

        #region Overriden methodes
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Android.Views.View view = inflater.Inflate(Resource.Layout.user_view_organizator_profile, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            ShowProgress();
            SetUpAttributes();
            SetUpListeners();
            FillUpAttributes();
        }

        #endregion

        #region Methodes

        private void SetUpAttributes()
        {
            _organiserName = View.FindViewById<TextView>(Resource.Id.followOrganiserProfileName);
            _organiserCity = View.FindViewById<TextView>(Resource.Id.followOrganiserProfileCity);
            _organiserEmail = View.FindViewById<TextView>(Resource.Id.followOrganiserProfileEmail);
            _organiserContact = View.FindViewById<TextView>(Resource.Id.followOrganiserProfileContact);
            _organiserAddress = View.FindViewById<TextView>(Resource.Id.followOrganiserProfileAddress);
            _organiserDateOfFundation = View.FindViewById<TextView>(Resource.Id.followOrganiserProfileFoundationDate);
            _organiserProfileDescription = View.FindViewById<TextView>(Resource.Id.followOrganiserProfileDescription);
            _organiserRateStar = View.FindViewById<ImageView>(Resource.Id.organiserRatingStar);
            _organiserRating = View.FindViewById<TextView>(Resource.Id.UserOrganiserRatingText);
            _organiserImage = View.FindViewById<ImageView>(Resource.Id.UserOrganiserImage);
            SetUpButton();
        }

        private void SetUpListeners()
        {
            _userFollowOrganiserBtn.Click += OnFollowOrganiserClicked;
            _organiserAddress.Click += OnOrganiserAddressClicked;
            _organiserCity.Click += OnOrganiserAddressClicked;
            _organiserRateStar.Click += OnRatingStarClicked;
        }

        private void OnRatingStarClicked(object sender, EventArgs e)
        {
            var transaction=FragmentManager.BeginTransaction();
            UserRateOrgDialogFragment dialog = new UserRateOrgDialogFragment();
            if (((ImageView)sender).Tag != null)
            {
                dialog.RatingKey = (string)(((ImageView)sender).Tag);
            }
            dialog._rated += OnUserRatedConfirmedClicked;
            dialog.Show(transaction, null);
        }

        private void OnUserRatedConfirmedClicked(object sender, UserRateOrgDialogFragment.RatedDialogArgs e)
        {
            Java.Util.HashMap ratingMapRow = new Java.Util.HashMap();
            ratingMapRow.Put(Resources.GetString(Resource.String.user_id_column), NightOut.Model.LoggedUser.LoggedInUser.Id);
            ratingMapRow.Put(Resources.GetString(Resource.String.organiser_id_column), _organiserId);
            ratingMapRow.Put(Resources.GetString(Resource.String.rating_column), e.UserRate);

            if(_organiserRateStar.Tag!=null)
            {
                NightOut.Model.FirebaseCommunicator.GetDatabase().GetReference(Resources.GetString(Resource.String.user_rate_organiser_table_name))
                    .Child((string)_organiserRateStar.Tag)
                    .SetValue(ratingMapRow);
            }
            else
            {
                Firebase.Database.DatabaseReference newRowRatingReference
                = NightOut.Model.FirebaseCommunicator.GetDatabase().GetReference(Resources.GetString(Resource.String.user_rate_organiser_table_name)).Push();
                _organiserRateStar.Tag = newRowRatingReference.Key;
                newRowRatingReference.SetValue(ratingMapRow);
            }
        }

        private void FillUpAttributes()
        {
            if (_organiserId == null)
                return;
            NightOut.Model.EventListeners.OrganiserListener Listener = new Model.EventListeners.OrganiserListener();
            Listener.OrganiserRetrived += OnOrganiserRetrived;
            NightOut.Model.FirebaseCommunicator.GetDatabase().Reference
                .Child(Resources.GetString(Resource.String.organisers_table_name))
                .Child(_organiserId)
                .AddValueEventListener(Listener);

            System.Threading.Tasks.Task.Run(() =>
            {
                NightOut.Model.EventListeners.SingleRateListener SingleRListener = new NightOut.Model.EventListeners.SingleRateListener();
                SingleRListener.RateRetrived += OnRateRetrived;
                SingleRListener.OrganiserId = _organiserId;
                NightOut.Model.FirebaseCommunicator.GetDatabase().Reference
                    .Child(Resources.GetString(Resource.String.user_rate_organiser_table_name))
                    .OrderByChild(Resources.GetString(Resource.String.user_id_column))
                    .EqualTo(Model.LoggedUser.LoggedInUser.Id)
                    .AddListenerForSingleValueEvent(SingleRListener);
            });
            System.Threading.Tasks.Task.Run(() => GetRating());
            StopProgress();
        }

        private void GetRating()
        {
            Model.EventListeners.OrganiserRatingListener RatingRetriver = new Model.EventListeners.OrganiserRatingListener();
            RatingRetriver.RatingRetrived += OnRatingRetrived;
            Model.FirebaseCommunicator.GetDatabase().Reference
                .Child(Application.Context.Resources.GetString(Resource.String.user_rate_organiser_table_name))
                .OrderByChild(Application.Context.Resources.GetString(Resource.String.organiser_id_column))
                .EqualTo(_organiserId)
                .AddListenerForSingleValueEvent(RatingRetriver);
        }

        private void OnRatingRetrived(object sender, Model.EventListeners.OrganiserRatingListener.RatingArgs e)
        {
            _organiserRating.Text = e.Rating + " / 5";
        }

        private void OnRateRetrived(object sender, Model.EventListeners.SingleRateListener.RatingEventArgs e)
        {
            if (e.UserRateId == null)
            {
                _organiserRateStar.Tag = null;
            }
            else
            {
                _organiserRateStar.Tag = e.UserRateId;
            }
            
        }

        private void SetUpButton()
        {
            _userFollowOrganiserBtn = View.FindViewById<Button>(Resource.Id.userFollowOrganiser);
            _userFollowOrganiserBtn.Tag = null;
            NightOut.Model.EventListeners.FollowedOrganiserListener OrganisersListener
                = new NightOut.Model.EventListeners.FollowedOrganiserListener();
            OrganisersListener.FollowedOrganisersRetrived += OnFollowedOrganisersRetrived;
            NightOut.Model.FirebaseCommunicator.GetDatabase().Reference
                    .Child(Resources.GetString(Resource.String.user_followed_organisers_table_name))
                    .OrderByChild(Resources.GetString(Resource.String.user_id_column))
                    .EqualTo(NightOut.Model.LoggedUser.LoggedInUser.Id)
                    .AddListenerForSingleValueEvent(OrganisersListener);
        }

        private void CreateNewFollowConnection()
        {
            Java.Util.HashMap newRowMap = new Java.Util.HashMap();
            newRowMap.Put(Resources.GetString(Resource.String.user_id_column), NightOut.Model.LoggedUser.LoggedInUser.Id);
            newRowMap.Put(Resources.GetString(Resource.String.organiser_id_column), _organiserId);

            Firebase.Database.DatabaseReference newRowReference
                = NightOut.Model.FirebaseCommunicator.GetDatabase().GetReference(Resources.GetString(Resource.String.user_followed_organisers_table_name)).Push();

            _userFollowOrganiserBtn.Tag = newRowReference.Key;
            newRowReference.SetValue(newRowMap);
        }

        private void DeleteFollowConnection()
        {
            NightOut.Model.FirebaseCommunicator.GetDatabase().Reference
                    .Child(Resources.GetString(Resource.String.user_followed_organisers_table_name))
                    .Child((string)_userFollowOrganiserBtn.Tag).RemoveValue();
            _userFollowOrganiserBtn.Tag = null;
        }

        private void OpenMaps(string Address)
        {
            Android.Net.Uri AddressURI = Android.Net.Uri.Parse(Resources.GetString(Resource.String.maps_query) + Address);
            Intent mapIntent = new Intent(Intent.ActionView, AddressURI);
            mapIntent.SetPackage(Resources.GetString(Resource.String.maps_package));
            if (mapIntent.ResolveActivity(Activity.PackageManager) != null)
            {
                StartActivity(mapIntent);
            }
        }

        public Android.Graphics.Bitmap DecodeImg(string profilePictureString)
        {
            byte[] decodedByteArray = Android.Util.Base64.Decode(profilePictureString, Android.Util.Base64Flags.Default);
            return Android.Graphics.BitmapFactory.DecodeByteArray(decodedByteArray, 0, decodedByteArray.Length);
        }

        private void ShowProgress()
        {
            _progress = new ProgressDialog(Activity);
            _progress.SetTitle("Loading organiser");
            _progress.SetMessage("Please wait....");
            _progress.Show();
        }

        private void StopProgress()
        {
            _progress.Dismiss();
        }
        #endregion

        #region Listeners

        private void OnOrganiserRetrived(object sender, Model.EventListeners.OrganiserListener.OrganiserDataEventArgs e)
        {
            _organiserName.Text = e.Organiser.UserName;
            _organiserCity.Text = "City: " + e.Organiser.Local.City;
            _organiserEmail.Text = e.Organiser.Email;
            _organiserContact.Text = e.Organiser.ContactPhone;
            _organiserAddress.Text = e.Organiser.Local.Address;
            _organiserDateOfFundation.Text = "Founded: " + e.Organiser.Local.FoundationDate.ToString(Resources.GetString(Resource.String.date_format));
            _organiserProfileDescription.Text = e.Organiser.Description;
            _organiserImage.SetImageBitmap(DecodeImg(e.Organiser.Image));
        }

        private void OnFollowOrganiserClicked(object sender, EventArgs e)
        {
            if (_userFollowOrganiserBtn.Tag == null)
            {
                _userFollowOrganiserBtn.Text = "Unfollow organiser";
                CreateNewFollowConnection();
            }
            else
            {
                _userFollowOrganiserBtn.Text = "Follow";
                DeleteFollowConnection();

            }
        }

        private void OnOrganiserAddressClicked(object sender, EventArgs e) => OpenMaps(_organiserCity.Text.Substring(_organiserCity.Text.IndexOf(' ')) + "+" + _organiserAddress.Text);

        private void OnFollowedOrganisersRetrived(object sender, Model.EventListeners.FollowedOrganiserListener.FollowedOrganisersDataArgs e)
        {
            if (!e.FollowedOrganisers.ContainsKey(_organiserId))
                _userFollowOrganiserBtn.Text = "Follow";
            else
            {
                _userFollowOrganiserBtn.Text = "Unfollow organiser";
                e.FollowedOrganisers.TryGetValue(_organiserId, out string Key);
                _userFollowOrganiserBtn.Tag = Key;
            }
        }

        #endregion

    }
}