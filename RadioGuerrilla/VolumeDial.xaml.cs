using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace RadioGuerrilla
{
    public sealed partial class VolumeDial : UserControl, INotifyPropertyChanged
    {
        public VolumeDial()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        private void Grid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            this.Angle = GetAngle(e.Position, this.RenderSize);
        }

        private double _angle;
        public double Angle
        {
            get { return _angle; }
            set 
            {
                SetProperty(ref _angle, value);
                this.Amount = this.Angle / 360d;
            }
        }

        private double _amount;
        public double Amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        public enum Quadrants : int { nw = 2, ne = 1, sw = 4, se = 3 }
        private double GetAngle(Point touchPoint, Size circleSize)
        {
            var _X = touchPoint.X - (circleSize.Width / 2d);
            var _Y = circleSize.Height - touchPoint.Y - (circleSize.Height / 2d);
            var _Hypot = Math.Sqrt(_X * _X + _Y * _Y);
            var _Value = Math.Asin(_Y / _Hypot) * 180 / Math.PI;
            var _Quadrant = (_X >= 0) ?
                (_Y >= 0) ? Quadrants.ne : Quadrants.se :
                (_Y >= 0) ? Quadrants.nw : Quadrants.sw;
            switch (_Quadrant)
            {
                case Quadrants.ne: _Value = 090 - _Value; break;
                case Quadrants.nw: _Value = 270 + _Value; break;
                case Quadrants.se: _Value = 090 - _Value; break;
                case Quadrants.sw: _Value = 270 + _Value; break;
            }
            return _Value;
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
