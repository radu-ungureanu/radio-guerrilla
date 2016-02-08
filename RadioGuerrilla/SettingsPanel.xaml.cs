using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.ApplicationSettings;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace RadioGuerrilla
{
    public sealed partial class SettingsPanel : UserControl, INotifyPropertyChanged
    {
        public SettingsPanel()
        {
            this.InitializeComponent();
            this.DataContext = this;

            var _settings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            if (_settings.Values.ContainsKey("value"))
            {
                SliderValue = (double)_settings.Values["value"];
            }
            else
            {
                SliderValue = 10;
            }
        }

        private double _sliderValue;
        public double SliderValue
        {
            get { return _sliderValue; }
            set
            {
                SetProperty(ref _sliderValue, value);
            }
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

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            MainPage.Current.Volume.Angle = SliderValue / 100 * 360;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var settings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            if (settings.Values.ContainsKey("value"))
            {
                settings.Values.Remove("value");
            }
            settings.Values.Add("value", SliderValue);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (!object.Equals(storage, value))
            {
                storage = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
}
