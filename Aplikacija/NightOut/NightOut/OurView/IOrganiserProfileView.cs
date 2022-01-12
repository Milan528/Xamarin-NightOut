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
    public interface IOrganiserProfileView
    {
        void UpdateInterface();

        NightOut.Presenter.OrganiserProfilePresenter OrganiserProfilePresenter { get; }

        void CommitUserChanges();

        void CreateEvent(NightOut.Model.Event EventToBeCreated);

        void UpdateEvent(NightOut.Model.Event EventToUpdate);

        bool IsActive { get; set; }
    }
}