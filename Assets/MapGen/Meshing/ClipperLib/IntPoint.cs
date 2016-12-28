using System;

namespace ClipperLib
{
    public struct IntPoint
    {
        public int X;
        public int Y;
#if use_xyz
        public int Z;

        public IntPoint(int x, int y, int z = 0)
        {
            this.X = x; this.Y = y; this.Z = z;
        }

        public IntPoint(double x, double y, double z = 0)
        {
            this.X = (int)x; this.Y = (int)y; this.Z = (int)z;
        }

        public IntPoint(DoublePoint dp)
        {
            this.X = (int)dp.X; this.Y = (int)dp.Y; this.Z = 0;
        }

        public IntPoint(IntPoint pt)
        {
            this.X = pt.X; this.Y = pt.Y; this.Z = pt.Z;
        }
#else
        public IntPoint(int X, int Y)
        {
            this.X = X; this.Y = Y;
        }
        public IntPoint(double x, double y)
        {
            this.X = (int)x; this.Y = (int)y;
        }

        public IntPoint(IntPoint pt)
        {
            this.X = pt.X; this.Y = pt.Y;
        }
#endif

        public static bool operator ==(IntPoint a, IntPoint b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(IntPoint a, IntPoint b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is IntPoint)
            {
                IntPoint a = (IntPoint)obj;
                return (X == a.X) && (Y == a.Y);
            }
            else return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}