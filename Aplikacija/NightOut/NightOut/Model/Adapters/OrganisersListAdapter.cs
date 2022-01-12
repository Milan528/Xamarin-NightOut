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

namespace NightOut.Model.Adapters
{
    class OrganisersListAdapter : BaseAdapter<IListAdapter>
    {
        private Context _listContext;
        private List<NightOut.Model.Organiser> _organisersList;
        private Android.Support.V4.App.FragmentActivity _parentActivity;

        public Android.Support.V4.App.FragmentActivity ParentActivity
        {
            set => _parentActivity = value;
        }

        public OrganisersListAdapter(Context ListContext, List<NightOut.Model.Organiser> OrganisersList)
        {
            _listContext = ListContext;
            _organisersList = OrganisersList;
        }

        public override IListAdapter this[int position] => (IListAdapter)_organisersList.ElementAt(position);

        public override int Count => _organisersList.Count;

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View newView = View.Inflate(_listContext, Resource.Layout.organizator_my_follower_container, null);
            newView.FindViewById<TextView>(Resource.Id.organizatorFollowerNameContainer).Text
                = _organisersList.ElementAt(position).UserName
                + " " + _organisersList.ElementAt(position).Local.City 
                + " " + _organisersList.ElementAt(position).Local.Address;

            if(_organisersList.ElementAt(position).Image!=String.Empty)
                newView.FindViewById<ImageView>(Resource.Id.organizatorFollowerProfilePicture)
                   .SetImageBitmap(Presenter.ImagePresenter.DecodeImg(_organisersList.ElementAt(position).Image));

            newView.Click += (object sender, EventArgs e)=>
            {
                var Transaction = _parentActivity.SupportFragmentManager.BeginTransaction();
                NightOut.Fragments.UserViewOrganiserFragment ViewOrganiserFragment = new NightOut.Fragments.UserViewOrganiserFragment();
                ViewOrganiserFragment.OrganiserId = _organisersList.ElementAt(position).Id;
                Transaction.Replace(Resource.Id.UserProfileFragmentContainer, ViewOrganiserFragment);
                Transaction.AddToBackStack(null);
                Transaction.Commit();
            };
            return newView;
        }
        
    }
}