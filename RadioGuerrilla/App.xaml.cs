using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace RadioGuerrilla
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            SettingsPane.GetForCurrentView().CommandsRequested += App_CommandsRequested;

            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                }

                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                if (!rootFrame.Navigate(typeof(MainPage), args.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            Window.Current.Activate();
        }

        void App_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var privacy = new SettingsCommand(
                "privacyId",
                "Privacy",
                (handler) =>
                {
                    Popup popup = BuildSettingsItem(new PrivacyPane(), 346);
                });
            args.Request.ApplicationCommands.Add(privacy);

            var settings = new SettingsCommand(
                "settingsId",
                "Settings",
                (handler) =>
                {
                    Popup popup = BuildSettingsItem(new SettingsPanel(), 346);
                });
            args.Request.ApplicationCommands.Add(settings);
        }

        private Popup BuildSettingsItem(UserControl userControl, int width)
        {
            var popup = new Popup();

            popup.IsLightDismissEnabled = true;
            userControl.Width = width;
            userControl.Height = Window.Current.Bounds.Height;

            popup.Child = userControl;

            popup.SetValue(Canvas.LeftProperty, Window.Current.Bounds.Width - width);
            popup.SetValue(Canvas.TopProperty, 0);
            popup.IsOpen = true;

            return popup;
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}
