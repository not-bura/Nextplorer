using System.Windows.Media;
using Color = Nextplorer.Domain.Color;

namespace Nextplorer.Presentation
{
    public static class ColorPickerTheme
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

        public static SolidColorBrush Background { get; }

        static ColorPickerTheme()
        {
            var _current = m_current;

            Background = new SolidColorBrush(_current.Background);
        }

        public static void Apply(Palette source)
        {
            Background.Color = source.Background;
        }

        public sealed class Palette
        {
            public required Color Background { get; init; }

            public static Palette Dark { get; } = new()
            {
                Background = Color.FromRGB(0x1E, 0x1E, 0x1E),
            };

            public static Palette Light { get; } = new()
            {
                Background = Color.FromRGB(0xF0, 0xF0, 0xF0),
            };
        }
    }
}
