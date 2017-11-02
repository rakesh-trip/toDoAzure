using System;
using System.Threading.Tasks;
using System.IO;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Media;
using Android.Graphics;
using Android.Provider;
using Android.Database;

using Microsoft.WindowsAzure.MobileServices;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;



namespace ToDoXamarinDemo.Droid
{
    [Activity(Label = "ToDoXamarinDemo.Droid",
        Icon = "@drawable/icon",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        Theme = "@android:style/Theme.Holo.Light")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity, IAuthenticate
    {


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            //Since we set the request code to 1 for both the camera and photo gallery, that's what we need to check for
            if (requestCode == 1)
            {
                if (resultCode == Result.Ok)
                {
                    if (data.Data != null)
                    {
                        //Grab the Uri which is holding the path to the image
                        Android.Net.Uri uri = data.Data;

                        //Read the meta data of the image to determine what orientation the image should be in
                        int orientation = getOrientation(uri);

                        //Start a background task so we can do all the bitmap stuff off the UI thread
                        BitmapWorkerTask task = new BitmapWorkerTask(this.ContentResolver, uri);

                        task.Execute(orientation);
                    }
                }
            }
        }

        public int getOrientation(Android.Net.Uri photoUri)
        {
            ICursor cursor = Application.ApplicationContext.ContentResolver.Query(photoUri, new String[] { MediaStore.Images.ImageColumns.Orientation }, null, null, null);

            if (cursor.Count != 1)
            {
                return -1;
            }

            cursor.MoveToFirst();
            return cursor.GetInt(0);
        }




        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Initialize Azure Mobile Apps
            CurrentPlatform.Init();

            // Initialize Xamarin Forms
            Forms.Init(this, bundle);

            // Initialize the authenticator before loading the app.
            App.Init((IAuthenticate)this);

            // Load the main application
            LoadApplication(new App());
        }
        // Define a authenticated user.
        private MobileServiceUser user;

        public async Task<bool> Authenticate()
        {
            var success = false;
            var message = string.Empty;
            try
            {
                // Sign in with Google login using a server-managed flow.
                user = await TodoItemManager.DefaultManager.CurrentClient.LoginAsync(this,
                                                                                     MobileServiceAuthenticationProvider.Google, "mytodoappdemorakesh123");
                if (user != null)
                {
                    message = string.Format("you are now signed-in as {0}.",
                        user.UserId);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            // Display the success or failure message.
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetMessage(message);
            builder.SetTitle("Sign-in result");
            builder.Create().Show();

            return success;
        }
    }

    //Start: Added for BitmapWorkerTask

    public class BitmapWorkerTask : AsyncTask<int, int, Bitmap>
    {
        private Android.Net.Uri uriReference;
        private int data = 0;
        private ContentResolver resolver;

        public BitmapWorkerTask(ContentResolver cr, Android.Net.Uri uri)
        {
            uriReference = uri;
            resolver = cr;
        }

        // Decode image in background.
        protected override Bitmap RunInBackground(params int[] p)
        {
            //This will be the orientation that was passed in from above (task.Execute(orientation);)
            data = p[0];

            Bitmap mBitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(resolver, uriReference);
            Bitmap myBitmap = null;

            if (mBitmap != null)
            {
                //In order to rotate the image we create a Matrix object, rotate if the image is not already in it's correct orientation
                Matrix matrix = new Matrix();
                if (data != 0)
                {
                    matrix.PreRotate(data);
                }

                myBitmap = Bitmap.CreateBitmap(mBitmap, 0, 0, mBitmap.Width, mBitmap.Height, matrix, true);
                return myBitmap;
            }

            return null;
        }

        //Called when the RunInBackground method has finished
        protected override void OnPostExecute(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                MemoryStream stream = new MemoryStream();

                //Compressing by 50%, feel free to change if file size is not a factor
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 50, stream);
                byte[] bitmapData = stream.ToArray();

                //Send image byte array back to UI
                MessagingCenter.Send<byte[]>(bitmapData, "ImageSelected");

                //clean up bitmaps so the app doesn't crash from using up too much memory
                bitmap.Recycle();
                GC.Collect();
            }
        }
    }
    //End: Added for BitmapWorkerTask

}

