
using System;
namespace GMap.NET.Internals
{
    /// <summary>
    /// struct for drawing tile
    /// </summary>
    public struct DrawTile : IEquatable<DrawTile>, IComparable<DrawTile>
    {
        public GPoint PosXY;
        public GPoint PosPixel;
        public double DistanceSqr;

        public override string ToString()
        {
            return PosXY + ", px: " + PosPixel;
        }

        #region IEquatable<DrawTile> Members

        public bool Equals(DrawTile other)
        {
            return (PosXY == other.PosXY);
        }

        #endregion

        #region IComparable<DrawTile> Members

        public int CompareTo(DrawTile other)
        {
            return other.DistanceSqr.CompareTo(DistanceSqr);
        }

        #endregion
    }
}
