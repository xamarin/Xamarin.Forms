using GMap.NET.GTK.Markers;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace GMap.NET.GTK.ToolTips
{
    [Serializable]
    public class GMapToolTip : ISerializable, IDisposable
    {
        private bool _disposed = false;
        private GMapMarker _marker;
        public Point Offset;

        static GMapToolTip()
        {
            DefaultStroke.Width = 2;
            DefaultStroke.LineJoin = LineJoin.Round;
            DefaultStroke.StartCap = LineCap.RoundAnchor;
            DefaultFormat.LineAlignment = StringAlignment.Center;
            DefaultFormat.Alignment = StringAlignment.Center;
        }

        public GMapToolTip(GMapMarker marker)
        {
            this.Marker = marker;
            this.Offset = new Point(14, -44);
        }

        public GMapMarker Marker
        {
            get
            {
                return _marker;
            }
            internal set
            {
                _marker = value;
            }
        }

        public static readonly StringFormat DefaultFormat = new StringFormat();

        [NonSerialized]
        public readonly StringFormat Format = DefaultFormat;

        public static readonly Font DefaultFont = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold, GraphicsUnit.Pixel);

        [NonSerialized]
        public Font Font = DefaultFont;

        public static readonly Pen DefaultStroke = new Pen(Color.FromArgb(140, Color.MidnightBlue));

        [NonSerialized]
        public Pen Stroke = DefaultStroke;

        public static readonly Brush DefaultFill = new SolidBrush(Color.FromArgb(222, Color.AliceBlue));

        [NonSerialized]
        public Brush Fill = DefaultFill;

        public static readonly Brush DefaultForeground = new SolidBrush(Color.Navy);

        [NonSerialized]
        public Brush Foreground = DefaultForeground;

        public Size TextPadding = new Size(10, 10);

        public virtual void OnRender(Graphics g)
        {
            Size st = g.MeasureString(Marker.ToolTipText, Font).ToSize();
            Rectangle rect = new Rectangle(Marker.ToolTipPosition.X, Marker.ToolTipPosition.Y - st.Height, st.Width + TextPadding.Width, st.Height + TextPadding.Height);
            rect.Offset(Offset.X, Offset.Y);

            g.DrawLine(Stroke, Marker.ToolTipPosition.X, Marker.ToolTipPosition.Y, rect.X, rect.Y + rect.Height / 2);

            g.FillRectangle(Fill, rect);
            g.DrawRectangle(Stroke, rect);
            g.DrawString(Marker.ToolTipText, Font, Foreground, rect, Format);
        }


        protected GMapToolTip(SerializationInfo info, StreamingContext context)
        {
            Offset = Extensions.GetStruct(info, "Offset", Point.Empty);
            TextPadding = Extensions.GetStruct(info, "TextPadding", new Size(10, 10));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Offset", this.Offset);
            info.AddValue("TextPadding", this.TextPadding);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }
}