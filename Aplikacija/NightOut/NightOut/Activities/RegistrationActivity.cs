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
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using SupportFragment = Android.Support.V4.App.Fragment;
using NightOut.OurView;
using NightOut.Fragments;
using Firebase.Auth;
using Firebase.Database;
using Firebase;
using NightOut.Model;
using Android.Gms.Tasks;
using Java.Lang;
using Java.Util;
using NightOut.Presenter;

namespace NightOut
{
    [Activity(Label = "@string/app_name", Theme = "@style/LoginTheme",MainLauncher =false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        Icon = "@drawable/Icon")]
    
    public class RegistrationActivity : FragmentActivity,IRegistrationView
    {

        #region Atributes

        private SupportFragment _currentFragment;
        private Stack<SupportFragment> _stackFragment;
        private RegistrationChoiceFragment _choiceFragment;
        private RegisterFormUserFragment _userFormFragment;
        private RegisterFormOrganFragment _organFormFragment;
        private FirebasePresenter _firebasePresenter;

        #endregion

        #region Overriden Methods
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.registration_main_page);

            SetUpAttributes();
            SetUpFragments();
        }

        public override void OnBackPressed()
        {
            if(SupportFragmentManager.BackStackEntryCount >0)
            {
                SupportFragmentManager.PopBackStack();
                _currentFragment = _stackFragment.Pop();
            }
            else
                base.OnBackPressed();
            _organFormFragment.ClearAttributes();
            _userFormFragment.ClearAttributes();
        }

        #endregion

        #region Additional Methodes

        private void SetUpAttributes()
        {
            _choiceFragment = new RegistrationChoiceFragment();
            _userFormFragment = new RegisterFormUserFragment();
            _organFormFragment = new RegisterFormOrganFragment();
            _stackFragment = new Stack<SupportFragment>();
            _firebasePresenter = new FirebasePresenter(this);
            _firebasePresenter.InitializeRegistration();
        }

        public void SetUpFragments()
        {
            var Transaction = SupportFragmentManager.BeginTransaction();
            Transaction.Add(Resource.Id.RegistrationFragmentContainer, _organFormFragment, Resources.GetString(Resource.String.registration_form_fragment_tag));
            Transaction.Hide(_organFormFragment);
            Transaction.Add(Resource.Id.RegistrationFragmentContainer, _userFormFragment, Resources.GetString(Resource.String.registration_form_fragment_tag));
            Transaction.Hide(_userFormFragment);
            Transaction.Add(Resource.Id.RegistrationFragmentContainer, _choiceFragment, Resources.GetString(Resource.String.registration_choice_fragment_tag));
            _currentFragment = _choiceFragment;
            Transaction.Commit();
        }

        public void OpenFragment(SupportFragment newFragment)
        {
            if (newFragment.IsVisible)
                return;
            var Transaction = SupportFragmentManager.BeginTransaction();
            Transaction.SetCustomAnimations(Resource.Animation.slide_in, Resource.Animation.pop_slide_out,Resource.Animation.pop_slide_in, Resource.Animation.slide_out);
            Transaction.Hide(_currentFragment);
            Transaction.Show(newFragment);
            Transaction.AddToBackStack(null);
            Transaction.Commit();
            _stackFragment.Push(_currentFragment);
            _currentFragment = newFragment;
        }

        public void OpenFormFragment(Type FormType)
        {
            if (_currentFragment.GetType() == FormType
                || _currentFragment.GetType() == typeof(RegisterFormUserFragment) 
                || _currentFragment.GetType() == typeof(RegisterFormOrganFragment))
                return;
            if (FormType == typeof(RegisterFormOrganFragment))
            {
                OpenFragment(_organFormFragment);
                _organFormFragment.ClearAttributes();
            }
            else
            {
                OpenFragment(_userFormFragment);
                _userFormFragment.ClearAttributes();
            }
        }
 
        public void RegisterOrganizator(string email, string password, string name, string city, string address)
        {
            _firebasePresenter.RegisterOrganizator(email, password, name, city, address);
        }

        public void RegisterUser(string email, string password, string name, string surname , int sex,string date)
        {
            _firebasePresenter.RegisterUser(email, password, name, surname, sex, date);
        }

        public void GoToLoginPage() => Finish();

        #endregion

        #region Event Listeners
        public void OnOrganizatorRegistrationFailed(object sender, EventArgs e) => _organFormFragment.RegistrationFailure();

        public void OnUserRegistrationFailed(object sender, EventArgs e) => _userFormFragment.RegistrationFailure();

        public void OnOrganizatorRegistrationSucceed(object sender, EventArgs e) => _organFormFragment.RegistrationSuccess();

        public void OnUserRegistrationSucceed(object sender, EventArgs e) => _userFormFragment.RegistrationSuccess();
        #endregion

    }


}