using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using NightOut.Activities;
using NightOut.Presenter;

namespace NightOut.OurView
{
    [Activity(Label = "@string/app_name", Theme = "@style/LoginTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/Icon")]
    public class LoginActivity : AppCompatActivity, IAttributeCollector, ILogInView
    {
        #region Attributes

        private EditText _email;
        private EditText _password;
        private Button _loginButton;
        private Button _regButton;
        private FirebasePresenter _firebasePresenter;
        #pragma warning disable CS0618 
        private ProgressDialog _progress;
        #pragma warning restore CS0618

        #endregion

        #region Overriden Methods

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.loginLayoutView);
            SetUpAttributes();
            SetUpListeners();
        }

        public override void OnBackPressed()
        {
            var activity = (Activity)this;
            activity.FinishAffinity();
        }

        #endregion

        #region Additional Methods

        public bool CheckAttributes()
        {
            if (!_email.Text.Contains("@"))
            {
                Toast.MakeText(Android.App.Application.Context, Resource.String.email_invalid_input_toast, ToastLength.Short).Show();
                return false;
            }
            if (_password.Text.Length < 8)
            {
                Toast.MakeText(Android.App.Application.Context, Resource.String.password_invalid_input_toast, ToastLength.Short).Show();
                return false;
            }
            return true;
        }

        private void SetUpListeners()
        {
            _regButton.Click += OnBtnRegisterClicked;
            _loginButton.Click += OnBtnLoginClicked;
        }

        public void SetUpAttributes()
        {
            _email = FindViewById<EditText>(Resource.Id.emailText);
            _password = FindViewById<EditText>(Resource.Id.passwordText);
            _loginButton = FindViewById<Button>(Resource.Id.loginBtn);
            _regButton = FindViewById<Button>(Resource.Id.registerBtn);
            _firebasePresenter = new FirebasePresenter(this);
            _firebasePresenter.InitializeLogin();
        }

        public void OnTaskCompleteLoginFailure(object sender, EventArgs e)
        {
            Toast.MakeText(Android.App.Application.Context, Resource.String.login_failed_toast, ToastLength.Short).Show();
            _progress.Dismiss();
        }

        public void OnTaskCompleteLoginSuccess(object sender, EventArgs e)
        {
            Model.EventListeners.AttributeListener Event = new Model.EventListeners.AttributeListener();
            Event.AttributesRetrived += OnAttributesAttrived;
            Event.AttributesNotRetrived += OnAttributesNotAttrived;
            Model.FirebaseCommunicator.GetDatabase().Reference
                .Child("users")
                .Child(Firebase.Auth.FirebaseAuth.Instance.CurrentUser.Uid)
                .AddListenerForSingleValueEvent(Event);
        }

        #endregion

        #region EventListeners

        private void OnBtnLoginClicked(object sender, EventArgs e)
        {
            //StartActivity(typeof(User_main_page_activity)); //ostavljeno radi testiranja novih stranica
            if (!CheckAttributes())
                return;
            ShowProgres();
            _firebasePresenter.LoginPerson(_email.Text, _password.Text);

        }

        private void OnBtnRegisterClicked(object sender, EventArgs e)
        {
            StartActivity(typeof(RegistrationActivity));
            OverridePendingTransition(Resource.Animation.slide_in, Resource.Animation.pop_slide_out);
        }

        private void OnAttributesNotAttrived(object sender, Model.EventListeners.AttributeListener.AttributeDataEventArgs e)
        {
            FinishAffinity();
            StartActivity(typeof(OrganiserProfileActivity));
        }

        private void OnAttributesAttrived(object sender, Model.EventListeners.AttributeListener.AttributeDataEventArgs e)
        {
            FinishAffinity();
            StartActivity(typeof(UserMainPageActivity));
        }

        public void ShowProgres()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            _progress = new ProgressDialog(this);
#pragma warning restore CS0618 // Type or member is obsolete
            _progress.SetTitle("Loading...");
            _progress.SetMessage("Please wait.");
            _progress.SetCancelable(false);
            _progress.Show();
        }

        #endregion
    }
}