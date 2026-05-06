namespace Nextplorer.Domain
{
    public readonly struct Color
    {
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }
        public byte A { get; }

        private Color(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static Color FromRGB(byte r, byte g, byte b)
        {
            return new(r, g, b, byte.MaxValue);
        }

        public static Color FromRGBA(byte r, byte g, byte b, byte a)
        {
            return new(r, g, b, a);
        }

        public uint ToUInt()
        {
            var _result = 0U;
            _result |= (uint)R << (8 * 2);
            _result |= (uint)G << (8 * 1);
            _result |= (uint)B << (8 * 0);

            return _result;
        }

        public static implicit operator System.Windows.Media.Color(Color self)
        {
            return System.Windows.Media.Color.FromArgb(self.A, self.R, self.G, self.B);
        }

        public static implicit operator Color(System.Windows.Media.Color other)
        {
            return new(other.R, other.G, other.B, other.A);
        }
    }
}
