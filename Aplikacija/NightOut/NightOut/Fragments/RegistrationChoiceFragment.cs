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

namespace NightOut.Fragments
{
    public class RegistrationChoiceFragment : Android.Support.V4.App.Fragment
    {
        #region Attributes

        private IRegistrationView _parentActivity;
        private ImageButton _btnUser;
        private ImageButton _btnOrgan;

        #endregion

        #region Overriden methodes

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _parentActivity = (IRegistrationView)this.Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Android.Views.View view = inflater.Inflate(Resource.Layout.register_choice_fragment, container, false);
            _btnOrgan = view.FindViewById<ImageButton>(Resource.Id.btnOrganRegister);
            _btnUser = view.FindViewById<ImageButton>(Resource.Id.btnUserRegister);
            _btnUser.Click += OnBtnUserClicked;
            _btnOrgan.Click += OnBtnOrganClicked;
            return view;
        }

        #endregion

        private void OnBtnOrganClicked(object sender, EventArgs e) => _parentActivity.OpenFormFragment(typeof(RegisterFormOrganFragment));

        public void OnBtnUserClicked(object o, EventArgs e) => _parentActivity.OpenFormFragment(typeof(RegisterFormUserFragment));

    }
}