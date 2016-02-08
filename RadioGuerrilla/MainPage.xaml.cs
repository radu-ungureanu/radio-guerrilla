using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Media;
using Windows.UI.Xaml;

namespace RadioGuerrilla
{
    public sealed partial class MainPage : RadioGuerrilla.Common.LayoutAwarePage, INotifyPropertyChanged
    {
        private DispatcherTimer _timer;
        private bool _isPlaying;

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    OnChanged("IsPlaying");
                }
            }
        }
        public static MainPage Current;
        public VolumeDial Volume;

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = this;

            MediaControl.PlayPressed += MediaControl_PlayPressed;
            MediaControl.PausePressed += MediaControl_PausePressed;
            MediaControl.PlayPauseTogglePressed += MediaControl_PlayPauseTogglePressed;
            MediaControl.StopPressed += MediaControl_StopPressed;
            MediaControl.FastForwardPressed += MediaControl_FastForwardPressed;
            MediaControl.RewindPressed += MediaControl_RewindPressed;
            MediaControl.ChannelDownPressed += MediaControl_ChannelDownPressed;
            MediaControl.ChannelUpPressed += MediaControl_ChannelUpPressed;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMinutes(1);
            _timer.Tick += PeriodicInfoDownload;

            Current = this;
            Volume = volumeDial;
        }

        private void StartPlaying()
        {
            IsPlaying = true;
            mediaplayer.Source = new Uri("http://live.eliberadio.ro:8010/eliberadio-32.aac");
            mediaplayer.Play();
            _timer.Start();

            PeriodicInfoDownload(null, null);
        }

        private void StopPlaying()
        {
            IsPlaying = false;
            mediaplayer.Stop();
            mediaplayer.Source = null;
            _timer.Stop();

            radioInfo.Text = "Oprit";
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var _settings = Windows.Storage.ApplicationData.Current.RoamingSettings;

            if (_settings.Values.ContainsKey("value"))
            {
                var _value = (double)_settings.Values["value"];
                volumeDial.Angle = _value / 100 * 360;
            }
            else
            {
                volumeDial.Angle = 25;
            }
        }

        private async void MediaControl_PlayPauseTogglePressed(object sender, object e)
        {
            if (IsPlaying)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    StopPlaying();
                });
            }
            else
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    StartPlaying();
                });
            }
        }

        private void playBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IsPlaying)
            {
                StopPlaying();
            }
            else
            {
                StartPlaying();
            }
        }

        private async void PeriodicInfoDownload(object sender, object e)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:21.0) Gecko/20100101 Firefox/21.0");
            httpClient.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate");

            try
            {
                var response = await httpClient.GetAsync("http://live.eliberadio.ro:8000/index.html");

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var nowPlaying = result.Substring(result.LastIndexOf("class=default"));
                    nowPlaying = nowPlaying.Substring(nowPlaying.IndexOf("<b") + 3);
                    nowPlaying = nowPlaying.Substring(0, nowPlaying.IndexOf("</"));
                    nowPlaying = nowPlaying.Replace("�", "\'").Replace("_", ".");

                    radioInfo.Text = nowPlaying;
                }
            }
            catch (Exception) { }
        }

        private async void MediaControl_PausePressed(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                StopPlaying();
            });
        }

        private async void MediaControl_PlayPressed(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                StartPlaying();
            });
        }

        private async void MediaControl_StopPressed(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                StopPlaying();
            });
        }

        private void MediaControl_ChannelUpPressed(object sender, object e) { }
        private void MediaControl_ChannelDownPressed(object sender, object e) { }
        private void MediaControl_RewindPressed(object sender, object e) { }
        private void MediaControl_FastForwardPressed(object sender, object e) { }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

