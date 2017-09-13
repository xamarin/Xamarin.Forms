using System;
using System.Runtime.Serialization;
using System.Drawing;
using GMap.NET.GTK.ToolTips;

namespace GMap.NET.GTK.Markers
{
    [Serializable]
    public class GMapMarker : ISerializable, IDisposable
    {
        private bool _disposed = false;
        private GMapOverlay _overlay;
        private PointLatLng _position;
        private Point _offset;
        private Rectangle _area;
        private bool _visible = true;
        private string _toolTipText;

        public object Tag;

        public GMapMarker(PointLatLng pos)
        {
            Position = pos;
        }

        public GMapOverlay Overlay
        {
            get
            {
                return _overlay;
            }
            internal set
            {
                _overlay = value;
            }
        }

        public PointLatLng Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (_position != value)
                {
                    _position = value;

                    if (IsVisible)
                    {
                        if (Overlay != null && Overlay.Control != null)
                        {
                            Overlay.Control.UpdateMarkerLocalPosition(this);
                        }
                    }
                }
            }
        }

        public Point Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                if (_offset != value)
                {
                    _offset = value;

                    if (IsVisible)
                    {
                        if (Overlay != null && Overlay.Control != null)
                        {
                            Overlay.Control.UpdateMarkerLocalPosition(this);
                        }
                    }
                }
            }
        }

        public Point LocalPosition
        {
            get
            {
                return _area.Location;
            }
            set
            {
                if (_area.Location != value)
                {
                    _area.Location = value;
                    {
                        if (Overlay != null && Overlay.Control != null)
                        {
                            if (!Overlay.Control.HoldInvalidation)
                            {
                                Overlay.Control.Invalidate();
                            }
                        }
                    }
                }
            }
        }

        public Point ToolTipPosition
        {
            get
            {
                Point ret = _area.Location;
                ret.Offset(-Offset.X, -Offset.Y);
                return ret;
            }
        }

        public Size Size
        {
            get
            {
                return _area.Size;
            }
            set
            {
                _area.Size = value;
            }
        }

        public Rectangle LocalArea
        {
            get
            {
                return _area;
            }
        }

        public GMapToolTip ToolTip;

        public MarkerTooltipMode ToolTipMode = MarkerTooltipMode.OnMouseOver;

        public string ToolTipText
        {
            get
            {
                return _toolTipText;
            }

            set
            {
                if (ToolTip == null && !string.IsNullOrEmpty(value))
                {
                    ToolTip = new GMapCustomToolTip(this);
                }

                _toolTipText = value;
            }
        }

        public bool IsVisible
        {
            get
            {
                return _visible;
            }
            set
            {
                if (value != _visible)
                {
                    _visible = value;

                    if (Overlay != null && Overlay.Control != null)
                    {
                        if (_visible)
                        {
                            Overlay.Control.UpdateMarkerLocalPosition(this);
                        }
                        else
                        {
                            if (Overlay.Control.IsMouseOverMarker)
                            {
                                Overlay.Control.IsMouseOverMarker = false;
                                Overlay.Control.RestoreCursorOnLeave();
                            }
                        }

                        {
                            if (!Overlay.Control.HoldInvalidation)
                            {
                                Overlay.Control.Invalidate();
                            }
                        }
                    }
                }
            }
        }

        public bool DisableRegionCheck = false;

        public bool IsHitTestVisible = true;

        private bool isMouseOver = false;

        public bool IsMouseOver
        {
            get
            {
                return isMouseOver;
            }
            internal set
            {
                isMouseOver = value;
            }
        }

        public virtual void OnRender(Graphics g)
        {

        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Position", Position);
            info.AddValue("Tag", Tag);
            info.AddValue("Offset", Offset);
            info.AddValue("Area", _area);
            info.AddValue("ToolTip", ToolTip);
            info.AddValue("ToolTipMode", ToolTipMode);
            info.AddValue("ToolTipText", ToolTipText);
            info.AddValue("Visible", IsVisible);
            info.AddValue("DisableregionCheck", DisableRegionCheck);
            info.AddValue("IsHitTestVisible", IsHitTestVisible);
        }

        protected GMapMarker(SerializationInfo info, StreamingContext context)
        {
            Position = Extensions.GetStruct(info, "Position", PointLatLng.Empty);
            Tag = Extensions.GetValue<object>(info, "Tag", null);
            Offset = Extensions.GetStruct(info, "Offset", Point.Empty);
            _area = Extensions.GetStruct(info, "Area", Rectangle.Empty);

            ToolTip = Extensions.GetValue<GMapToolTip>(info, "ToolTip", null);
            if (ToolTip != null) ToolTip.Marker = this;

            ToolTipMode = Extensions.GetStruct(info, "ToolTipMode", MarkerTooltipMode.OnMouseOver);
            ToolTipText = info.GetString("ToolTipText");
            IsVisible = info.GetBoolean("Visible");
            DisableRegionCheck = info.GetBoolean("DisableregionCheck");
            IsHitTestVisible = info.GetBoolean("IsHitTestVisible");
        }

        public virtual void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                Tag = null;

                if (ToolTip != null)
                {
                    _toolTipText = null;
                    ToolTip.Dispose();
                    ToolTip = null;
                }
            }
        }
    }

    public delegate void MarkerEnter(GMapMarker item);

    public delegate void MarkerLeave(GMapMarker item);

    public delegate void MarkerClick(GMapMarker item);
}