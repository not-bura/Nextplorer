using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Nextplorer.Infrastructure
{
    public interface ITheme
    {
        public Color Value { get; set; }
    }

    public sealed class SolidColorBrushTheme
        : ITheme
    {
        public Color Value
        {
            get;
            set
            {
                field = value;
                m_instance.Color = value;
            }
        }

        private SolidColorBrush m_instance;

        public SolidColorBrushTheme(SolidColorBrush instance)
        {
            m_instance = instance;
        }
    }

    public sealed class DropShadowTheme
        : ITheme
    {
        public Color Value
        {
            get;
            set
            {
                field = value;

            }
        }

        private DropShadowEffect m_instance;

        public DropShadowTheme(DropShadowEffect instance)
        {
            m_instance = instance;
        }
    }

    public static class ThemeModel
    {
        private static Palette m_current = Palette.Dark;
        public static Palette Current
        {
            get => m_current;
            set
            {
                m_current = value;
                Apply(value);
            }
        }

        public static void Apply(Palette palette)
        {
            WindowHeader.Color = palette.WindowHeader;
            WindowContent.Color = palette.WindowContent;
            Text.Color = palette.Text;
        }

        public static SolidColorBrush WindowHeader { get; }
        public static SolidColorBrush WindowContent { get; }
        public static SolidColorBrush Text { get; }

        static ThemeModel()
        {
            var _current = m_current;

            WindowHeader = new SolidColorBrush(_current.WindowHeader);
            WindowContent = new SolidColorBrush(_current.WindowContent);
            Text = new SolidColorBrush(_current.Text);
        }

        public sealed class Palette
        {
            public Color WindowHeader { get; private set; }
            public Color WindowContent { get; private set; }
            public Color Text { get; private set; }

            public static Palette Dark = new()
            {
                WindowHeader = Color.FromRgb(0x1E, 0x1E, 0x1E),
                WindowContent = Color.FromRgb(0x17, 0x17, 0x17),
                Text = Colors.WhiteSmoke,
            };

            public static Palette Light = new()
            {
                WindowHeader = Color.FromRgb(0xF0, 0xF0, 0xF0),
                WindowContent = Color.FromRgb(0xFF, 0xFF, 0xFF),
                Text = Colors.Black,
            };
        }
    }
}
