namespace GMap.NET
{
    using System;
    using System.IO;

    /// <summary>
    /// image abstraction proxy
    /// </summary>
    public abstract class PureImageProxy
    {
        abstract public PureImage FromStream(Stream stream);
        abstract public bool Save(Stream stream, PureImage image);

        public PureImage FromArray(byte[] data)
        {
            MemoryStream m = new MemoryStream(data, 0, data.Length, false, true);
            var pi = FromStream(m);
            if (pi != null)
            {
                m.Position = 0;
                pi.Data = m;
            }
            else
            {
                m.Dispose();
            }
            m = null;

            return pi;
        }
    }

    /// <summary>
    /// image abstraction
    /// </summary>
    public abstract class PureImage : IDisposable
    {
        public MemoryStream Data;

        public bool IsParent;
        public long Ix;
        public long Xoff;
        public long Yoff;

        #region IDisposable Members

        public abstract void Dispose();

        #endregion
    }
}