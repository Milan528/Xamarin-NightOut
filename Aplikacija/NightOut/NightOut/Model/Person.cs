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

namespace NightOut.Model
{
    public abstract class Person : Java.Lang.Object
    {
        #region Attributes
        protected string _id;
        protected string _email;
        protected string _password;
        protected string _description;
        #endregion

        #region Properties

        public string Email
        {
            get
            {
                return this._email.ToString();
            }
            set
            {
                this._email = value.ToString();
            }
        }

        public string Password
        {
            get
            {
                return this._password;
            }
            set
            {
                if(value != null)
                    if (value.Length < 8)
                        return;
                this._password = value;
            }
        }

        public string Description
        {
            get
            {
                return this._description.ToString();
            }
            set
            {
                this._description = value.ToString();
            }
        }

        public string Id
        {
            get
            {
                return this._id;
            }
        }

        #endregion

        #region Constructors
        public Person()
        {
            _email = null;
            _id = null;
            _password = null;
            _description = null;
        }

        public Person(string NewId)
        {
            _id = NewId;
            _email = null;
            _password = null;
            _description = null;
        }

        public Person(string NewId, string NewEmail, string NewPassword, string NewDescription)
        {
            _id = NewId;
            _email = NewEmail.ToString();
            Password = NewPassword;
            _description = NewDescription.ToString();
        }

        public Person(Person CopyPerson)
        {
            _id = CopyPerson._id;
            _password = CopyPerson._password;
            _email = CopyPerson._email;
            _description = CopyPerson._description;
        }

        #endregion
    }
}