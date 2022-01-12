using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Android.Content.Res;
using NightOut.Model.EventListeners;

using Java.Util;

namespace NightOut.Model
{
    public enum Sex
    {
        Undefined,
        Male,
        Female
    }
    public class User : Person
    {
        #region Attributes
        private string _firstName;
        private string _surname;
        private Sex _sex;
        private DateTime _dateOfBirth;
        private bool _isAdministrator;
        private string _imageString;

        #endregion

        #region Properties

        public string Image
        {
            get => _imageString;
            set => _imageString=value;
        }

        public string FirstName
        {
            get
            {
                return this._firstName.ToString();
            }
            set
            {
                this._firstName = value.ToString();
            }
        }

        public string Surname
        {
            get
            {
                return this._surname.ToString();
            }
            set
            {
                this._surname = value.ToString();
            }
        }

        public string FullName
        {
            get
            {
                return this._firstName.ToString() + " " + this._surname.ToString();
            }
        }

        public Sex Sex
        {
            get
            {
                if (this._sex == Sex.Male)
                    return Sex.Male;
                else
                    return Sex.Female;
            }
            set
            {
                this._sex = value;
            }
        }

        public DateTime DateOfBirth
        {
            get
            {
                return new DateTime(this._dateOfBirth.Year,this._dateOfBirth.Month,this._dateOfBirth.Day);
            }
            set
            {
                this._dateOfBirth = value;
            }
        }

        public bool IsAdministrator
        {
            get
            {
                return this._isAdministrator;
            }
            set
            {
                this._isAdministrator = value;
            }
        }


        #endregion

        #region Constructors

        public User():base()
        {
            _firstName = null;
            _surname = null;
            _sex = Sex.Undefined;
            _dateOfBirth = DateTime.MaxValue;
            _isAdministrator = false;
            _imageString = string.Empty;
        }

        public User(string NewId) : base(NewId)
        {
            _firstName = null;
            _surname = null;
            _sex = Sex.Undefined;
            _dateOfBirth = DateTime.MaxValue;
            _isAdministrator = false;
            _imageString = String.Empty;
        }

        public User(string NewId, string NewEmail, string NewPassword, string NewDescription,string NewFirstName,string NewSurname,
            Sex NewSex,DateTime NewDateOfBirth,bool NewIsAdministrator):base(NewId,NewEmail,NewPassword,NewDescription)
        {
            _firstName = NewFirstName.ToString();
            _surname = NewSurname.ToString();
            _sex = NewSex;
            _dateOfBirth = NewDateOfBirth;
            _isAdministrator = NewIsAdministrator;
            _imageString = String.Empty;
        }

        public User(User CopyUser):base(CopyUser)
        {
            _firstName = CopyUser._firstName;
            _surname = CopyUser._surname;
            _sex = CopyUser._sex;
            _dateOfBirth = CopyUser._dateOfBirth;
            _isAdministrator = CopyUser._isAdministrator;
            _imageString = String.Empty;
        }
        #endregion

        #region Methods
        public static HashMap ToHashMap(User user)
        {
            HashMap userMap = new HashMap();

            if (user.Email != null)
                userMap.Put(Application.Context.Resources.GetString(Resource.String.user_email_column), user.Email);
            else userMap.Put(Application.Context.Resources.GetString(Resource.String.user_email_column), "");

            if (user.FirstName != null)
                userMap.Put(Application.Context.Resources.GetString(Resource.String.user_firstname_column), user.FirstName);
            else userMap.Put(Application.Context.Resources.GetString(Resource.String.user_firstname_column), "");

            if (user.Surname != null)
                userMap.Put(Application.Context.Resources.GetString(Resource.String.user_surname_column), user.Surname);
            else userMap.Put(Application.Context.Resources.GetString(Resource.String.user_surname_column), "");

            if (user.Sex != 0)
                userMap.Put(Application.Context.Resources.GetString(Resource.String.user_sex_column), ((int)user.Sex).ToString());
            else userMap.Put(Application.Context.Resources.GetString(Resource.String.user_sex_column), "0");

            if (user.DateOfBirth != DateTime.MaxValue)
                userMap.Put(Application.Context.Resources.GetString(Resource.String.user_birthday_column), user.DateOfBirth.ToString(Application.Context.Resources.GetString(Resource.String.date_format)));
            else userMap.Put(Application.Context.Resources.GetString(Resource.String.user_birthday_column), DateTime.MaxValue.ToString(Application.Context.Resources.GetString(Resource.String.date_format)));

            if(user.IsAdministrator)
                userMap.Put(Application.Context.Resources.GetString(Resource.String.user_is_administrator_column), "true");
            else userMap.Put(Application.Context.Resources.GetString(Resource.String.user_is_administrator_column), "false");

            if (user.Description != null)
                userMap.Put(Application.Context.Resources.GetString(Resource.String.user_description_column), user.Description);
            else userMap.Put(Application.Context.Resources.GetString(Resource.String.user_description_column), "");

            if (user.Image != String.Empty)
                userMap.Put(Application.Context.Resources.GetString(Resource.String.user_profile_image_column), user.Image);
            else userMap.Put(Application.Context.Resources.GetString(Resource.String.user_profile_image_column), "");

            return userMap;
        }
        #endregion
    }

}