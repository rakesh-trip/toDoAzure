using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Foundation;
using System.Threading.Tasks;
using UIKit;

using Microsoft.WindowsAzure.MobileServices;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace ToDoXamarinDemo.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IAuthenticate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // Initialize Azure Mobile Apps
            CurrentPlatform.Init();

            // Initialize Xamarin Forms
            Forms.Init();

            App.Init(this);

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        // Define a authenticated user.
        private MobileServiceUser user;

        public async Task<bool> Authenticate()
        {
            var success = false;
            var message = string.Empty;
            try
            {
                // Sign in with google login using a server-managed flow.
                if (user == null)
                {
                    user = await TodoItemManager.DefaultManager.CurrentClient
                        .LoginAsync(UIApplication.SharedApplication.KeyWindow.RootViewController,
                                    MobileServiceAuthenticationProvider.Google, "mytodoappdemorakesh123");
                    Console.Write(user);


                    if (user != null)
                    {
                        message = string.Format("You are now signed-in as {0}.", user.UserId);
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            // Display the success or failure message.
            UIAlertView alert = new UIAlertView("Sign In Result", "You have signed in successfully.", null, "OK", null);
            alert.Show();



            return success;
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            return TodoItemManager.DefaultManager.CurrentClient.ResumeWithURL(url);
        }
    }
}

