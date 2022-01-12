using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using NightOut.Model;
using NightOut.OurView;
using NightOut.Presenter;

namespace NightOut.Activities
{
    [Activity(Label = "string/app_name", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/Icon")]
    public class AdministratorDeleteUserPage : Android.Support.V4.App.Fragment
    {
        private User _displayedUser;
        private IAdmin _parent;
        private TextView name;
        private TextView emailAddress;
        private TextView date;
        private TextView sex;
        private TextView description;
        private Button deleteUser;
        private ImageView _userImage;

        #region Ovverides
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _parent = (IAdmin)this.Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.admin_delete_user_page, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            SetUpAttributes();
        }

        private void SetUpAttributes()
        {
            _userImage= View.FindViewById<ImageView>(Resource.Id.adminUserProfilePicture);
            _userImage.SetImageBitmap(ImagePresenter.DecodeImg(_displayedUser.Image));
            name = View.FindViewById<TextView>(Resource.Id.adminUserNameAndSurname);
            name.Text = _displayedUser.FullName;

            emailAddress = View.FindViewById<TextView>(Resource.Id.adminUserProfileEmail);
            emailAddress.Text = _displayedUser.Email;

            date = View.FindViewById<TextView>(Resource.Id.adminUserProfileDateOfBirth);
            date.Text = _displayedUser.DateOfBirth.ToString();

            sex = View.FindViewById<TextView>(Resource.Id.adminUserProfileSex);
            sex.Text = _displayedUser.Sex.ToString();

            description = View.FindViewById<TextView>(Resource.Id.adminUserProfileDescription);
            description.Text = _displayedUser.Description;

            deleteUser = View.FindViewById<Button>(Resource.Id.adminDeleteUserProfile);
            deleteUser.Click += DeleteUser_Click;
        }

        private void DeleteUser_Click(object sender, EventArgs e)
        {
            string key = _displayedUser.Id;
            FirebaseCommunicator.GetDatabase().Reference.Child("users").Child(key).RemoveValue().AddOnCompleteListener(new Model.EventListeners.DeleteUserListener { UserId = _displayedUser.Id });
            Activity.OnBackPressed();
        }
        #endregion


        public User DisplayedUser {
            get => _displayedUser;
            set => _displayedUser = value;
        }
    }
}