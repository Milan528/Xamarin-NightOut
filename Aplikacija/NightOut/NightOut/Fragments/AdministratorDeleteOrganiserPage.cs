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
    public class AdministratorDeleteOrganiserPage : Android.Support.V4.App.Fragment
    {
        private Organiser displayedOrganiser;
        private IAdmin admin;
        private TextView name;
        private TextView rating;
        private TextView email;
        private TextView contact;
        private TextView city;
        private TextView foundationDate;
        private TextView description;
        private Button viewEvents;
        private Button deleteOrganiser;
        private ImageView organiserImage;

        public Organiser DisplayedOrganiser
        {
            get => displayedOrganiser;
            set => displayedOrganiser = value;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            admin = (IAdmin)this.Activity;
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
            name = View.FindViewById<TextView>(Resource.Id.adminOrganiserName);
            rating = View.FindViewById<TextView>(Resource.Id.adminOrganiserRating);
            email= View.FindViewById<TextView>(Resource.Id.adminOrganiserProfileEmail);
            contact = View.FindViewById<TextView>(Resource.Id.adminOrganiserProfileContact);
            city = View.FindViewById<TextView>(Resource.Id.adminOrganiserProfileCity);
            foundationDate = View.FindViewById<TextView>(Resource.Id.adminOrganiserProfileFoundationDate);
            description = View.FindViewById<TextView>(Resource.Id.adminOrganiserProfileDescription);
            organiserImage = View.FindViewById<ImageView>(Resource.Id.adminDeleteOrgImage);
            viewEvents = View.FindViewById<Button>(Resource.Id.adminViewEvents);
            deleteOrganiser = View.FindViewById<Button>(Resource.Id.adminDeleteOrganizerProfile);

            viewEvents.Click += ViewEvents_Click;
            deleteOrganiser.Click += DeleteOrganiser_Click;
        }

        private void FillAttributes()
        {
            name.Text = displayedOrganiser.UserName;
            rating.Text = displayedOrganiser.Rating.ToString();
            email.Text = displayedOrganiser.Email;
            contact.Text = displayedOrganiser.ContactPhone;
            city.Text = displayedOrganiser.Local.City;
            foundationDate.Text = displayedOrganiser.Local.FoundationDate.ToString();
            description.Text = displayedOrganiser.Description;
            organiserImage.SetImageBitmap(Presenter.ImagePresenter.DecodeImg(displayedOrganiser.Image));

        }

        private void DeleteOrganiser_Click(object sender, EventArgs e)
        {
            Model.EventListeners.DeleteOrganiserListener Listener = new Model.EventListeners.DeleteOrganiserListener();
            Listener.Organiser = displayedOrganiser;
            FirebaseCommunicator.GetDatabase()
                .Reference
                .Child(Resources.GetString(Resource.String.organisers_table_name))
                .Child(displayedOrganiser.Id)
                .RemoveValue().AddOnCompleteListener(Activity,Listener);
        }

        private void ViewEvents_Click(object sender, EventArgs e)
        {
            AdministratorViewAndDeleteOgranizerEvents page = new AdministratorViewAndDeleteOgranizerEvents();
            page.OrgId = displayedOrganiser.Id;
            OpenFragment(page);
        }


        public void OpenFragment(SupportFragment newFragment)
        {
            if (newFragment.IsVisible)
                return;
            var Transaction = ChildFragmentManager.BeginTransaction();
            Transaction.Replace(Resource.Id.adminOrganizatorProfileFragmentContainer, newFragment);
            Transaction.AddToBackStack(null);
            Transaction.Commit();
        }
    }
}