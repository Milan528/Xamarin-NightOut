using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace NightOut.Presenter
{
    public class ImagePresenter
    {
        public static Android.Graphics.Bitmap DecodeImg(string profilePictureString)
        {
            byte[] decodedByteArray = Android.Util.Base64.Decode(profilePictureString, Android.Util.Base64Flags.Default);
            return Android.Graphics.BitmapFactory.DecodeByteArray(decodedByteArray, 0, decodedByteArray.Length);
        }

        public static System.Threading.Tasks.Task<string> DisplayCustomDialog(Context DialogContext,string Title,string Message)
        {
            var TaskComplete = new System.Threading.Tasks.TaskCompletionSource<string>();

            Android.App.AlertDialog.Builder QuestionDialog = new AlertDialog.Builder(DialogContext);
            QuestionDialog.SetTitle(Title);
            QuestionDialog.SetMessage(Message);
            QuestionDialog.SetPositiveButton("Yes", (senderAlert, args) =>
            {
                TaskComplete.SetResult("Yes");
            });
            QuestionDialog.SetNegativeButton("No", (senderAlert, args) =>
            {
                TaskComplete.SetResult("No");
            });

            Dialog NewDialog = QuestionDialog.Create();
            NewDialog.Show();
            return TaskComplete.Task;
        }

        public static string CompressImage(Android.Graphics.Bitmap Image)
        {
            System.IO.MemoryStream MyStream = new System.IO.MemoryStream();
            Image.Compress(Android.Graphics.Bitmap.CompressFormat.Webp, 60, MyStream);
            return Convert.ToBase64String(MyStream.ToArray());
        }

    }
}