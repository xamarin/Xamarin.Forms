using System;
using System.ComponentModel;

namespace Xamarin.Forms
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct Vector
    {
        public Vector(double x, double y)
            : this()
        {
            X = x;
            Y = y;
        }

        public Vector(Point p)
            : this()
        {
            X = p.X;
            Y = p.Y;
        }

        public Vector(double angle)
            : this()
        {
            X = Math.Cos(Math.PI * angle / 180);
            Y = Math.Sin(Math.PI * angle / 180);
        }

        public double X { private set; get; }
        public double Y { private set; get; }

        public double LengthSquared
        {
            get { return X * X + Y * Y; }
        }

        public double Length
        {
            get { return Math.Sqrt(LengthSquared); }
        }

        public Vector Normalized
        {
            get
            {
                double length = Length;

                if (length != 0)
                {
                    return new Vector(X / length, Y / length);
                }
                return new Vector();
            }
        }

        public static double AngleBetween(Vector v1, Vector v2)
        {
            return 180 * (Math.Atan2(v2.Y, v2.X) - Math.Atan2(v1.Y, v1.X)) / Math.PI;
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Point operator +(Vector v, Point p)
        {
            return new Point(v.X + p.X, v.Y + p.Y);
        }

        public static Point operator +(Point p, Vector v)
        {
            return new Point(v.X + p.X, v.Y + p.Y);
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Point operator -(Point p, Vector v)
        {
            return new Point(p.X - v.X, p.Y - v.Y);
        }

        public static Vector operator *(Vector v, double d)
        {
            return new Vector(d * v.X, d * v.Y);
        }

        public static Vector operator *(double d, Vector v)
        {
            return new Vector(d * v.X, d * v.Y);
        }

        public static Vector operator /(Vector v, double d)
        {
            return new Vector(v.X / d, v.Y / d);
        }

        public static Vector operator -(Vector v)
        {
            return new Vector(-v.X, -v.Y);
        }

        public static explicit operator Point(Vector v)
        {
            return new Point(v.X, v.Y);
        }

        public override string ToString()
        {
            return string.Format("({0} {1})", X, Y);
        }
    }
}