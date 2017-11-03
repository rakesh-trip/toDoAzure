using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ToDoXamarinDemo
{
    public partial class TodoList : ContentPage
    {
        // Track whether the user has authenticated.
        bool authenticated = false;

        TodoItemManager manager;
        string selectedImage;
        byte[] selectedImageArray;
        public TodoList()
        {
            InitializeComponent();

            manager = TodoItemManager.DefaultManager;

            // OnPlatform<T> doesn't currently support the "Windows" target platform, so we have this check here.
            if (manager.IsOfflineEnabled && Device.OS == TargetPlatform.Windows)
            {
                var syncButton = new Button
                {
                    Text = "Sync items",
                    HeightRequest = 30
                };
                syncButton.Clicked += OnSyncItems;

                buttonsPanel.Children.Add(syncButton);
            }


        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Refresh items only when authenticated.
            if (authenticated == true)
            {
                // Set syncItems to true in order to synchronize the data
                // on startup when running in offline mode.
                await RefreshItems(true, syncItems: false);

                // Hide the Sign-in button.
                this.loginButton.IsVisible = false;
            }
        }

        // Data methods
        async Task AddItem(TodoItem item)
        {
            if (item.Name == "")
            {
                await DisplayAlert("ToDo", "Please enter a to do task", "OK");
            }
            else
            {
                await manager.SaveTaskAsync(item);
                todoList.ItemsSource = await manager.GetTodoItemsAsync();}
           
        }

        async Task CompleteItem(TodoItem item)
        {
            item.Done = true;
            await manager.SaveTaskAsync(item);
            todoList.ItemsSource = await manager.GetTodoItemsAsync();
        }

        public async void OnAdd(object sender, EventArgs e)
        {

            if (authenticated == false)
            {
                await DisplayAlert("ToDo", "Please sign in to add tasks.", "OK");
            }

            else if (selectedImageArray != null) {
                // upload image

                selectedImage = await BlobManager.Instance.UploadAsync(selectedImageArray);

                var todo = new TodoItem { Name = newItemName.Text, Image = "https://todoblobs.blob.core.windows.net/todoblobscontainer/" + selectedImage };
                await AddItem(todo);
            }
            else {

                var todo = new TodoItem { Name = newItemName.Text};
                await AddItem(todo);
            }

            newItemName.Text = string.Empty;
            imageV.Source = null;
            selectedImageArray = null;
            newItemName.Unfocus();
        }

        async void loginButton_Clicked(object sender, EventArgs e)
        {
            if (authenticated == true)
            {
                await DisplayAlert("ToDo", "You have already logged in","OK");
                await RefreshItems(true, syncItems: false);

            }
            else
            {
                authenticated = await App.Authenticator.Authenticate();
            }
            await RefreshItems(true, syncItems: false);

            // Set syncItems to true to synchronize the data on startup when offline is enabled.
        }


        async void imageButton_Clicked(object sender, EventArgs e)
        {


            if (authenticated == false)
            {
                await DisplayAlert("ToDo", "Please sign in to continue", "OK");
                await RefreshItems(true, syncItems: false);

            }else
            {
            // when image is selected
            MessagingCenter.Subscribe<byte[]>(this, "ImageSelected", (args) =>
            {


                Device.BeginInvokeOnMainThread(() =>
                {
                    //Set the source of the image view with the byte array
                    //var image = new Image();

                    selectedImageArray = (byte[])args;
                    imageV.Source = ImageSource.FromStream(() => new MemoryStream((byte[])args));

                });


            });

            Device.BeginInvokeOnMainThread(async () =>
            {
                //Ask the user if they want to use the camera or pick from the gallery
                var action = await DisplayActionSheet("Add Photo", "Cancel", null, "Choose Image");

                if (action == "Choose Image")
                {
                    DependencyService.Get<CameraInterface>().BringUpPhotoGallery();
                }
                //else if (action == "Take Photo")
                //{
                //    DependencyService.Get<CameraInterface>().BringUpCamera();
                //}



            });
            }
        }






        // Event handlers
        public async void OnSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var todo = e.SelectedItem as TodoItem;
            if (Device.OS != TargetPlatform.iOS && todo != null)
            {
                // Not iOS - the swipe-to-delete is discoverable there
                if (Device.OS == TargetPlatform.Android)
                {
                    await DisplayAlert(todo.Name, "Press-and-hold to complete task " + todo.Name, "Got it!");
                }
                else
                {
                    // Windows, not all platforms support the Context Actions yet
                    if (await DisplayAlert("Mark completed?", "Do you wish to complete " + todo.Name + "?", "Complete", "Cancel"))
                    {
                        await CompleteItem(todo);
                    }
                }
            }

            // prevents background getting highlighted
            todoList.SelectedItem = null;
        }

        // http://developer.xamarin.com/guides/cross-platform/xamarin-forms/working-with/listview/#context
        public async void OnComplete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var todo = mi.CommandParameter as TodoItem;
            await CompleteItem(todo);
        }

        // http://developer.xamarin.com/guides/cross-platform/xamarin-forms/working-with/listview/#pulltorefresh
        public async void OnRefresh(object sender, EventArgs e)
        {
            var list = (ListView)sender;
            Exception error = null;
            try
            {
                await RefreshItems(false, true);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                list.EndRefresh();
            }

            if (error != null)
            {
                await DisplayAlert("Refresh Error", "Couldn't refresh data (" + error.Message + ")", "OK");
            }
        }

        public async void OnSyncItems(object sender, EventArgs e)
        {
            await RefreshItems(true, true);
        }

        private async Task RefreshItems(bool showActivityIndicator, bool syncItems)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {
                todoList.ItemsSource = await manager.GetTodoItemsAsync(syncItems);
                
            }
        }


        private class ActivityIndicatorScope : IDisposable
        {
            private bool showIndicator;
            private ActivityIndicator indicator;
            private Task indicatorDelay;

            public ActivityIndicatorScope(ActivityIndicator indicator, bool showIndicator)
            {
                this.indicator = indicator;
                this.showIndicator = showIndicator;

                if (showIndicator)
                {
                    indicatorDelay = Task.Delay(2000);
                    SetIndicatorActivity(true);
                }
                else
                {
                    indicatorDelay = Task.FromResult(0);
                }
            }

            private void SetIndicatorActivity(bool isActive)
            {
                this.indicator.IsVisible = isActive;
                this.indicator.IsRunning = isActive;
            }

            public void Dispose()
            {
                if (showIndicator)
                {
                    indicatorDelay.ContinueWith(t => SetIndicatorActivity(false), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }


        }
    }
}

 