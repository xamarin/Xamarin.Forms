using GMap.NET.GTK.Markers;
using System;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using GMap.NET.GTK.Helpers;
using System.Drawing;

namespace GMap.NET.GTK
{
    [Serializable]
    public class GMapOverlay : ISerializable, IDeserializationCallback, IDisposable
    {
        private bool _isVisibile = true;
        private bool _disposed = false;
        private GMapControl _control;

        public string Id;

        public bool IsVisibile
        {
            get
            {
                return _isVisibile;
            }
            set
            {
                if (value != _isVisibile)
                {
                    _isVisibile = value;

                    if (Control != null)
                    {
                        if (_isVisibile)
                        {
                            Control.HoldInvalidation = true;
                            {
                                ForceUpdate();
                            }

                            Control.Refresh();
                        }
                        else
                        {
                            if (Control.IsMouseOverMarker)
                            {
                                Control.IsMouseOverMarker = false;
                            }

                            if (Control.IsMouseOverPolygon)
                            {
                                Control.IsMouseOverPolygon = false;
                            }

                            if (Control.IsMouseOverRoute)
                            {
                                Control.IsMouseOverRoute = false;
                            }

                            Control.RestoreCursorOnLeave();

                            if (!Control.HoldInvalidation)
                            {
                                Control.Invalidate();
                            }
                        }
                    }
                }
            }
        }

        public readonly ObservableCollectionThreadSafe<GMapMarker> Markers = 
            new ObservableCollectionThreadSafe<GMapMarker>();

        public GMapControl Control
        {
            get
            {
                return _control;
            }
            internal set
            {
                _control = value;
            }
        }

        public GMapOverlay()
        {
            CreateEvents();
        }

        public GMapOverlay(string id)
        {
            Id = id;
            CreateEvents();
        }

        void CreateEvents()
        {
            Markers.CollectionChanged += new NotifyCollectionChangedEventHandler(Markers_CollectionChanged);
        }

        void ClearEvents()
        {
            Markers.CollectionChanged -= new NotifyCollectionChangedEventHandler(Markers_CollectionChanged);
        }

        public void Clear()
        {
            Markers.Clear();
        }

        void Markers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GMapMarker obj in e.NewItems)
                {
                    if (obj != null)
                    {
                        obj.Overlay = this;
                        if (Control != null)
                        {
                            Control.UpdateMarkerLocalPosition(obj);
                        }
                    }
                }
            }

            if (Control != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
                {
                    if (Control.IsMouseOverMarker)
                    {
                        Control.IsMouseOverMarker = false;
                        Control.RestoreCursorOnLeave();
                    }
                }

                if (!Control.HoldInvalidation)
                {
                    Control.Invalidate();
                }
            }
        }

        internal void ForceUpdate()
        {
            if (Control != null)
            {
                foreach (GMapMarker obj in Markers)
                {
                    if (obj.IsVisible)
                    {
                        Control.UpdateMarkerLocalPosition(obj);
                    }
                }
            }
        }

        public virtual void OnRender(Graphics g)
        {
            if (Control != null)
            {
                if (Control.MarkersEnabled)
                {
                    foreach (GMapMarker m in Markers)
                    {
                        if (m.IsVisible || m.DisableRegionCheck)
                        {
                            m.OnRender(g);
                        }
                    }

                    foreach (GMapMarker m in Markers)
                    {
                        if (m.ToolTip != null && m.IsVisible)
                        {
                            if (!string.IsNullOrEmpty(m.ToolTipText) && (m.ToolTipMode == MarkerTooltipMode.Always || (m.ToolTipMode == MarkerTooltipMode.OnMouseOver && m.IsMouseOver)))
                            {
                                m.ToolTip.OnRender(g);
                            }
                        }
                    }
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("IsVisible", IsVisibile);

            GMapMarker[] markerArray = new GMapMarker[Markers.Count];
            Markers.CopyTo(markerArray, 0);
            info.AddValue("Markers", markerArray);
        }

        private GMapMarker[] deserializedMarkerArray;

        protected GMapOverlay(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetString("Id");
            IsVisibile = info.GetBoolean("IsVisible");

            deserializedMarkerArray = Extensions.GetValue(info, "Markers", new GMapMarker[0]);
          
            CreateEvents();
        }
        
        public void OnDeserialization(object sender)
        {
            foreach (GMapMarker marker in deserializedMarkerArray)
            {
                marker.Overlay = this;
                Markers.Add(marker);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                ClearEvents();

                foreach (var m in Markers)
                {
                    m.Dispose();
                }

                Clear();
            }
        }
    }
}