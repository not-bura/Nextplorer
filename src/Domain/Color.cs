using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Nextplorer.Domain
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
    public readonly record struct Color
        : IEquatable<Color>
    {
        private readonly byte m_r;
        private readonly byte m_g;
        private readonly byte m_b;
        private readonly byte m_a;

        public byte R
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_r;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            init => m_r = value;
        }

        public byte G
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_g;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            init => m_g = value;
        }

        public byte B
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_b;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            init => m_b = value;
        }

        public byte A
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_a;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            init => m_a = value;
        }

        private Color(byte r, byte g, byte b, byte a)
        {
            m_r = r;
            m_g = g;
            m_b = b;
            m_a = a;
        }

        #region static

        public static Color FromRGB(byte r, byte g, byte b)
        {
            return new(r, g, b, byte.MaxValue);
        }

        public static Color FromRGB(ReadOnlySpan<char> source)
        {
            return new(
                HexToByte(source[0], source[1]),
                HexToByte(source[2], source[3]),
                HexToByte(source[4], source[5]),
                byte.MaxValue
            );
        }

        public static Color FromRGBA(byte r, byte g, byte b, byte a)
        {
            return new(r, g, b, a);
        }

        public static Color FromRGBA(ReadOnlySpan<char> source)
        {
            return new(
                HexToByte(source[0], source[1]),
                HexToByte(source[2], source[3]),
                HexToByte(source[4], source[5]),
                HexToByte(source[6], source[7])
            );
        }

        public static Color FromARGB(ReadOnlySpan<char> source)
        {
            return new(
                HexToByte(source[2], source[3]),
                HexToByte(source[4], source[5]),
                HexToByte(source[6], source[7]),
                HexToByte(source[0], source[1])
            );
        }

        public static Color FromHSV(uint h, byte s, byte v)
        {
            if (s == 0)
            {
                var _value = To255(s);
                return new(_value, _value, _value, byte.MaxValue);
            }

            uint region = h / 60;
            uint remainder = (h - region * 60) * 255 / 60;

            uint p = (uint)v * (255U - s) / 255U;
            uint q = (uint)v * (255 - (s * remainder / 255)) / 255;
            uint t = (uint)v * (255 - (s * (255 - remainder) / 255)) / 255;

            return region switch
            {
                0 => new(v, (byte)t, (byte)p, byte.MaxValue),
                1 => new((byte)q, v, (byte)p, byte.MaxValue),
                2 => new((byte)p, v, (byte)t, byte.MaxValue),
                3 => new((byte)p, (byte)q, v, byte.MaxValue),
                4 => new((byte)t, (byte)p, v, byte.MaxValue),
                _ => new(v, (byte)p, (byte)q, byte.MaxValue),
            };

            static byte To255(byte source)
            {
                return source >= 100
                    ? byte.MaxValue
                    : (byte)(source / 100.0 * 255);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte HexToByte(char high, char low)
        {
            return (byte)((HexToNibble(high) << 4) | HexToNibble(low));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint HexToNibble(char source)
        {
            return source <= '9'
                ? (uint)(source - '0')
                : (uint)((source & 0xF) + 9);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char NibbleToHex(int source)
        {
            return source <= 9
                ? (char)('0' + source)
                : (char)('A' + (source - 10));
        }

        #endregion static

        public bool Equals(Color other)
        {
            return other.m_r == m_r
                && other.m_g == m_g
                && other.m_b == m_b
                && other.m_a == m_a;
        }

        public override int GetHashCode()
        {
            return (int)ToUInt();
        }

        public uint ToUInt()
        {
            return  (uint)m_r << (8 * 3)
                |   (uint)m_g << (8 * 2)
                |   (uint)m_b << (8 * 1)
                |   (uint)m_a;
        }

        public string ToHexRGB()
        {
            var _destination = (stackalloc char[6]);
            WriteToRGB(_destination);
            return _destination.ToString();
        }

        public string ToHexRGBA()
        {
            var _destination = (stackalloc char[8]);
            WriteToRGBA(_destination);
            return _destination.ToString();
        }

        public void WriteToRGB(Span<char> destination)
        {
            destination[0] = NibbleToHex(m_r >> 4);
            destination[1] = NibbleToHex(0xF & m_r);
            destination[2] = NibbleToHex(m_g >> 4);
            destination[3] = NibbleToHex(0xF & m_g);
            destination[4] = NibbleToHex(m_b >> 4);
            destination[5] = NibbleToHex(0xF & m_b);
        }

        public void WriteToRGBA(Span<char> destination)
        {
            WriteToRGB(destination);
            destination[6] = NibbleToHex(m_a >> 4);
            destination[7] = NibbleToHex(0xF & m_a);
        }

        public static implicit operator System.Windows.Media.Color(Color self)
        {
            return System.Windows.Media.Color.FromArgb(self.m_a, self.m_r, self.m_g, self.m_b);
        }

        public static implicit operator Color(System.Windows.Media.Color other)
        {
            return new(other.R, other.G, other.B, other.A);
        }
    }
}
