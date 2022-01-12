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
    public class Organiser : Person
    {
        #region Attributes
        private double _rating;
        private int _numberOfVotes;
        private string _userName;
        private string _contactPhone;
        private string _profileImage;
        private Local _local;
        #endregion

        #region Properties

        public string Image
        {
            get => _profileImage;
            set => _profileImage = value;
        }

        public Local Local
        {
            get
            {
                return this._local;
            }
            set
            {
                this._local = value;
            }
        }

        public double Rating
        {
            get
            {
                return this._rating;
            }
            set
            {
                this._rating = value;
            }
        }

        public int NumberOfVotes
        {
            get
            {
                return this._numberOfVotes;
            }
            set
            {
                this._numberOfVotes = value;
            }
        }

        public string UserName
        {
            get
            {
                return this._userName.ToString();
            }
            set
            {
                this._userName = value.ToString();
            }
        }

        public string ContactPhone
        {
            get
            {
                return this._contactPhone.ToString();
            }
            set
            {
                this._contactPhone = value.ToString();
            }
        }

        #endregion

        #region Constructors

        public Organiser() : base()
        {
            _rating = 0;
            _numberOfVotes = 0;
            _userName = null;
            _contactPhone = null;
            _profileImage = null;
            _local = null;
        }

        public Organiser(string NewId) : base(NewId)
        {
            _rating = 0;
            _numberOfVotes = 0;
            _userName = null;
            _contactPhone = null;
            _profileImage = String.Empty;
        }

        public Organiser(string NewId, string NewEmail, string NewPassword, string NewDescription, double NewRating, int NewNumberOfVotes,
            string NewUserName, string NewContactPhone) : base(NewId, NewEmail, NewPassword, NewDescription)
        {
            _rating = NewRating;
            _numberOfVotes = NewNumberOfVotes;
            _userName = NewUserName.ToString();
            _contactPhone = NewContactPhone.ToString();
            _profileImage = String.Empty;
        }

        public Organiser(Organiser CopyOrganiser) :base(CopyOrganiser)
        {
            _rating = CopyOrganiser._rating;
            _numberOfVotes = CopyOrganiser._numberOfVotes;
            _userName = CopyOrganiser._userName;
            _contactPhone = CopyOrganiser._contactPhone;
            _profileImage = String.Empty;
        }

        #endregion

        #region Methods

        public double AddNewVote(int NewVote)
        {
            double VoteSum = Rating * NumberOfVotes;
            _numberOfVotes++;
            VoteSum += NewVote;
            _rating = VoteSum / _numberOfVotes;
            return _rating;
        }

        public static HashMap ToHashMap(Organiser organiser)
        {
            HashMap orgMap = new HashMap();

            if (organiser.Email != null)
            {
                orgMap.Put(Application.Context.Resources.GetString(Resource.String.organiser_email_column), organiser.Email);
            }
            else { orgMap.Put(Application.Context.Resources.GetString(Resource.String.organiser_email_column), ""); }

            if (organiser.UserName != null)
            {
                orgMap.Put(Application.Context.Resources.GetString(Resource.String.organiser_name_column), organiser.UserName);
            }
            else { orgMap.Put(Application.Context.Resources.GetString(Resource.String.organiser_name_column), ""); }
            
            if (organiser.ContactPhone != null)
            {
                orgMap.Put(Application.Context.Resources.GetString(Resource.String.organiser_contact_column), organiser.ContactPhone);
            }
            else { orgMap.Put(Application.Context.Resources.GetString(Resource.String.organiser_contact_column), ""); }

            if (organiser.Rating != 0)
            {
                orgMap.Put(Application.Context.Resources.GetString(Resource.String.organiser_rating_column), organiser.Rating.ToString());
            }
            else { orgMap.Put(Application.Context.Resources.GetString(Resource.String.organiser_rating_column), "0"); }

            if (organiser.NumberOfVotes != 0)
            {
                orgMap.Put(Application.Context.Resources.GetString(Resource.String.organiser_number_of_votes_column), organiser.NumberOfVotes.ToString());
            }
            else { orgMap.Put(Application.Context.Resources.GetString(Resource.String.organiser_number_of_votes_column), "0"); }

            if (organiser.Description != null)
            {
                orgMap.Put(Application.Context.Resources.GetString(Resource.String.organiser_description_column), organiser.Description);
            }
            else { orgMap.Put(Application.Context.Resources.GetString(Resource.String.organiser_description_column), ""); }

            if (organiser.Image != String.Empty)
            {
                orgMap.Put(Application.Context.Resources.GetString(Resource.String.organiser_profile_image_column), organiser.Image);
            }
            else { orgMap.Put(Application.Context.Resources.GetString(Resource.String.organiser_profile_image_column), ""); }

            if(organiser.Local!=null)
                orgMap.Put(Application.Context.Resources.GetString(Resource.String.local_id_column), organiser.Local.Id);

            return orgMap;
        }

        #endregion
    }
}