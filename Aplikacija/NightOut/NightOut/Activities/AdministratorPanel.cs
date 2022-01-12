using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupportFragment = Android.Support.V4.App.Fragment;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using NightOut.Model;
using NightOut.Model.EventListeners;
using NightOut.OurView;
using NightOut.Fragments;

namespace NightOut.Activities
{
    [Activity(Label = "@string/app_name",MainLauncher =false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,Icon ="@drawable/Icon")]
    public class AdministratorPanel : Android.Support.V4.App.FragmentActivity, IAdmin
    {
        private EditText _email;
        private Button _delete;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.administrator_panel);
            // Create your application here
            SetUpAttributes();
        }

        private void SetUpAttributes()
        {
            _email = FindViewById<EditText>(Resource.Id.administratorDeleteAddress);
            _delete = FindViewById<Button>(Resource.Id.administratorGoToPage);
            _delete.Click += OpenPage;
        }

        private void OpenPage(object sender, EventArgs e)
        {
            try
            {
                if (_email.Text != "")
                {
                    System.Threading.Tasks.Task.Run(() => { OpenUser(); });
                    System.Threading.Tasks.Task.Run(() => { OpenOrganiser(); });
                }
            }
            catch (Exception x)
            {

            }
        }


        private void OpenUser()
        {
            UserListListener userListener = new UserListListener();
            userListener.UsersRetrived += (object send, UserListListener.UsersDataEventArgs ev) =>
            {
                if(ev.Users.Count>0)
                    CreateUserPage(ev.Users);
            };
            FirebaseCommunicator.GetDatabase()
                .Reference.Child(Resources.GetString(Resource.String.users_table_name))
                .OrderByChild(Resources.GetString(Resource.String.user_email_column))
                .EqualTo(_email.Text)
                .AddValueEventListener(userListener);
        }

        private void OpenOrganiser()
        {
            OrganiserListListener organiserListener = new OrganiserListListener();
            organiserListener.OrganisersRetrived += (object sender, OrganiserListListener.OrganiserDataEventArgs e) =>
            {
                  if (e.Organisers.Count > 0)
                      CreateOrganiserPage(e.Organisers);
            };
            FirebaseCommunicator.GetDatabase()
                .Reference.Child(Resources.GetString(Resource.String.organisers_table_name))
                .OrderByChild(Resources.GetString(Resource.String.organiser_email_column))
                .EqualTo(_email.Text)
                .AddValueEventListener(organiserListener);
        }

        private void CreateUserPage(List<User> users)
        {
            AdministratorDeleteUserPage page = new AdministratorDeleteUserPage();
            page.DisplayedUser = users[0];
            OpenFragment(page);
        }

        private void CreateOrganiserPage(List<Organiser> Organisers)
        {
            Fragments.AdministratorDeleteOrganiserPage Page = new Fragments.AdministratorDeleteOrganiserPage();
            Page.DisplayedOrganiser = Organisers[0];
            OpenFragment(Page);
        }

        private void CreateOrganizerPage(List<Organiser> organisers)
        {

            AdministratorDeleteOrganiserPage page = new AdministratorDeleteOrganiserPage();
            page.DisplayedOrganiser = organisers[0];
            OpenFragment(page);

        }

        public void OpenFragment(SupportFragment newFragment)
        {
            if (newFragment.IsVisible)
                return;
            var Transaction = SupportFragmentManager.BeginTransaction();
            Transaction.Replace(Resource.Id.adminFragmentContainer, newFragment);
            Transaction.AddToBackStack(null);
            Transaction.Commit();
        }
    }



   
}