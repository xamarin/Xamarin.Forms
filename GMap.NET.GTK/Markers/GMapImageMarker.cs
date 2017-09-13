namespace System.Windows.Forms.Markers
{
    using GMap.NET.GTK.Markers;
    using Collections.Generic;
    using Runtime.Serialization;
    using Drawing;
    using GMap.NET;
    using Properties;

    [Serializable]
    public class GMapImageMarker : GMapMarker, ISerializable, IDeserializationCallback
    {
        private Bitmap _bitmap;
        private Bitmap _bitmapShadow;
        private static Bitmap _pushpin_shadow;

        public readonly GMapImageMarkerType Type;

        public GMapImageMarker(PointLatLng p, GMapImageMarkerType type)
           : base(p)
        {
            Type = type;

            if (type != GMapImageMarkerType.None)
            {
                LoadBitmap();
            }
        }

        public GMapImageMarker(PointLatLng p, Bitmap Bitmap)
            : base(p)
        {
            _bitmap = Bitmap;
            Size = new Size(Bitmap.Width, Bitmap.Height);
            Offset = new Point(-Size.Width / 2, -Size.Height);
        }

        static readonly Dictionary<string, Bitmap> iconCache = new Dictionary<string, Bitmap>();

        void LoadBitmap()
        {
            _bitmap = GetIcon(Type.ToString());

            if(_bitmap == null)
            {
                return;
            }

            Size = new Size(_bitmap.Width, _bitmap.Height);

            switch (Type)
            {
                case GMapImageMarkerType.Red:
                case GMapImageMarkerType.RedDot:
                    {
                        Offset = new Point(-9, -Size.Height + 1);

                        if (_pushpin_shadow == null)
                        {
                            _pushpin_shadow = Resources.Shadow;
                        }

                        _bitmapShadow = _pushpin_shadow;
                    }
                    break;
            }
        }

        public static Bitmap GetIcon(string name)
        {
            Bitmap ret;
            if (!iconCache.TryGetValue(name, out ret))
            {
                ret = Resources.ResourceManager.GetObject(name, Resources.Culture) as Bitmap;
                iconCache.Add(name, ret);
            }
            return ret;
        }


        public override void OnRender(Graphics g)
        {
            if (_bitmap == null)
                return;

            if (_bitmapShadow != null)
            {
                g.DrawImage(_bitmapShadow, LocalPosition.X, LocalPosition.Y, _bitmapShadow.Width, _bitmapShadow.Height);
            }

            g.DrawImage(_bitmap, LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
        }

        public override void Dispose()
        {
            if (_bitmap != null)
            {
                if (!iconCache.ContainsValue(_bitmap))
                {
                    _bitmap.Dispose();
                    _bitmap = null;
                }
            }

            base.Dispose();
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("type", Type);

            base.GetObjectData(info, context);
        }

        protected GMapImageMarker(SerializationInfo info, StreamingContext context)
           : base(info, context)
        {
            Type = Extensions.GetStruct(info, "type", GMapImageMarkerType.None);
        }

        public void OnDeserialization(object sender)
        {
            if (Type != GMapImageMarkerType.None)
            {
                LoadBitmap();
            }
        }
    }
}