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

namespace NightOut.Fragments
{
    public class EditOrganiserProfileFragment : Android.Support.V4.App.Fragment
    {
        #region Attributes
        
        EditText _name;
        EditText _email;
        EditText _newPassword;
        EditText _bio;
        EditText _contact;
        EditText _address, _city;
        TextView _founded;
        ImageView _checkMark;
        IOrganiserProfileView _parentActivity;

        #endregion
       
        #region Override

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _parentActivity = (IOrganiserProfileView)this.Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.edit_organizator_profile_page, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            SetUpAttributes();
            SetUpListeners();
            FillOutAttributes();
        }

        #endregion

        #region Methods
       
        private void SetUpAttributes()
        {
            _name = View.FindViewById<EditText>(Resource.Id.editOrganizatorNameText);
            _email = View.FindViewById<EditText>(Resource.Id.editOrganizatorEmailText);
            _contact= View.FindViewById<EditText>(Resource.Id.editOrganizatorContactText);
            _newPassword = View.FindViewById<EditText>(Resource.Id.editOrganizatorNewPasswordText);

            _bio = View.FindViewById<EditText>(Resource.Id.editOrganizatorBioText);
            _city= View.FindViewById<EditText>(Resource.Id.editOrganizatorCityText);
            _address= View.FindViewById<EditText>(Resource.Id.editOrganizatorAddressText);
            _founded= View.FindViewById<TextView>(Resource.Id.editOrganizatorFounded);
            _checkMark= View.FindViewById<ImageView>(Resource.Id.organizatorCheckMark);
        }

        private void SetUpListeners()
        {
            _founded.Click += OnDateDisplayClicked;
            _checkMark.Click += SaveChanges;
        }

        private void FillOutAttributes()
        {
            NightOut.Model.Organiser Change = _parentActivity.OrganiserProfilePresenter.LogedInUser;
            NightOut.Model.Local ChangeLocal = Change.Local;
            _name.Text = Change.UserName;
            _email.Text = Change.Email;
            _bio.Text = Change.Description;
            _contact.Text = Change.ContactPhone;
            _address.Text = ChangeLocal.Address;
            _city.Text = ChangeLocal.City;
            _founded.Text = ChangeLocal.FoundationDate.ToString(Resources.GetString(Resource.String.date_format));
        }

        private void OnDateDisplayClicked(object sender, EventArgs e)
        {
            DatePickerDialog Dialog = new DatePickerDialog(Activity, Android.Resource.Style.ThemeHoloLightDialogMinWidth, OnDatePickerChosed, DateTime.Today.Year, DateTime.Today.Month - 1, DateTime.Today.Day);
            Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
            Dialog.Show();
        }

        private void OnDatePickerChosed(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            if (e.Date > DateTime.Today)
                Toast.MakeText(Activity, Resources.GetString(Resource.String.date_invalid_input_toast), ToastLength.Short).Show();
            else
            {
                DateTime Proba = e.Date.ToUniversalTime().AddDays(1);
                _founded.Text = Proba.ToString(Resources.GetString(Resource.String.date_format));
            }
        }

        private void SaveChanges(object sender, EventArgs e)
        {
            NightOut.Model.Organiser OrganiserToBeChanged = _parentActivity.OrganiserProfilePresenter.LogedInUser;
            if (_name.Text != OrganiserToBeChanged.UserName)
                _parentActivity.OrganiserProfilePresenter.LogedInUser.UserName = _name.Text;
            if (_email.Text != OrganiserToBeChanged.Email)
                _parentActivity.OrganiserProfilePresenter.LogedInUser.Email = _email.Text;
            if (_newPassword.Text != String.Empty)
            {
                if (_newPassword.Text.Length >= 8)
                    _parentActivity.OrganiserProfilePresenter.LogedInUser.Password = _newPassword.Text;
                else
                    Toast.MakeText(Android.App.Application.Context, Resources.GetString(Resource.String.password_invalid_input_toast), ToastLength.Short).Show();
            }
            if (OrganiserToBeChanged.Description != _bio.Text)
                _parentActivity.OrganiserProfilePresenter.LogedInUser.Description = _bio.Text;
            if (_contact.Text != OrganiserToBeChanged.ContactPhone)
                _parentActivity.OrganiserProfilePresenter.LogedInUser.ContactPhone = _contact.Text;
            if (_address.Text != OrganiserToBeChanged.Local.Address)
                _parentActivity.OrganiserProfilePresenter.LogedInUser.Local.Address = _address.Text;
            if (_city.Text != OrganiserToBeChanged.Local.City)
                _parentActivity.OrganiserProfilePresenter.LogedInUser.Local.City = _city.Text;
            if (_founded.Text != OrganiserToBeChanged.Local.FoundationDate.ToString(Resources.GetString(Resource.String.date_format)))
                _parentActivity.OrganiserProfilePresenter.LogedInUser.Local.FoundationDate = DateTime.ParseExact(_founded.Text, Resources.GetString(Resource.String.date_format), null);
            UpdateInterface();
        }

        public void UpdateInterface()
        {
            _parentActivity.CommitUserChanges();
            Activity.OnBackPressed();
        }

        public override void OnDestroy()
        {
            _parentActivity.IsActive = true;
            base.OnDestroy();
        }

        #endregion
    }
}
