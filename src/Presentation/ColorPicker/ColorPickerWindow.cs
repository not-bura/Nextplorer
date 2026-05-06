using Nextplorer.Domain;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Shell;
using Color = Nextplorer.Domain.Color;

namespace Nextplorer.Presentation
{
    public sealed class ColorPickerWindow
    {
        private Window m_window;

        private Color m_current;
        private Action<Color> m_callback;

        private ColorPickerWindow(Action<Color> callback)
        {
            var _window = m_window = new Window
            {
                Topmost = true,
                Width = 300,
                Height = 300,
                //WindowStyle = WindowStyle.None,
                Background = ColorPickerTheme.Background,
            };

            _window.BorderThickness = new(1.0);
            _window.BorderBrush = Brushes.White;

            var _chrome = new WindowChrome()
            {
                CaptionHeight = 40.0,
                ResizeBorderThickness = new(7.0),
            };
            WindowChrome.SetWindowChrome(_window, _chrome);

            m_current = Colors.Black;
            m_callback = callback;

            var _canvas = new Canvas();
            Triangle(_canvas);

            _window.Content = _canvas;
        }

        private void Triangle(Canvas parent)
        {
            var _size = 100.0;

            var _side = Math.Sqrt(3) * _size;
            var _height = 1.5 * _size;

            var _triangle = new Rectangle
            {
                Width = _height,
                Height = _side,
                Effect = ColorPickerShaderEffect.Triangle(),
                Fill = Brushes.White,
            };
            var _center = new Vector2(150.0 - (_height / 3.0), 150 - (_side / 2.0));
            var _offset = _center;
            Canvas.SetLeft(_triangle, _offset.X);
            Canvas.SetTop(_triangle, _offset.Y);

            var _ring = new Rectangle
            {
                Width = 300,
                Height = 300,
                Effect = ColorPickerShaderEffect.ColorRing(),
                Fill = Brushes.White,
            };

            parent.Background = Brushes.White;

            var _children = parent.Children;
            _children.Add(_ring);
            _children.Add(_triangle);
        }

        private Vector2 Circle(double degree)
        {
            var _radian = degree * Math.PI / 180.0;

            var (_sin, _cos) = Math.SinCos(_radian);
            return new(_cos, _sin);
        }

        public static ColorPickerWindow Show(Window owner, Action<Color> callback)
        {
            var _instance = new ColorPickerWindow(callback);

            var _window = _instance.m_window;
            _window.Show();
            _window.Activate();

            //_window.Owner = owner;

            return _instance;
        }
    }
}
