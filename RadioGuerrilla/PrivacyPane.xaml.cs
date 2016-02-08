using Windows.UI.ApplicationSettings;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace RadioGuerrilla
{
    public sealed partial class PrivacyPane : UserControl
    {
        public PrivacyPane()
        {
            this.InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Parent.GetType() == typeof(Popup))
            {
                ((Popup)this.Parent).IsOpen = false;
            }

            if (ApplicationView.Value != ApplicationViewState.Snapped)
            {
                SettingsPane.Show();
            }
        }
    }
}
