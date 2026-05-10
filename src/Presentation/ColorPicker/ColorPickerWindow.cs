using Nextplorer.Domain;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Shell;
using Color = Nextplorer.Domain.Color;

namespace Nextplorer.Presentation
{
    public sealed class ColorPickerWindow
    {
        const double SCALE = 220.0;

        private Window m_window;

        private Color m_current;

        private TextBox m_colorCode;
        private Slider redSlider;
        private Slider greenSlider;
        private Slider blueSlider;

        private Slider m_hueSlider;
        private Slider m_saturationSlider;
        private Slider m_brightnessSlider;

        private Action<Color> m_callback;

        private ColorPickerWindow(Action<Color> callback)
        {
            var _window = m_window = new Window
            {
                Topmost = true,
                ResizeMode = ResizeMode.NoResize,
                Width = 240,
                Height = 500,
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

            var _canvas = new Canvas
            {
                Width = _window.Width,
                Height = _window.Height,
                Background = ColorPickerTheme.Background,
            };
            _canvas.Children.Add(new Rectangle{ Fill = Brushes.Gray, Width = _window.Width, Height = _window.Height, });
            Triangle(_canvas);

            m_colorCode = new()
            {
                Text = m_current.ToHexRGBA(),
            };
            Canvas.SetLeft(m_colorCode, 20);
            Canvas.SetTop(m_colorCode, 250);

            redSlider = Create(20, 300);
            greenSlider = Create(20, 350);
            blueSlider = Create(20, 400);

            Slider Create(double x, double y)
            {
                var _result = new Slider
                {
                    Width = 200.0,
                    Minimum = 0,
                    Maximum = 255,
                    Value = 255,
                    BorderThickness = new(1.0),
                };

                Canvas.SetLeft(_result, x);
                Canvas.SetTop(_result, y);

                return _result;
            }

            redSlider.ValueChanged += (_, e) =>
            {
                ref var _color = ref m_current;
                _color = _color with { R = (byte)e.NewValue };
                m_colorCode.Text = _color.ToHexRGBA();
            };
            greenSlider.ValueChanged += (_, e) =>
            {
                ref var _color = ref m_current;
                _color = _color with { G = (byte)e.NewValue };
                m_colorCode.Text = _color.ToHexRGBA();
            };
            blueSlider.ValueChanged += (_, e) =>
            {
                ref var _color = ref m_current;
                _color = _color with { B = (byte)e.NewValue };
                m_colorCode.Text = _color.ToHexRGBA();
            };

            _canvas.Children.Add(m_colorCode);
            _canvas.Children.Add(redSlider);
            _canvas.Children.Add(greenSlider);
            _canvas.Children.Add(blueSlider);

            _window.Content = _canvas;
        }

        private void Triangle(Canvas parent)
        {
            const double SCALE_HALF = SCALE / 2.0;

            var _size = SCALE / 3.0;

            var _side = Math.Sqrt(3) * _size;
            var _height = 1.5 * _size;

            var _triangle = new Rectangle
            {
                Width = _height,
                Height = _side,
                Effect = ColorPickerShaderEffect.Triangle(),
                Fill = Brushes.White,
            };
            var _center = new Vector2(SCALE_HALF - (_height / 3.0), SCALE_HALF - (_side / 2.0));
            var _offset = _center;
            Canvas.SetLeft(_triangle, _offset.X);
            Canvas.SetTop(_triangle, _offset.Y);

            var _ring = new Rectangle
            {
                Width = SCALE,
                Height = SCALE,
                Effect = ColorPickerShaderEffect.ColorRing(),
                Fill = Brushes.White,
            };

            var _selector = new Ellipse
            {
                Width = 22.0, Height = 22.0,
                Stroke = Brushes.White,
                StrokeThickness = 2.0,
            };

            //UpdateRing(new(0, 1));

            var _enable = false;
            _ring.MouseLeftButtonDown += Down;
            _ring.MouseMove += Move;
            _ring.MouseLeftButtonUp += Up;
            
            void Down(object sender, MouseButtonEventArgs e)
            {
                _enable = true;
                var _pos = e.GetPosition(_ring);
                UpdateRing(_pos);
                Mouse.Capture(_ring);
            }
            void Move(object sender, MouseEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Released)
                {
                    _enable = false;
                    Mouse.Capture(null);
                }

                if (false == _enable)
                {
                    return;
                }

                var _pos = e.GetPosition(_ring);
                UpdateRing(_pos);
            }
            void Up(object sender, MouseButtonEventArgs e)
            {
                if (false == _enable)
                {
                    return;
                }

                var _pos = e.GetPosition(_ring);
                UpdateRing(_pos);

                _enable = false;
                Mouse.Capture(null);
            }
            void UpdateRing(Point point)
            {
                var _center = new Point(_ring.Width / 2.0, _ring.Height / 2.0);

                var _vector = point - _center;
                var _radian = Math.Atan2(_vector.Y, _vector.X);
                var _degree = _radian * (180.0 / Math.PI);

                if (_degree < 0)
                {
                    _degree += 360.0;
                }

                var _c = Color.FromHSV((uint)_degree, 1, 1);
                m_colorCode.Text = _c.ToHexRGBA();

                var _normalize = _vector;
                _normalize.Normalize();

                var _pos = _normalize * (SCALE_HALF - 11.0) + new Point(SCALE_HALF - 11.0, SCALE_HALF - 11.0);
                Canvas.SetLeft(_selector, _pos.X);
                Canvas.SetTop(_selector, _pos.Y);
            }

            parent.Background = Brushes.White;

            var _children = parent.Children;
            _children.Add(_ring);
            _children.Add(_triangle);
            _children.Add(_selector);

            //var cp = new ColorPicker(200.0, 18.0, Colors.Red);
            //_children.Add(cp.Root);
            //Canvas.SetTop(cp.Root, 100);
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
            _window.ShowDialog();
            _window.Activate();

            //_window.Owner = owner;

            return _instance;
        }
    }
}
