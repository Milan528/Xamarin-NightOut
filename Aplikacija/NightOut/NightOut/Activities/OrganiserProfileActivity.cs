using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupportFragment = Android.Support.V4.App.Fragment;

using NightOut.Fragments;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using NightOut.Presenter;
using System.Threading.Tasks;
using NightOut.OurView;
using NightOut.Model;

namespace NightOut.Activities
{
    [Activity(Label = "@string/app_name",Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/Icon")]
    public class OrganiserProfileActivity : Android.Support.V4.App.FragmentActivity, IOrganiserProfileView
    {

        #region Attributes
        private TextView _organiserName;
        private TextView _organiserEmail;
        private TextView _organiserRating;
        private TextView _organiserContact;
        private TextView _organiserCity;
        private TextView _organiserAddress;
        private TextView _organiserFoundationDate;
        private TextView _organiserProfileDescription;
        private TextView _organiserEditProfile;
        private TextView _editOrgProfile;
        private Button _organiserButtonFollowers;
        private Button _organiserButtonEvents;
        private OrganiserProfilePresenter _presenter;
        private ImageView _organiserProfileImage;

        private ProgressDialog _progress;
        #endregion

        #region Property

        public bool IsActive
        {
            get;
            set;
        }

        public OrganiserProfilePresenter OrganiserProfilePresenter
        {
            get => _presenter;
        }

        #endregion

        #region Override

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.organizator_profile_page);
            SetUpAttributes();
            SetUpEventListeners();
            _presenter = new OrganiserProfilePresenter(this);
            _presenter.SetUpOrganiser();
            ShowProgres();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            
            if(IsActive)
            {
                if (resultCode == Result.Ok)
                {
                    System.IO.Stream pictureStream = ContentResolver.OpenInputStream(data.Data);
                    Android.Graphics.Bitmap ImageBitmap = Android.Graphics.BitmapFactory.DecodeStream(pictureStream);
                    _organiserProfileImage.SetImageBitmap(ImageBitmap);
                    Task.Run(() => _presenter.PutImageInDatabase(ImageBitmap));
                }
            }
        }

        #endregion

        #region Methods

        private void SetUpAttributes()
        {
            _organiserName = FindViewById<TextView>(Resource.Id.organiserName);
            _organiserEmail = FindViewById<TextView>(Resource.Id.organiserProfileEmail);
            _organiserRating = FindViewById<TextView>(Resource.Id.organiserRating);
            _organiserContact = FindViewById<TextView>(Resource.Id.organiserProfileContact);
            _organiserCity = FindViewById<TextView>(Resource.Id.organiserProfileCity);
            _organiserAddress = FindViewById<TextView>(Resource.Id.organiserProfileAddress);
            _organiserFoundationDate = FindViewById<TextView>(Resource.Id.organiserProfileFoundationDate);
            _organiserProfileDescription = FindViewById<TextView>(Resource.Id.organiserProfileDescription);
            _organiserEditProfile = FindViewById<TextView>(Resource.Id.organiserEditProfile);
            _organiserButtonFollowers = FindViewById<Button>(Resource.Id.organiserButtonFollowers);
            _organiserButtonEvents = FindViewById<Button>(Resource.Id.organiserButtonEvents);
            _organiserProfileImage = FindViewById<ImageView>(Resource.Id.OrganiserProfilePicture);
            IsActive = true;
        }

        private void SetUpEventListeners()
        {
            _editOrgProfile = FindViewById<TextView>(Resource.Id.organiserEditProfile);
            _organiserAddress.Click += OnOrganiserAddressClicked;
            _organiserCity.Click += OnOrganiserAddressClicked;
            _editOrgProfile.Click += OnEditProfileClicked;
            _organiserButtonEvents.Click += OnButtonEventsClicked;
            _organiserButtonFollowers.Click += OnButtonFolllowersClicked;
            _organiserProfileImage.Click += OnProfilePictureClicked;
        }

        public void UpdateInterface()
        {
            NightOut.Model.Organiser LogedOrganiser = _presenter.LogedInUser;
            _organiserName.Text = LogedOrganiser.UserName;
            _organiserEmail.Text = LogedOrganiser.Email;
            _organiserRating.Text = LogedOrganiser.Rating.ToString();
            _organiserContact.Text = LogedOrganiser.ContactPhone;
            _organiserCity.Text = "City : " + LogedOrganiser.Local.City;
            _organiserAddress.Text = LogedOrganiser.Local.Address;
            _organiserProfileDescription.Text = LogedOrganiser.Description;
            _organiserProfileDescription.Text = LogedOrganiser.Description;
            if (LogedOrganiser.Image != "")
                _organiserProfileImage.SetImageBitmap(ImagePresenter.DecodeImg(LogedOrganiser.Image));
            if (_presenter.LogedInUser.Local.FoundationDate.Year==9999)
                _organiserFoundationDate.Text = "Founded : ";
            else
                _organiserFoundationDate.Text = "Founded : " + LogedOrganiser.Local.FoundationDate.ToString(Resources.GetString(Resource.String.date_format));
            Task.Run(() => _presenter.GetRating(FillOutRating));
            StopProgres();
        }

        private void FillOutRating(double OrganiserRating)
        {
            _organiserRating.Text = OrganiserRating.ToString() + " / 5";
        }

        public void OpenFragment(SupportFragment newFragment)
        {
            IsActive = false;
            if (newFragment.IsVisible)
                return;
            var Transaction = SupportFragmentManager.BeginTransaction();
            Transaction.Replace(Resource.Id.organizatorProfileFragmentContainer, newFragment);
            Transaction.AddToBackStack(null);
            Transaction.Commit();
        }

        public void CommitUserChanges()
        {
            _presenter.SaveUserChanges();
            UpdateInterface();
        }

        public void CreateEvent(Event EventToBeCreated) => _presenter.CreateEvent(EventToBeCreated);

        public void UpdateEvent(Event EventToUpdate) => _presenter.UpdateEvent(EventToUpdate);

        public void OpenMaps(string Address)
        {
            Android.Net.Uri gmmIntentUri = Android.Net.Uri.Parse(Resources.GetString(Resource.String.maps_query)+Address);
            Intent mapIntent = new Intent(Intent.ActionView,gmmIntentUri);
            mapIntent.SetPackage(Resources.GetString(Resource.String.maps_package));
            if (mapIntent.ResolveActivity(PackageManager) != null)
            {
                StartActivity(mapIntent);
            }
        }

        public void ShowProgres()
        {
            _progress = new ProgressDialog(this);
            _progress.SetTitle("Loading...");
            _progress.SetMessage("Please wait.");
            _progress.SetCancelable(false);
            _progress.Show();
        }

        public void StopProgres()
        {
            _progress.Dismiss();
        }
        #endregion

        #region Listeners
        private void OnOrganiserAddressClicked(object sender, EventArgs e) 
            => OpenMaps(_presenter.LogedInUser.Local.City + "+" + _presenter.LogedInUser.Local.Address);

        private void OnButtonEventsClicked(object sender, EventArgs e)
        {
            OrganisersEventsFragment Events = new OrganisersEventsFragment();
            Bundle Data = new Bundle();
            Data.PutString(Resources.GetString(Resource.String.organiser_id_column), _presenter.LogedInUser.Id);
            Events.Arguments = Data;
            OpenFragment(Events);
        }

        private void OnEditProfileClicked(object sender, EventArgs e) => OpenFragment(new EditOrganiserProfileFragment());

        private async void OnProfilePictureClicked(object sender, EventArgs e)
        {
            string resault = await ImagePresenter.DisplayCustomDialog(this,
                Resources.GetString(Resource.String.profile_picture_title),
                Resources.GetString(Resource.String.profile_picture_question));
            if (resault == "No")
                return;
            Intent GalleryPage = new Intent();
            GalleryPage.SetType("image/*");
            GalleryPage.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(GalleryPage, Resources.GetString(Resource.String.gallery_profile_title)), 0);
        }

        private void OnButtonFolllowersClicked(object sender, EventArgs e)
        {
            OrganiserFollowersFragment Followers = new OrganiserFollowersFragment();
            Bundle Data = new Bundle();
            Data.PutString(Resources.GetString(Resource.String.organiser_id_column), _presenter.LogedInUser.Id);
            Followers.Arguments = Data;
            OpenFragment(Followers);
        }

        #endregion


    }
}