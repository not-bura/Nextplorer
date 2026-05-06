using MessagePack;
using MessagePack.Formatters;
using System.Windows;

namespace Nextplorer.Domain
{
    [MessagePackFormatter(typeof(Vector2))]
    public sealed class Vector2Formatter
        : IMessagePackFormatter<Vector2>
    {
        public void Serialize(ref MessagePackWriter writer, Vector2 value, MessagePackSerializerOptions options)
        {
            writer.Write(value.X);
            writer.Write(value.Y);
        }

        public Vector2 Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var _x = reader.ReadDouble();
            var _y = reader.ReadDouble();
            return new(_x, _y);
        }
    }

    public readonly struct Vector2
        : IFormattable
    {
        public double X { get; }
        public double Y { get; }

        public Vector2(double value)
        {
            X = value;
            Y = value;
        }

        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        #region operator

        public static Vector2 operator +(Vector2 lhs, Vector2 rhs)
        {
            return new(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }

        public static Vector2 operator -(Vector2 lhs, Vector2 rhs)
        {
            return new(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }

        public static Vector2 operator *(Vector2 lhs, double rhs)
        {
            return new(lhs.X * rhs, lhs.Y * rhs);
        }

        public static Vector2 operator /(Vector2 lhs, double rhs)
        {
            return new(lhs.X  / rhs, lhs.Y / rhs);
        }

        public static Vector2 operator ~(Vector2 self)
        {
            return new(-self.X, -self.Y);
        }

        public static implicit operator Point(Vector2 self)
        {
            return new(self.X, self.Y);
        }

        #endregion operator

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}";
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return $"X: {X.ToString(format, formatProvider)}, Y: {Y.ToString(format, formatProvider)}";
        }
    }
}
