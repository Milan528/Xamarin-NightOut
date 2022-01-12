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
using Android.Graphics;

namespace NightOut.Activities
{
    [Activity(Label = "@string/app_name", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,Icon ="@drawable/Icon")]
    public class UserProfileActivity : Android.Support.V4.App.FragmentActivity, IUserProfileView
    {

        #region Attributes

        private TextView _userNameAndSurname;
        private TextView _userEmail;
        private TextView _userDateOfBirth;
        private TextView _userSex;
        private TextView _userDescription;
        private TextView _editUserProfile;
        private Button _followedEventsBtn;
        private Button _followedOrganisersBtn;
        private Refractored.Controls.CircleImageView _profilePicture;
        private UserProfilePresenter _presenter;

        #endregion

        #region Property

        public UserProfilePresenter UserProfilePresenter
        {
            get => _presenter;
        }

        #endregion

        #region Overriden methodes

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _presenter = new UserProfilePresenter(this);
            SetContentView(Resource.Layout.user_profile_page);
            SetUpAttributes();
            SetUpListeners();
            _presenter.SetUpUser();
        }

        #endregion

        #region Aditional Methods

        private void SetUpAttributes()
        {
            _userNameAndSurname = FindViewById<TextView>(Resource.Id.userNameAndSurname);
            _userEmail = FindViewById<TextView>(Resource.Id.userProfileEmail);
            _userDateOfBirth = FindViewById<TextView>(Resource.Id.userProfileDateOfBirth);
            _userSex = FindViewById<TextView>(Resource.Id.userProfileSex);
            _userDescription = FindViewById<TextView>(Resource.Id.UserProfileDescription);
            _profilePicture = FindViewById<Refractored.Controls.CircleImageView>(Resource.Id.userProfilePicture);
            _followedEventsBtn = FindViewById<Button>(Resource.Id.userButtonEvents);
            _followedOrganisersBtn = FindViewById<Button>(Resource.Id.userButtonOgranisers);
            _editUserProfile = FindViewById<TextView>(Resource.Id.userEditProfile);
        }

        private void SetUpListeners()
        {
            _profilePicture.Click += OnProfilePictureClicked;
            _followedEventsBtn.Click += ShowEvents;
            _followedOrganisersBtn.Click += OnFollowedOrganisersClicked;
            _editUserProfile.Click += OnEditProfileClicked;
        }

        private void ShowEvents(object sender, EventArgs e)
        {
            NightOut.Fragments.UserFollowedEventsFragment Fragment = new NightOut.Fragments.UserFollowedEventsFragment();
            Bundle Data = new Bundle();
            Data.PutString(Resources.GetString(Resource.String.user_id_column), _presenter.LogedInUser.Id);
            Fragment.Arguments = Data;
            OpenFragment(Fragment);
        }

        public void OpenFragment(SupportFragment newFragment)
        {
            if (newFragment.IsVisible)
                return;
            var Transaction = SupportFragmentManager.BeginTransaction();
            Transaction.Replace(Resource.Id.UserProfileFragmentContainer, newFragment);
            Transaction.AddToBackStack(null);
            Transaction.Commit();
        }

        public void UpdateInterface()
        {
            Model.User LoggedUser = _presenter.LogedInUser;
            _userNameAndSurname.Text = LoggedUser.FirstName + " " + LoggedUser.Surname;
            _userEmail.Text = "Email: " + LoggedUser.Email;
            _userSex.Text = "Sex: " + LoggedUser.Sex.ToString();
            _userDateOfBirth.Text = "Date: " + LoggedUser.DateOfBirth.ToString(Resources.GetString(Resource.String.date_format));
            _userDescription.Text = "" + LoggedUser.Description;
            if(LoggedUser.Image!="")
                _profilePicture.SetImageBitmap(ImagePresenter.DecodeImg(LoggedUser.Image));
        }

        public void CommitUserChanges()
        {
            _presenter.SaveUserChanges();
            UpdateInterface();
        }

        #endregion

        #region Listeners

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

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                System.IO.Stream pictureStream = ContentResolver.OpenInputStream(data.Data);
                Android.Graphics.Bitmap ImageBitmap = Android.Graphics.BitmapFactory.DecodeStream(pictureStream);
                _profilePicture.SetImageBitmap(ImageBitmap);
                Task.Run(() => _presenter.PutImageInDatabase(ImageBitmap));
            }
        }

        private void OnFollowedOrganisersClicked(object sender, EventArgs e)
        {
            NightOut.Fragments.UserFollowedOrganisersFragment Fragment = new NightOut.Fragments.UserFollowedOrganisersFragment();
            Bundle Data = new Bundle();
            Data.PutString(Resources.GetString(Resource.String.user_id_column), Model.LoggedUser.LoggedInUser.Id);
            Fragment.Arguments = Data;
            OpenFragment(Fragment);
        }

        private void OnEditProfileClicked(object sender, EventArgs e) => OpenFragment(new EditUserProfileFragment());

        #endregion
    }
}