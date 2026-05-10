using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Bitmap = System.Drawing.Bitmap;
using Graphics = System.Drawing.Graphics;

namespace Nextplorer.Presentation
{
    public sealed class ColorPicker
        : IDisposable
    {
        // Win32 APIでカーソルの絶対座標を取得するための構造体
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT { public int X; public int Y; }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        public static Color GetCursorNeighborColor()
        {
            GetCursorPos(out var p);

            // 10x10の範囲を取得 (カーソルを中央にするため-5)
            int size = 10;
            using (var bmp = new Bitmap(size, size))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    // 画面からBitmapへコピー
                    g.CopyFromScreen(p.X - 5, p.Y - 5, 0, 0, new(size, size));
                }

                // 例として中央の1ピクセルを取得
                // (全ピクセルの平均を取りたい場合は、ここでループして計算)
                var pixelColor = bmp.GetPixel(5, 5);

                return Color.FromArgb(pixelColor.A, pixelColor.R, pixelColor.G, pixelColor.B);
            }
        }

        private Color m_color;
        private bool m_spoit;

        private Canvas m_root;
        private Rectangle m_colorGraphic;
        private Rectangle m_alphaGraphic;
        private Button m_spoitButton;

        private SolidColorBrush m_colorBrush;

        public Canvas Root => m_root;

        public Color Color
        {
            get => m_color;
            set
            {
                m_color = value;
                m_colorBrush.Color = value;

                m_alphaGraphic.Width = (value.A / byte.MaxValue) * m_colorGraphic.Width;
            }
        }

        public ColorPicker(double width, double height, Color color)
        {
            var _root = m_root = new()
            {
                Width = width,
                Height = height,
            };

            var _brush = m_colorBrush = new SolidColorBrush(color);

            var _buttonSize = height;
            var _alphaHeight = height * 0.2;

            var _colorGraphic = m_colorGraphic = new Rectangle
            {
                Width = width - _buttonSize,
                Height = height - _alphaHeight,
                Fill = _brush,
            };

            var _alphaGraphic = m_alphaGraphic = new Rectangle
            {
                Width = width - _buttonSize,
                Height = _alphaHeight,
                Fill = Brushes.White,
            };
            Canvas.SetBottom(_alphaGraphic, 0.0);

            var _spoitButton = m_spoitButton = new Button
            {
                Width = _buttonSize,
                Height = height,
            };
            Canvas.SetRight(_spoitButton, 0.0);

            var _children = m_root.Children;
            _children.Add(_colorGraphic);
            _children.Add(_alphaGraphic);
            _children.Add(_spoitButton);

            _spoitButton.PreviewMouseLeftButtonUp += (_, _) =>
            {
                m_spoit = !m_spoit;
                if (m_spoit)
                {
                    EnableCapture();
                }
                else
                {
                    DisableCapture();
                }
            };
        }

        public void Dispose()
        {

        }

        private void EnableCapture()
        {
            m_root.CaptureMouse();
            m_root.MouseMove += OnMove;
            m_root.MouseLeftButtonDown += (_, _) =>
            {
                DisableCapture();

                var _color = GetCursorNeighborColor();
                Color = _color;
            };
        }

        private void DisableCapture()
        {
            m_root.ReleaseMouseCapture();
            m_root.MouseMove -= OnMove;
            m_spoit = false;
        }

        private void OnMove(object sender, MouseEventArgs e)
        {
            var _color = GetCursorNeighborColor();
            m_colorBrush.Color = _color;
        }

        // TODO: Buttonのようなコントロールを実装する
    }
}
