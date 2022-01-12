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
    class FollowersListAdapter : BaseAdapter<IListAdapter>
    {

        private Context _listContext;
        private List<NightOut.Model.User> _followersList;
        private Android.Support.V4.App.FragmentActivity _parentActivity;

        public Android.Support.V4.App.FragmentActivity ParentActivity
        {
            set => _parentActivity = value;
        }

        public FollowersListAdapter(Context ListContext, List<NightOut.Model.User> FollowersList)
        {
            _listContext = ListContext;
            _followersList = FollowersList;
        }

        public override IListAdapter this[int position] => (IListAdapter)_followersList.ElementAt(position);

        public override int Count => _followersList.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View newView = View.Inflate(_listContext, Resource.Layout.organizator_my_follower_container, null);

            newView.FindViewById<TextView>(Resource.Id.organizatorFollowerNameContainer).Text
                = _followersList.ElementAt(position).FullName;

            if(_followersList.ElementAt(position).Image!=String.Empty)
                newView.FindViewById<ImageView>(Resource.Id.organizatorFollowerProfilePicture)
                    .SetImageBitmap(Presenter.ImagePresenter.DecodeImg(_followersList.ElementAt(position).Image));

            return newView;
        }
    }
}