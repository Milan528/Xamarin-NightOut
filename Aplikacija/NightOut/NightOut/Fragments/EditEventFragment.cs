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

namespace NightOut.Fragments
{
    public class EditEventFragment : Android.Support.V4.App.Fragment
    {
        #region Attributes

        private NightOut.Model.Event _editEvent;
        private NightOut.OurView.IOrganiserProfileView _parentActivity;

        private EditText _eventCity;
        private EditText _eventAddress;
        private EditText _eventDescription;
        private TextView _eventDate;
        private Spinner _eventType;
        private Button _editButton;
        private Button _deleteButton;
        private ImageView _editImage;
        private string _encodedImage = String.Empty;
        
        #endregion


        #region Properties
        public NightOut.Model.Event EditEvent
        {
            set => _editEvent = value;
        }

        #endregion

        #region Overriden Methodes

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _parentActivity = (NightOut.OurView.IOrganiserProfileView)Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Android.Views.View view = inflater.Inflate(Resource.Layout.organizator_edit_and_delete_page, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            SetUpAttributes();
            SetUpListeners();
            FillOutAttributes();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == (int)Result.Ok)
            {
                System.IO.Stream pictureStream = Application.Context.ContentResolver.OpenInputStream(data.Data);
                Android.Graphics.Bitmap ImageBitmap = Android.Graphics.BitmapFactory.DecodeStream(pictureStream);
                _editImage.SetImageBitmap(ImageBitmap);
                System.Threading.Tasks.Task.Run(() => _editEvent.Image = Presenter.ImagePresenter.CompressImage(ImageBitmap));
            }
        }

        #endregion

        #region Methodes

        private void SetUpAttributes()
        {
            _editButton = View.FindViewById<Button>(Resource.Id.organiserEditSaveChangesEvent);
            _deleteButton = View.FindViewById<Button>(Resource.Id.organiserDeleteEvent);
            _eventCity = View.FindViewById<EditText>(Resource.Id.editEventCityText);
            _eventAddress = View.FindViewById<EditText>(Resource.Id.editEventAddressText);
            _eventDescription = View.FindViewById<EditText>(Resource.Id.editEventBioText);
            _eventType = View.FindViewById<Spinner>(Resource.Id.editEventTypeSpinner);
            _eventType.Adapter = new ArrayAdapter<string>(Context, Android.Resource.Layout.SimpleListItem1, Enum.GetNames(typeof(NightOut.Model.EventType)));
            _eventDate = View.FindViewById<TextView>(Resource.Id.editEventDate);
            _editImage = View.FindViewById<ImageView>(Resource.Id.editEventImage);
        }

        private void SetUpListeners()
        {
            _editButton.Click += OnEditButtonClicked;
            _eventDate.Click += OnEventDateClicked;
            _editImage.Click += OnEditImageClicked;
            _deleteButton.Click += OnDeleteButtonClicked;
        }

        private void FillOutAttributes()
        {
            _eventCity.Text = _editEvent.City;
            _eventAddress.Text = _editEvent.Address;
            _eventDescription.Text = _editEvent.Description;
            _eventDate.Text = _editEvent.DateOfEvent.ToString(Resources.GetString(Resource.String.date_format));
            _eventType.SetSelection((int)_editEvent.TypeOfEvent);
            if (_editEvent.Image != "")
                _editImage.SetImageBitmap(Presenter.ImagePresenter.DecodeImg(_editEvent.Image));
        }

        private void ReadFromInputs()
        {
            _editEvent.Address = _eventAddress.Text;
            _editEvent.City = _eventCity.Text;
            _editEvent.DateOfEvent = DateTime.ParseExact(_eventDate.Text, Resources.GetString(Resource.String.date_format), null);
            _editEvent.TypeOfEvent = (NightOut.Model.EventType)_eventType.SelectedItemPosition;
            _editEvent.TimeOfEvent = "";
            _editEvent.Description = _eventDescription.Text;
            if (_encodedImage != "")
                _editEvent.Image = _encodedImage;
        }

        private bool CheckInputs()
        {
            if (_eventCity.Text == "")
            {
                Toast.MakeText(Android.App.Application.Context, Resource.String.city_invalid_input_toast, ToastLength.Short).Show();
                return false;
            }
            if (_eventAddress.Text == "")
            {
                Toast.MakeText(Android.App.Application.Context, Resource.String.address_invalid_input_toast, ToastLength.Short).Show();
                return false;
            }
            if (_eventDescription.Text == "")
            {
                Toast.MakeText(Android.App.Application.Context, Resource.String.description_invalid_input, ToastLength.Short).Show();
                return false;
            }
            if (_eventDate.Text == "")
            {
                Toast.MakeText(Android.App.Application.Context, Resource.String.date_of_event_invalid_input, ToastLength.Short).Show();
                return false;
            }
            return true;
        }

        #endregion

        #region Listeners

        private async void OnEditImageClicked(object sender, EventArgs e)
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

        private void OnEventDateClicked(object sender, EventArgs e)
        {
            DatePickerDialog Dialog = new DatePickerDialog(Activity, Android.Resource.Style.ThemeHoloLightDialogMinWidth, OnDatePickerChosed, DateTime.Today.Year, DateTime.Today.Month - 1, DateTime.Today.Day);
            Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
            Dialog.Show();
        }

        private void OnDatePickerChosed(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            if (e.Date < DateTime.Today)
                Toast.MakeText(Activity, Resource.String.date_of_event_invalid_input, ToastLength.Short).Show();
            else
            {
                DateTime Proba = e.Date.ToUniversalTime().AddDays(1);
                _eventDate.Text = Proba.ToString(Resources.GetString(Resource.String.date_format));
            }
        }

        private void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (!CheckInputs())
                return;
            ReadFromInputs();
            _parentActivity.UpdateEvent(_editEvent);
            Activity.OnBackPressed();
        }

        private void OnDeleteButtonClicked(object sender,EventArgs e)
        {
            Model.FirebaseCommunicator.GetDatabase().Reference
                .Child(Resources.GetString(Resource.String.events_table_name))
                .Child(_editEvent.Id)
                .RemoveValue();

            Model.FirebaseCommunicator.GetDatabase().Reference
                .Child(Resources.GetString(Resource.String.user_followed_events_table_name))
                .OrderByChild(Resources.GetString(Resource.String.event_id_column))
                .EqualTo(_editEvent.Id)
                .AddListenerForSingleValueEvent(new Model.EventListeners.RemoveFollowedEventsListener());

            Activity.OnBackPressed();
        }
        #endregion
    }
}