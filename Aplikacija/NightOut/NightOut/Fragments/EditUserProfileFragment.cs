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
using NightOut.Activities;
using NightOut.OurView;
using NightOut.Model;

namespace NightOut.Fragments
{
    public class EditUserProfileFragment : Android.Support.V4.App.Fragment
    {

        #region Attributes

        EditText _name;
        EditText _surname;
        EditText _email;
        EditText _newPassword;
        EditText _bio;
        Spinner _sex;
        ImageView _checkMark;
        IUserProfileView _parentActivity;

        #endregion

        #region Override

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _parentActivity = (IUserProfileView)this.Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.edit_user_profile_page, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            SetUpAttributes();
            FillOutAttributes();
        }
        #endregion

        #region Methodes

        private void SetUpAttributes()
        {
            _name = View.FindViewById<EditText>(Resource.Id.editUserNameText);
            _surname = View.FindViewById<EditText>(Resource.Id.editUserSurnameText);
            _email = View.FindViewById<EditText>(Resource.Id.editUserEmailText);
            _newPassword = View.FindViewById<EditText>(Resource.Id.editUserNewPasswordText);
            _sex= View.FindViewById<Spinner>(Resource.Id.editUserSex);
            _bio = View.FindViewById<EditText>(Resource.Id.editUserBioText);

            _checkMark = View.FindViewById<ImageView>(Resource.Id.userProfileCheckMark);
            _checkMark.Click += SaveChanges;
        }

        private void FillOutAttributes()
        {
            User Change = _parentActivity.UserProfilePresenter.LogedInUser;
            _name.Text = Change.FirstName.ToString();
            _surname.Text = Change.Surname.ToString();
            _email.Text = Change.Email.ToString();
            if(Change.Description !=String.Empty)
                _bio.Text = _parentActivity.UserProfilePresenter.LogedInUser.Description.ToString();
            if (Change.Sex == Sex.Male)
                _sex.SetSelection(0);
            else
                _sex.SetSelection(1);
        }

        private void SaveChanges(object sender, EventArgs e)
        {
            User UserToBeChanged = _parentActivity.UserProfilePresenter.LogedInUser;
            if (_name.Text != UserToBeChanged.FirstName)
                _parentActivity.UserProfilePresenter.LogedInUser.FirstName = _name.Text;
            if(_surname.Text != UserToBeChanged.Surname)
                _parentActivity.UserProfilePresenter.LogedInUser.Surname = _surname.Text;
            if(_email.Text != UserToBeChanged.Email)
                _parentActivity.UserProfilePresenter.LogedInUser.Email = _email.Text;
            if (_newPassword.Text != String.Empty)
            {
                if (_newPassword.Text.Length >= 8)
                    _parentActivity.UserProfilePresenter.LogedInUser.Password = _newPassword.Text;
                else
                    Toast.MakeText(Android.App.Application.Context,
                        Application.Context.Resources.GetString(Resource.String.password_invalid_input_toast), ToastLength.Short).Show();
            }
            if (UserToBeChanged.Sex.ToString() != _sex.SelectedItem.ToString())
            {
                if (_sex.SelectedItemId == 0)
                    _parentActivity.UserProfilePresenter.LogedInUser.Sex = Sex.Male;
                else
                    _parentActivity.UserProfilePresenter.LogedInUser.Sex= Sex.Female;
            }
            if (UserToBeChanged.Description != _bio.Text)
                _parentActivity.UserProfilePresenter.LogedInUser.Description = _bio.Text;
            UpdateInterface();
        }

        public void UpdateInterface()
        {
            Activity.OnBackPressed();
            _parentActivity.CommitUserChanges();
        }

        #endregion

    }
}