using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupportFragment = Android.Support.V4.App.Fragment;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using NightOut.Model;
using NightOut.OurView;

namespace NightOut.Fragments
{
    public class AdministratorDeleteOrganizatorPage : Android.Support.V4.App.Fragment
    {
        private Organiser _displayedOrganiser;
        private IAdmin _admin;
        private TextView name;
        private TextView rating;
        private TextView email;
        private TextView contact;
        private TextView city;
        private TextView foundationDate;
        private TextView description;
        private Button viewEvents;
        private Button deleteOrganiser;
        private ImageView _profileImage;
        
        
        public Organiser DisplayedOrganizer { get=> _displayedOrganiser; set=> _displayedOrganiser = value; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _admin = (IAdmin)this.Activity;
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            View view = inflater.Inflate(Resource.Layout.admin_delete_organizer_page, container, false);
            return view;
        }
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            SetupAttributes();
            FillAttributes();
        }
        private void SetupAttributes()
        {
            _profileImage= View.FindViewById<ImageView>(Resource.Id.adminDeleteOrgImage);
            name = View.FindViewById<TextView>(Resource.Id.adminOrganiserName);
            rating = View.FindViewById<TextView>(Resource.Id.adminOrganiserRating);
            email= View.FindViewById<TextView>(Resource.Id.adminOrganiserProfileEmail);
            contact = View.FindViewById<TextView>(Resource.Id.adminFragmentContainer);
            city = View.FindViewById<TextView>(Resource.Id.adminOrganiserProfileCity);
            foundationDate = View.FindViewById<TextView>(Resource.Id.adminOrganiserProfileFoundationDate);
            description = View.FindViewById<TextView>(Resource.Id.adminOrganiserProfileDescription);

            viewEvents = View.FindViewById<Button>(Resource.Id.adminViewEvents);
            deleteOrganiser = View.FindViewById<Button>(Resource.Id.adminDeleteOrganizerProfile);

            viewEvents.Click += ViewEvents_Click;
            deleteOrganiser.Click += DeleteOrganiser_Click;
        }
        private void FillAttributes()
        {
            name.Text = _displayedOrganiser.UserName;
            rating.Text = _displayedOrganiser.Rating.ToString();
            email.Text = _displayedOrganiser.Email;
            contact.Text = _displayedOrganiser.ContactPhone;
            city.Text = _displayedOrganiser.Local.City;
            foundationDate.Text = _displayedOrganiser.Local.FoundationDate.ToString();
            description.Text = _displayedOrganiser.Description;

        }

        private void DeleteOrganiser_Click(object sender, EventArgs e)
        {
            FirebaseCommunicator.GetDatabase().Reference.Child("organisers/" + _displayedOrganiser.Id).RemoveValue();
        }

        private void ViewEvents_Click(object sender, EventArgs e)
        {
            AdministratorViewAndDeleteOgranizerEvents page = new AdministratorViewAndDeleteOgranizerEvents();
            page.OrgId = _displayedOrganiser.Id;
            OpenFragment(page);
        }

       

        public void OpenFragment(SupportFragment newFragment)
        {
            if (newFragment.IsVisible)
                return;
            var Transaction = ChildFragmentManager.BeginTransaction();
            Transaction.Replace(Resource.Id.organizatorProfileFragmentContainer, newFragment);
            Transaction.AddToBackStack(null);
            Transaction.Commit();
        }
    }
}