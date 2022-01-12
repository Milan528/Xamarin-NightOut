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

namespace NightOut.Fragments 
{
    public class CreateEventFragment : Android.Support.V4.App.Fragment
    {
        #region Attributes

        private EditText _city;
        private EditText _address;
        private EditText _description;
        private Spinner _eventType;
        private TextView _dateOfEvent;
        private Button _addEvent;
        private ImageView _eventImage;
        private string _encodedImage = String.Empty;

        private NightOut.OurView.IOrganiserProfileView _parent;

        #endregion

        #region Ovverides

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _parent = (NightOut.OurView.IOrganiserProfileView)this.Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.organizator_create_event_page, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            SetUpAttributes();
            SetUpListeners();
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == (int)Result.Ok)
            {
                System.IO.Stream pictureStream = Application.Context.ContentResolver.OpenInputStream(data.Data);
                Android.Graphics.Bitmap ImageBitmap = Android.Graphics.BitmapFactory.DecodeStream(pictureStream);
                _eventImage.SetImageBitmap(ImageBitmap);
                System.Threading.Tasks.Task.Run(() => CompressImage(ImageBitmap));
            }
        }

        #endregion

        #region Methodes

        public void SetUpAttributes()
        {
            _city = View.FindViewById<EditText>(Resource.Id.eventCityText);
            _address = View.FindViewById<EditText>(Resource.Id.eventAddressText);
            _description = View.FindViewById<EditText>(Resource.Id.eventDescription);
            _eventType = View.FindViewById<Spinner>(Resource.Id.eventTypeSpinner);
            _dateOfEvent = View.FindViewById<TextView>(Resource.Id.eventDate);
            _eventImage = View.FindViewById<ImageView>(Resource.Id.createEventImage);
            _addEvent = View.FindViewById<Button>(Resource.Id.organiserAddEvent);
            _eventType.Adapter= new ArrayAdapter<string>(Context,
                Android.Resource.Layout.SimpleListItem1,
                Enum.GetNames(typeof(NightOut.Model.EventType)));
        }

        public void SetUpListeners()
        {
            _eventImage.Click += OnEventImageClicked;
            _dateOfEvent.Click += OnDateOfEventClicked;
            _addEvent.Click += OnAddEventClicked;
        }

        private async void OnEventImageClicked(object sender, EventArgs e)
        {
            string resault = await Presenter.ImagePresenter.DisplayCustomDialog(Activity,
                Resources.GetString(Resource.String.event_picture_title),
                Resources.GetString(Resource.String.event_picture_question));
            if (resault == "No")
                return;
            Intent GalleryPage = new Intent();
            GalleryPage.SetType("image/*");
            GalleryPage.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(GalleryPage, Resources.GetString(Resource.String.gallery_event_title)), 0);
        }
        
        private void CompressImage(Android.Graphics.Bitmap Image)
        {
            System.IO.MemoryStream MyStream = new System.IO.MemoryStream();
            Image.Compress(Android.Graphics.Bitmap.CompressFormat.Webp, 60, MyStream);
            _encodedImage = Convert.ToBase64String(MyStream.ToArray());
        }
        
        private Event ReadEvent()
        {
            Event NewEvent = new Event
            {
                OrganiserId= _parent.OrganiserProfilePresenter.LogedInUser.Id,
                Address = _address.Text,
                City = _city.Text,
                Description = _description.Text,
                TypeOfEvent = (NightOut.Model.EventType)_eventType.SelectedItemPosition,
                DateOfEvent = DateTime.ParseExact(_dateOfEvent.Text, Resources.GetString(Resource.String.date_format), null),
                Image = _encodedImage,
                TimeOfEvent=null
            };
            return NewEvent;
        }

        private bool CheckInputs()
        {
            if(_city.Text==String.Empty)
            {
                Toast.MakeText(Android.App.Application.Context, Resource.String.city_invalid_input_toast, ToastLength.Short).Show();
                return false;
            }
            if (_address.Text == String.Empty)
            {
                Toast.MakeText(Android.App.Application.Context, Resource.String.address_invalid_input_toast, ToastLength.Short).Show();
                return false;
            }
            if (_description.Text == String.Empty)
            {
                Toast.MakeText(Android.App.Application.Context, Resource.String.description_invalid_input, ToastLength.Short).Show();
                return false;
            }
            if (_dateOfEvent.Text == String.Empty)
            {
                Toast.MakeText(Android.App.Application.Context, Resource.String.date_of_event_invalid_input, ToastLength.Short).Show();
                return false;
            }
            return true;
        }

        #endregion

        #region Listeners

        private void OnDateOfEventClicked(object sender, EventArgs e)
        {
            DatePickerDialog Dialog = new DatePickerDialog(Activity,
                Android.Resource.Style.ThemeHoloLightDialogMinWidth,
                OnDatePickerChosed, DateTime.Today.Year,
                DateTime.Today.Month - 1,
                DateTime.Today.Day);
            Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
            Dialog.Show();
        }

        private void OnDatePickerChosed(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            if (e.Date < DateTime.Today)
                Toast.MakeText(Activity, Resource.String.date_invalid_input_toast, ToastLength.Short).Show();
            else
            {
                DateTime Proba = e.Date.ToUniversalTime().AddDays(1);
                _dateOfEvent.Text = Proba.ToString(Resources.GetString(Resource.String.date_format));
            }
        }

        private void OnAddEventClicked(object sender, EventArgs e)
        {
            CheckInputs();
            if (CheckInputs())
            {
                NightOut.Model.Event NewEvent = ReadEvent();
                _parent.CreateEvent(NewEvent);
                Activity.OnBackPressed();
            }
        }

        #endregion

    }
}