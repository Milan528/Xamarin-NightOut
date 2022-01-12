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

using Java.Util;

namespace NightOut.Model
{
    public class Local
    {

        #region Attributes
        private string _id;
        private string _address;
        private string _city;
        private DateTime _foundationDate;
        #endregion

        #region Properties

        public string Address
        {
            get
            {
                return this._address.ToString();
            }
            set
            {
                this._address = value.ToString();
            }
        }

        public string City
        {
            get
            {
                return this._city.ToString();
            }
            set
            {
                this._city = value.ToString();
            }
        }

        public DateTime FoundationDate
        {
            get
            {
                return this._foundationDate;
            }
            set
            {
                this._foundationDate = value;

            }
        }

        public string Id
        {
            get
            {
                return this._id;
;           }
        }

        #endregion

        #region Constructors

        public Local()
        {
            _id = null;
            _address = null;
            _city = null;
            _foundationDate = DateTime.MaxValue;
        }

        public Local(string NewId)
        {
            this._id = NewId;
            this._address = null;
            this._city = null;
            this._foundationDate = DateTime.MaxValue;
        }

        public Local(string NewId,string NewAddress,string NewCity,string NewName,DateTime NewFoundationDate)
        {
            this._id = NewId;
            this._address = NewAddress.ToString();
            this._city = NewCity.ToString();
            this._foundationDate = NewFoundationDate;
        }

        public Local(Local CopyLocal)
        {
            _id = CopyLocal._id;
            _foundationDate = CopyLocal._foundationDate;
            _city = CopyLocal._city;
            _address = CopyLocal._address;
        }
        #endregion

        #region Methods

        public static HashMap ToHashMap(Local local)
        {
            HashMap localMap = new HashMap();

            if (local.Address != null)
                localMap.Put(Application.Context.Resources.GetString(Resource.String.local_address_column), local.Address);
            else localMap.Put(Application.Context.Resources.GetString(Resource.String.local_address_column), "");

            if (local.City != null)
                localMap.Put(Application.Context.Resources.GetString(Resource.String.local_city_column), local.City);
            else localMap.Put(Application.Context.Resources.GetString(Resource.String.local_city_column), "");

            if(local.FoundationDate != null)
                localMap.Put(Application.Context.Resources.GetString(Resource.String.local_foundation_date_column), local.FoundationDate.ToString(Application.Context.Resources.GetString(Resource.String.date_format)));
            else localMap.Put(Application.Context.Resources.GetString(Resource.String.local_foundation_date_column), DateTime.MaxValue.ToString(Application.Context.Resources.GetString(Resource.String.date_format)));
            
            return localMap;
        }
        #endregion
    }
}