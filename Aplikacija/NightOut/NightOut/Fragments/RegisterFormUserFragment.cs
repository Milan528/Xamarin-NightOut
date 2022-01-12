using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using NightOut.OurView;

namespace NightOut.Fragments
{
    public class RegisterFormUserFragment : Android.Support.V4.App.Fragment,IAttributeCollector
    {
        #region Attributes
        private EditText _email;
        private EditText _password;
        private EditText _name;
        private EditText _surname;
        private Button _btnRegister;
        private Spinner _sex;
        private TextView _pickDate;
        private IRegistrationView _parentActivity;
        private TextView _goToLogin;
        #endregion

        #region Overriden methodes

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _parentActivity = (IRegistrationView)this.Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.registration_form_fragment_user, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            SetUpAttributes();
            SetUpListeners();
        }

        #endregion

        #region Additional methodes

        public void SetUpAttributes()
        {
            _email = View.FindViewById<EditText>(Resource.Id.RegisterUserEmail);
            _password = View.FindViewById<EditText>(Resource.Id.RegisterUserPassword);
            _name = View.FindViewById<EditText>(Resource.Id.RegisterUserName);
            _surname = View.FindViewById<EditText>(Resource.Id.RegisterUserSurname);
            _btnRegister = View.FindViewById<Button>(Resource.Id.RegisterUserBtnRegister);
            _sex = View.FindViewById<Spinner>(Resource.Id.RegisterUserSex);
            _goToLogin = View.FindViewById<TextView>(Resource.Id.goToLoginUser);
            _pickDate = View.FindViewById<TextView>(Resource.Id.RegisterUserDateDisplay);
        }

        public void SetUpListeners()
        {
            _btnRegister.Click += OnBtnRegisterClicked;
            _pickDate.Click += OnDateDisplayClicked;
            _goToLogin.Click += OnGoToLoginClicked;
        }

        public void ClearAttributes()
        {
            _email.Text = "";
            _password.Text = "";
            _name.Text = "";
            _surname.Text = "";
            _sex.SetSelection(0);
            _pickDate.Hint = "Date of birth";
        }
           
        public void RegistrationSuccess()
        {
            Toast.MakeText(Activity, Resource.String.success_registration, ToastLength.Short).Show();
            _parentActivity.GoToLoginPage();
        }

        public void RegistrationFailure() => Toast.MakeText(Activity, Resource.String.failed_registration, ToastLength.Short).Show();

        public bool CheckAttributes()
        {
            if (!_email.Text.ToString().Contains("@"))
            {
                Toast.MakeText(Activity, Resource.String.email_invalid_input_toast, ToastLength.Short).Show();
                return false;
            }
            else if (_password.Text.ToString().Length < 8)
            {
                Toast.MakeText(Activity, Resource.String.password_invalid_input_toast, ToastLength.Short).Show();
                return false;
            }
            else if (_name.Text.ToString() == String.Empty)
            {
                Toast.MakeText(Activity, Resource.String.name_invalid_input_toast, ToastLength.Short).Show();
                return false;
            }
            else if (_surname.Text.ToString() == String.Empty)
            {
                Toast.MakeText(Activity, Resource.String.surname_invalid_input_toast, ToastLength.Short).Show();
                return false;
            }
            else if (_pickDate.Text.ToString() ==String.Empty)
            {
                Toast.MakeText(Activity, Resource.String.date_invalid_input_toast, ToastLength.Short).Show();
                return false;
            }
            return true;
        }

        #endregion

        #region Listeners
        private void OnBtnRegisterClicked(object sender, EventArgs e)
        {
            if (CheckAttributes())
                _parentActivity.RegisterUser(_email.Text.ToString(), _password.Text.ToString(),
                    _name.Text.ToString(), _surname.Text.ToString(), _sex.SelectedItemPosition,
                    _pickDate.Text.ToString());
        }

        private void OnDateDisplayClicked(object sender, EventArgs e)
        {
            DatePickerDialog Dialog = new DatePickerDialog(Activity,
                Android.Resource.Style.ThemeHoloLightDialogMinWidth,
                OnDatePickerChosed,
                DateTime.Today.Year,
                DateTime.Today.Month - 1,
                DateTime.Today.Day);
            Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
            Dialog.Show();
        }

        private void OnDatePickerChosed(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            if (e.Date > DateTime.Today)
                Toast.MakeText(Activity, Resource.String.date_invalid_input_toast, ToastLength.Short).Show();
            else
            {
                DateTime Proba = e.Date.ToUniversalTime().AddDays(1);
                _pickDate.Text = Proba.ToString(Resources.GetString(Resource.String.date_format));
            }
        }

        private void OnGoToLoginClicked(object sender, EventArgs e) => _parentActivity.GoToLoginPage();

        #endregion
    }
}