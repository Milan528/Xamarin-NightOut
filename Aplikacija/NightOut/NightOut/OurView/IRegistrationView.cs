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

namespace NightOut.OurView
{
    public interface IRegistrationView
    {
        void OpenFormFragment(Type FormType);

        void RegisterOrganizator(string email, string password, string name, string city, string address);

        void RegisterUser(string email, string password, string name, string surname, int sex, string date);

        void GoToLoginPage();

        void OnOrganizatorRegistrationFailed(object sender, EventArgs e);

        void OnUserRegistrationFailed(object sender, EventArgs e);

        void OnOrganizatorRegistrationSucceed(object sender, EventArgs e);

        void OnUserRegistrationSucceed(object sender, EventArgs e);
    }
}