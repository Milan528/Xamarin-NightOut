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
    public class UserListListener : Java.Lang.Object, Firebase.Database.IValueEventListener
    {
        List<User> UserList = new List<User>();
        public event EventHandler<UsersDataEventArgs> UsersRetrived;

        public class UsersDataEventArgs : EventArgs
        {
            public List<User> Users { get; set; }
        }

        public void OnCancelled(DatabaseError error)
        { }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                DataSnapshot[] AllData = snapshot.Children.ToEnumerable<DataSnapshot>().ToArray<DataSnapshot>();
                foreach (DataSnapshot SingleUserdata in AllData)
                {
                    UserList.Add(GetUserFromData(SingleUserdata));
                }
                UsersRetrived.Invoke(this, new UsersDataEventArgs { Users = UserList });
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