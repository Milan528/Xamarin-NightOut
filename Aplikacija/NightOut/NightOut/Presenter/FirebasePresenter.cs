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
using NightOut.Model;

namespace NightOut.Presenter
{
    public class FirebasePresenter
    {
        #region Attributes

        private Activity _activity;
        private FirebaseCommunicator _communicator;

        #endregion

        #region Constructor

        public FirebasePresenter(Activity activity)
        {
            this._activity = activity;
            _communicator = new FirebaseCommunicator(activity);
        }

        #endregion

        #region Login

        public void InitializeLogin()
        {
            this._communicator.InitializeDatabaseLogin();
        }

        public void LoginPerson(string email,string password)
        {
            _communicator.Login(email, password);
        }

        #endregion

        #region Registration

        public void InitializeRegistration()
        {
            _communicator.InitializeDatabaseRegistration();
        }

        public void RegisterOrganizator(string email, string password, string name, string city, string address)
        {
            Local OrganiserLocal = new Local()
            {
                City = city,
                Address = address,
                FoundationDate = DateTime.MaxValue
            };
            Organiser NewOrganiser = new Organiser()
            {
                Email = email,
                Password = password,
                UserName = name,
                Rating = 0,
                NumberOfVotes = 0,
                Local = OrganiserLocal,
                ContactPhone = String.Empty,
                Image = String.Empty,
                Description = String.Empty
            };
            _communicator.Local = OrganiserLocal;
            _communicator.RegisterOrganizator(NewOrganiser);
        }

        public void RegisterUser(string email, string password, string name, string surname, int sex, string date)
        {
            User NewUser = new User()
            {
                Email = email,
                Password = password,
                Description = string.Empty,
                FirstName = name,
                Surname = surname,
                Sex = (Sex)sex,
                DateOfBirth = DateTime.ParseExact(date, Application.Context.Resources.GetString(Resource.String.date_format), null),
                Image = string.Empty,
                IsAdministrator = false
            };
            _communicator.RegisterUser(NewUser);
        }
        
        #endregion
    }
}