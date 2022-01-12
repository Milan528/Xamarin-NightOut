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
using Firebase.Database;

namespace NightOut.Model.EventListeners
{
    public class UserListener : Java.Lang.Object, IValueEventListener
    {
        List<User> UserList = new List<User>();
        public event EventHandler<UserDataEventArgs> UserRetrived;

        public class UserDataEventArgs : EventArgs
        {
            public List<User> Users { get; set; }
        }

        public void OnCancelled(DatabaseError error)
        { }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                UserList.Add(GetUserFromData(snapshot));
                UserRetrived.Invoke(this, new UserDataEventArgs { Users = UserList });
            }
        }

        private User GetUserFromData(DataSnapshot UserData)
        {
            User ResultUser = new User(UserData.Key)
            {
                Email = UserData.Child(Application.Context.Resources.GetString(Resource.String.user_email_column)).Value.ToString(),
                FirstName = UserData.Child(Application.Context.Resources.GetString(Resource.String.user_firstname_column)).Value.ToString(),
                Surname = UserData.Child(Application.Context.Resources.GetString(Resource.String.user_surname_column)).Value.ToString(),
                Sex = (Model.Sex)Convert.ToInt32(UserData.Child(Application.Context.Resources.GetString(Resource.String.user_sex_column)).Value.ToString()),
                Description = UserData.Child(Application.Context.Resources.GetString(Resource.String.user_description_column)).Value.ToString(),
                Image = UserData.Child(Application.Context.Resources.GetString(Resource.String.user_profile_image_column)).Value.ToString(),
            };
            string DateString = UserData.Child(Application.Context.Resources.GetString(Resource.String.user_birthday_column)).Value.ToString();
            DateTime Date = DateTime.ParseExact(DateString, Application.Context.Resources.GetString(Resource.String.date_format), null);
            if (Date == DateTime.MaxValue)
            {
                ResultUser.DateOfBirth = DateTime.MaxValue;
            }
            else
            {
                ResultUser.DateOfBirth = Date;
            }
            string isAdministrator = UserData.Child(Application.Context.Resources.GetString(Resource.String.user_is_administrator_column)).Value.ToString();
            if (isAdministrator == "true")
                ResultUser.IsAdministrator = true;
            return ResultUser;
        }

    }
}