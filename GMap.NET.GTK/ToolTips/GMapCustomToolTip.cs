namespace GMap.NET.GTK.ToolTips
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Runtime.Serialization;

    [Serializable]
    public class GMapCustomToolTip : GMapToolTip, ISerializable
    {
        public float Radius = 10f;

        public static readonly Pen DefaultToolTipStroke = new Pen(Color.FromArgb(140, Color.Navy));

        static GMapCustomToolTip()
        {
            DefaultToolTipStroke.Width = 3;
            DefaultToolTipStroke.LineJoin = LineJoin.Round;
            DefaultToolTipStroke.StartCap = LineCap.RoundAnchor;
        }

        public GMapCustomToolTip(Markers.GMapMarker marker)
         : base(marker)
      {
            Stroke = DefaultToolTipStroke;
            Fill = Brushes.Yellow;
        }

        public override void OnRender(Graphics g)
        {
            Size st = g.MeasureString(Marker.ToolTipText, Font).ToSize();
            Rectangle rect = new Rectangle(Marker.ToolTipPosition.X, Marker.ToolTipPosition.Y - st.Height, st.Width + TextPadding.Width, st.Height + TextPadding.Height);
            rect.Offset(Offset.X, Offset.Y);

            using (GraphicsPath objGP = new GraphicsPath())
            {
                objGP.AddLine(rect.X + 2 * Radius, rect.Y + rect.Height, rect.X + Radius, rect.Y + rect.Height + Radius);
                objGP.AddLine(rect.X + Radius, rect.Y + rect.Height + Radius, rect.X + Radius, rect.Y + rect.Height);

                objGP.AddArc(rect.X, rect.Y + rect.Height - (Radius * 2), Radius * 2, Radius * 2, 90, 90);
                objGP.AddLine(rect.X, rect.Y + rect.Height - (Radius * 2), rect.X, rect.Y + Radius);
                objGP.AddArc(rect.X, rect.Y, Radius * 2, Radius * 2, 180, 90);
                objGP.AddLine(rect.X + Radius, rect.Y, rect.X + rect.Width - (Radius * 2), rect.Y);
                objGP.AddArc(rect.X + rect.Width - (Radius * 2), rect.Y, Radius * 2, Radius * 2, 270, 90);
                objGP.AddLine(rect.X + rect.Width, rect.Y + Radius, rect.X + rect.Width, rect.Y + rect.Height - (Radius * 2));
                objGP.AddArc(rect.X + rect.Width - (Radius * 2), rect.Y + rect.Height - (Radius * 2), Radius * 2, Radius * 2, 0, 90); // Corner

                objGP.CloseFigure();

                g.FillPath(Fill, objGP);
                g.DrawPath(Stroke, objGP);
            }

            g.DrawString(Marker.ToolTipText, Font, Foreground, rect, Format);
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Radius", Radius);

            base.GetObjectData(info, context);
        }

        protected GMapCustomToolTip(SerializationInfo info, StreamingContext context)
         : base(info, context)
        {
            Radius = Extensions.GetStruct(info, "Radius", 10f);
        }
    }
}