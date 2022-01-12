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
using NightOut.OurView;
using Android.Support.Design.Widget;



namespace NightOut.Fragments
{
    public class RegisterFormOrganFragment : Android.Support.V4.App.Fragment,IAttributeCollector
    {
        #region Attributes
        private EditText _email;
        private EditText _password;
        private EditText _name;
        private EditText _city;
        private EditText _address;
        private Button _btnRegister;
        private IRegistrationView _parentActivity;
        private TextView _goToLogin;
        #endregion

        #region Overriden Methodes
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _parentActivity = (IRegistrationView)this.Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.registration_form_fragment_organ, container, false);
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

        public void SetUpListeners()
        {
            _btnRegister.Click += OnBtnRegisterClicked;
            _goToLogin.Click += OnGoToLoginClicked;
        }

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
            else if (_city.Text.ToString() == String.Empty)
            {
                Toast.MakeText(Activity, Resource.String.city_invalid_input_toast, ToastLength.Short).Show();
                return false;
            }
            else if (_address.Text.ToString() == String.Empty)
            {
                Toast.MakeText(Activity, Resource.String.address_invalid_input_toast, ToastLength.Short).Show();
                return false;
            }
            return true;
        }

        public void SetUpAttributes()
        {
            _email = View.FindViewById<EditText>(Resource.Id.RegisterOrganEmail);
            _password = View.FindViewById<EditText>(Resource.Id.RegisterOrganPassword);
            _name = View.FindViewById<EditText>(Resource.Id.RegisterOrganName);
            _btnRegister = View.FindViewById<Button>(Resource.Id.RegisterOrganBtnRegister);
            _city = View.FindViewById<EditText>(Resource.Id.RegisterOrganCity);
            _address = View.FindViewById<EditText>(Resource.Id.RegisterOrganAdress);
            _goToLogin = View.FindViewById<TextView>(Resource.Id.goToLogin);
        }
        
        public void ClearAttributes()
        {
            _email.Text=String.Empty;
            _password.Text = String.Empty;
            _name.Text = String.Empty;
            _city.Text = String.Empty;
            _address.Text = String.Empty;
        }       

        public void RegistrationSuccess()
        {
            Toast.MakeText(Activity, Resource.String.success_registration, ToastLength.Short).Show();
            System.Threading.Thread.Sleep(2500);
            _parentActivity.GoToLoginPage();
        }

        public void RegistrationFailure() => Toast.MakeText(Activity, Resource.String.failed_registration, ToastLength.Short).Show();

        #endregion

        #region Listeners

        private void OnGoToLoginClicked(object sender, EventArgs e)
        {
            _parentActivity.GoToLoginPage();
        }

        private void OnBtnRegisterClicked(object sender, EventArgs e)
        {
            if (CheckAttributes())
                _parentActivity.RegisterOrganizator(_email.Text.ToString(),
                     _password.Text.ToString(), _name.Text.ToString(), _city.Text.ToString(),
                     _address.Text.ToString());
        }

        #endregion
    }
}