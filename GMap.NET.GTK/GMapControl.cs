namespace GMap.NET.GTK
{
    using Images;
    using Markers;
    using Internals;
    using MapProviders;
    using System;
    using System.Collections.Specialized;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.IO;
    using System.Threading;
    using Helpers;

    public partial class GMapControl : Gtk.DrawingArea, Interface
    {
        private bool renderHelperLine = false;
        private bool _grayScale = false;
        private ColorMatrix _colorMatrix;
        private double _zoomReal;
        private Bitmap _backBuffer;
        private Graphics _gxOff;
        private RectLatLng? _lazySetZoomToFitRect = null;
        private ScaleModes _scaleMode = ScaleModes.Integer;
        private Gdk.CursorType _currentCursorType;
        private bool _showTileGridLines = false;
        private RectLatLng _selectedArea;
        private bool _negative = false;
        private bool _lazyEvents = true;
        private bool _isSelected = false;
        private bool _isDragging = false;
        private bool _isMouseOverRoute;
        private bool _isMouseOverPolygon;
        private bool _isMouseOverMarker;
        private PointLatLng _selectionStart;
        private PointLatLng _selectionEnd;
        private float? _mapRenderTransform = null;

        public event SelectionChange OnSelectionChange;
        public event MarkerEnter OnMarkerEnter;
        public event MarkerLeave OnMarkerLeave;
        public event MarkerClick OnMarkerClick;

        public readonly ObservableCollectionThreadSafe<GMapOverlay> Overlays =
            new ObservableCollectionThreadSafe<GMapOverlay>();

        [Category("GMap.NET")]
        [Description("maximum zoom level of map")]
        public int MaxZoom
        {
            get
            {
                return Core.maxZoom;
            }
            set
            {
                Core.maxZoom = value;
            }
        }

        [Category("GMap.NET")]
        [Description("minimum zoom level of map")]
        public int MinZoom
        {
            get
            {
                return Core.minZoom;
            }
            set
            {
                Core.minZoom = value;
            }
        }

        [Category("GMap.NET")]
        [Description("map zooming type for mouse wheel")]
        public MouseWheelZoomType MouseWheelZoomType
        {
            get
            {
                return Core.MouseWheelZoomType;
            }
            set
            {
                Core.MouseWheelZoomType = value;
            }
        }

        [Category("GMap.NET")]
        [Description("enable map zoom on mouse wheel")]
        public bool MouseWheelZoomEnabled
        {
            get
            {
                return Core.MouseWheelZoomEnabled;
            }
            set
            {
                Core.MouseWheelZoomEnabled = value;
            }
        }

        public string EmptyTileText = "We are sorry, but we don't\nhave imagery at this zoom\nlevel for this region.";

        public bool ShowCenter = true;

        public Pen EmptyTileBorders = new Pen(Brushes.White, 1);
        public Pen ScalePen = new Pen(Brushes.Blue, 1);
        public Pen CenterPen = new Pen(Brushes.Red, 1);
        public Pen SelectionPen = new Pen(Brushes.Black, 2);

        Brush SelectedAreaFill = new SolidBrush(Color.Transparent);
        Color selectedAreaFillColor = Color.Transparent;

        [Category("GMap.NET")]
        [Description("background color od the selected area")]
        public Color SelectedAreaFillColor
        {
            get
            {
                return selectedAreaFillColor;
            }
            set
            {
                if (selectedAreaFillColor != value)
                {
                    selectedAreaFillColor = value;

                    if (SelectedAreaFill != null)
                    {
                        SelectedAreaFill.Dispose();
                        SelectedAreaFill = null;
                    }

                    SelectedAreaFill = new SolidBrush(selectedAreaFillColor);
                }
            }
        }

        HelperLineOptions helperLineOption = HelperLineOptions.DontShow;

        [Browsable(false)]
        public HelperLineOptions HelperLineOption
        {
            get
            {
                return helperLineOption;
            }
            set
            {
                helperLineOption = value;
                renderHelperLine = (helperLineOption == HelperLineOptions.ShowAlways);
                if (Core.IsStarted)
                {
                    Invalidate();
                }
            }
        }

        public Pen HelperLinePen = new Pen(Color.Blue, 1);

        protected override bool OnKeyPressEvent(Gdk.EventKey e)
        {
            if (HelperLineOption == HelperLineOptions.ShowOnModifierKey)
            {
                if (renderHelperLine)
                {
                    Invalidate();
                }
            }

            return base.OnKeyPressEvent(e);
        }

        protected override bool OnKeyReleaseEvent(Gdk.EventKey e)
        {
            if (HelperLineOption == HelperLineOptions.ShowOnModifierKey)
            {
                if (!renderHelperLine)
                {
                    Invalidate();
                }
            }
            return base.OnKeyReleaseEvent(e);
        }

        Brush EmptytileBrush = new SolidBrush(Color.Navy);
        Color emptyTileColor = Color.Navy;

        [Category("GMap.NET")]
        [Description("background color of the empty tile")]
        public Color EmptyTileColor
        {
            get
            {
                return emptyTileColor;
            }
            set
            {
                if (emptyTileColor != value)
                {
                    emptyTileColor = value;

                    if (EmptytileBrush != null)
                    {
                        EmptytileBrush.Dispose();
                        EmptytileBrush = null;
                    }

                    EmptytileBrush = new SolidBrush(emptyTileColor);
                }
            }
        }

        public bool MapScaleInfoEnabled = false;

        public bool FillEmptyTiles = true;

        public bool DisableAltForSelection = false;

        [Browsable(false)]
        public int RetryLoadTile
        {
            get
            {
                return Core.RetryLoadTile;
            }
            set
            {
                Core.RetryLoadTile = value;
            }
        }

        [Browsable(false)]
        public int LevelsKeepInMemmory
        {
            get
            {
                return Core.LevelsKeepInMemmory;
            }

            set
            {
                Core.LevelsKeepInMemmory = value;
            }
        }

        [Category("GMap.NET")]
        public uint DragButton = 3;

        [Category("GMap.NET")]
        [Description("shows tile gridlines")]
        public bool ShowTileGridLines
        {
            get
            {
                return _showTileGridLines;
            }
            set
            {
                _showTileGridLines = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        public RectLatLng SelectedArea
        {
            get
            {
                return _selectedArea;
            }
            set
            {
                _selectedArea = value;

                if (Core.IsStarted)
                {
                    Invalidate();
                }
            }
        }

        public RectLatLng? BoundsOfMap = null;
        public bool ForceDoubleBuffer = false;
        readonly bool MobileMode = false;
        public bool HoldInvalidation = false;

        public void Refresh()
        {
            HoldInvalidation = false;

            lock (Core.invalidationLock)
            {
                Core.lastInvalidation = DateTime.Now;
            }

            base.QueueDraw();
        }

        public void Invalidate()
        {
            if (Core.Refresh != null)
            {
                Core.Refresh.Set();
            }
        }

        [Category("GMap.NET")]
        public bool GrayScaleMode
        {
            get
            {
                return _grayScale;
            }
            set
            {
                _grayScale = value;
                ColorMatrix = (value == true ? ColorMatrixs.GrayScale : null);
            }
        }

        [Category("GMap.NET")]
        public bool NegativeMode
        {
            get
            {
                return _negative;
            }
            set
            {
                _negative = value;
                ColorMatrix = (value == true ? ColorMatrixs.Negative : null);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public ColorMatrix ColorMatrix
        {
            get
            {
                return _colorMatrix;
            }
            set
            {
                _colorMatrix = value;
                if (GMapProvider.TileImageProxy != null && GMapProvider.TileImageProxy is GMapImageProxy)
                {
                    (GMapProvider.TileImageProxy as GMapImageProxy).ColorMatrix = value;
                    if (Core.IsStarted)
                    {
                        ReloadMap();
                    }
                }
            }
        }

        public GPoint RenderOffset
        {
            get
            {
                return Core.renderOffset;
            }
        }

        public readonly Core Core = new Core();

        internal readonly Font CopyrightFont = new Font(FontFamily.GenericSansSerif, 7, FontStyle.Regular);
        internal readonly Font MissingDataFont = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Bold);
        Font ScaleFont = new Font(FontFamily.GenericSansSerif, 5, FontStyle.Italic);
        internal readonly StringFormat CenterFormat = new StringFormat();
        internal readonly StringFormat BottomFormat = new StringFormat();
        readonly ImageAttributes TileFlipXYAttributes = new ImageAttributes();

#if DEBUG
        public Thread GuiThread;
#endif

#if !DESIGN

        public GMapControl()
        {
#if DEBUG
            GuiThread = Thread.CurrentThread;
#endif
            if (!IsDesignerHosted)
            {
                AddEvents((int)Gdk.EventMask.ButtonPressMask);
                AddEvents((int)Gdk.EventMask.ButtonReleaseMask);
                AddEvents((int)Gdk.EventMask.PointerMotionMask);
                AddEvents((int)Gdk.EventMask.ScrollMask);

                TileFlipXYAttributes.SetWrapMode(WrapMode.TileFlipXY);
                GrayScaleMode = GrayScaleMode;
                NegativeMode = NegativeMode;

                Core.SystemType = "WindowsForms";

                RenderMode = RenderMode.GDI_PLUS;

                CenterFormat.Alignment = StringAlignment.Center;
                CenterFormat.LineAlignment = StringAlignment.Center;

                BottomFormat.Alignment = StringAlignment.Center;
                BottomFormat.LineAlignment = StringAlignment.Far;

                if (GMaps.Instance.IsRunningOnMono)
                {
                    MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
                }

                Overlays.CollectionChanged +=
                    new NotifyCollectionChangedEventHandler(Overlays_CollectionChanged);
            }
        }

#endif

        static GMapControl()
        {
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            GMaps.Instance.UseDirectionsCache = false;
            GMaps.Instance.UseGeocoderCache = false;
            GMaps.Instance.UsePlacemarkCache = false;
            GMaps.Instance.UseRouteCache = false;
            GMaps.Instance.UseUrlCache = false;

            GMapImageProxy.Enable();
        }

        void Overlays_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GMapOverlay obj in e.NewItems)
                {
                    if (obj != null)
                    {
                        obj.Control = this;
                    }
                }

                if (Core.IsStarted && !HoldInvalidation)
                {
                    Invalidate();
                }
            }
        }

        void InvalidatorEngage(object sender, ProgressChangedEventArgs e)
        {
            Gtk.Application.Invoke(delegate
            {
                base.QueueDraw();
            });
        }

        internal void ForceUpdateOverlays()
        {
            try
            {
                HoldInvalidation = true;

                foreach (GMapOverlay o in Overlays)
                {
                    if (o.IsVisibile)
                    {
                        o.ForceUpdate();
                    }
                }
            }
            finally
            {
                Refresh();
            }
        }

        public void UpdateMarkerLocalPosition(GMapMarker marker)
        {
            GPoint p = FromLatLngToLocal(marker.Position);
            {
                if (!MobileMode)
                {
                    p.OffsetNegative(Core.renderOffset);
                }
                marker.LocalPosition = new Point((int)(p.X + marker.Offset.X), (int)(p.Y + marker.Offset.Y));
            }
        }

        public bool SetZoomToFitRect(RectLatLng rect)
        {
            if (_lazyEvents)
            {
                _lazySetZoomToFitRect = rect;
            }
            else
            {
                int maxZoom = Core.GetMaxZoomToFitRect(rect);
                if (maxZoom > 0)
                {
                    PointLatLng center = new PointLatLng(rect.Lat - (rect.HeightLat / 2), rect.Lng + (rect.WidthLng / 2));
                    Position = center;

                    if (maxZoom > MaxZoom)
                    {
                        maxZoom = MaxZoom;
                    }

                    if ((int)Zoom != maxZoom)
                    {
                        Zoom = maxZoom;
                    }

                    return true;
                }
            }

            return false;
        }

        public bool ZoomAndCenterMarkers(string overlayId)
        {
            RectLatLng? rect = GetRectOfAllMarkers(overlayId);
            if (rect.HasValue)
            {
                return SetZoomToFitRect(rect.Value);
            }

            return false;
        }

        public bool ZoomAndCenterRoutes(string overlayId)
        {
            return false;
        }

        public bool ZoomAndCenterRoute(MapRoute route)
        {
            return false;
        }

        public RectLatLng? GetRectOfAllMarkers(string overlayId)
        {
            RectLatLng? ret = null;

            double left = double.MaxValue;
            double top = double.MinValue;
            double right = double.MinValue;
            double bottom = double.MaxValue;

            foreach (GMapOverlay o in Overlays)
            {
                if (overlayId == null || o.Id == overlayId)
                {
                    if (o.IsVisibile && o.Markers.Count > 0)
                    {
                        foreach (GMapMarker m in o.Markers)
                        {
                            if (m.IsVisible)
                            {
                                if (m.Position.Lng < left)
                                {
                                    left = m.Position.Lng;
                                }

                                if (m.Position.Lat > top)
                                {
                                    top = m.Position.Lat;
                                }

                                if (m.Position.Lng > right)
                                {
                                    right = m.Position.Lng;
                                }

                                if (m.Position.Lat < bottom)
                                {
                                    bottom = m.Position.Lat;
                                }
                            }
                        }
                    }
                }
            }

            if (left != double.MaxValue && right != double.MinValue && top != double.MinValue && bottom != double.MaxValue)
            {
                ret = RectLatLng.FromLTRB(left, top, right, bottom);
            }

            return ret;
        }

        public RectLatLng? GetRectOfAllRoutes(string overlayId)
        {
            RectLatLng? ret = null;

            return ret;
        }

        public RectLatLng? GetRectOfRoute(MapRoute route)
        {
            RectLatLng? ret = null;

            return ret;
        }

        public Image ToImage()
        {
            Image ret = null;

            bool r = ForceDoubleBuffer;
            try
            {
                UpdateBackBuffer();

                if (!r)
                {
                    ForceDoubleBuffer = true;
                }

                Refresh();

                using (MemoryStream ms = new MemoryStream())
                {
                    using (var frame = (_backBuffer.Clone() as Bitmap))
                    {
                        frame.Save(ms, ImageFormat.Png);
                    }
                    ret = Image.FromStream(ms);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (!r)
                {
                    ForceDoubleBuffer = false;
                    ClearBackBuffer();
                }
            }
            return ret;
        }

        public void Offset(int x, int y)
        {
            if (IsRotated)
            {
                Point[] p = new Point[] { new Point(x, y) };
                rotationMatrixInvert.TransformVectors(p);
                x = p[0].X;
                y = p[0].Y;
            }

            Core.DragOffset(new GPoint(x, y));

            ForceUpdateOverlays();
        }

        public readonly static bool IsDesignerHosted = LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        protected override void OnShown()
        {
            base.OnShown();

            if (!IsDesignerHosted)
            {
                if (_lazyEvents)
                {
                    _lazyEvents = false;

                    if (_lazySetZoomToFitRect.HasValue)
                    {
                        SetZoomToFitRect(_lazySetZoomToFitRect.Value);
                        _lazySetZoomToFitRect = null;
                    }
                }

                Core.OnMapOpen().ProgressChanged += new ProgressChangedEventHandler(InvalidatorEngage);
                ForceUpdateOverlays();
            }
        }

        public override void Destroy()
        {
            Core.OnMapClose();

            Overlays.CollectionChanged -= new NotifyCollectionChangedEventHandler(Overlays_CollectionChanged);

            foreach (var o in Overlays)
            {
                o.Dispose();
            }

            Overlays.Clear();

            ScaleFont.Dispose();
            ScalePen.Dispose();
            CenterFormat.Dispose();
            CenterPen.Dispose();
            BottomFormat.Dispose();
            CopyrightFont.Dispose();
            EmptyTileBorders.Dispose();
            EmptytileBrush.Dispose();
            SelectedAreaFill.Dispose();
            SelectionPen.Dispose();
            ClearBackBuffer();

            base.Destroy();
        }

        public Color EmptyMapBackground = Color.WhiteSmoke;

        [Category("GMap.NET")]
        [Description("Widget has frame")]
        public bool HasFrame { get; set; }

        protected override bool OnExposeEvent(Gdk.EventExpose e)
        {
            if (ForceDoubleBuffer)
            {
                if (_gxOff != null)
                {
                    DrawGraphics(_gxOff);
                    throw new NotSupportedException();
                }
            }
            else
            {
                using (Graphics g = Gtk.DotNet.Graphics.FromDrawable(e.Window))
                {
                    g.SetClip(new Rectangle(e.Area.X, e.Area.Y, e.Area.Width, e.Area.Height));
                    DrawGraphics(g);
                }
            }

            if (HasFrame)
            {
                var gc = new Gdk.GC(e.Window);
                e.Window.DrawRectangle(gc, false, e.Area.X, e.Area.Y, e.Area.Width - 1, e.Area.Height - 1);
            }

            return base.OnExposeEvent(e);
        }

        void DrawGraphics(Graphics g)
        {
            g.Clear(EmptyMapBackground);

            if (_mapRenderTransform.HasValue)
            {
                if (!MobileMode)
                {
                    var center = new GPoint(Allocation.Width / 2, Allocation.Height / 2);
                    var delta = center;
                    delta.OffsetNegative(Core.renderOffset);
                    var pos = center;
                    pos.OffsetNegative(delta);

                    g.ScaleTransform(_mapRenderTransform.Value, _mapRenderTransform.Value, MatrixOrder.Append);
                    g.TranslateTransform(pos.X, pos.Y, MatrixOrder.Append);

                    DrawMap(g);
                    g.ResetTransform();

                    g.TranslateTransform(pos.X, pos.Y, MatrixOrder.Append);
                }
                else
                {
                    DrawMap(g);
                    g.ResetTransform();
                }
                OnPaintOverlays(g);
            }
            else
            {
                if (IsRotated)
                {
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    g.TranslateTransform((float)(Core.Width / 2.0), (float)(Core.Height / 2.0));
                    g.RotateTransform(-Bearing);
                    g.TranslateTransform((float)(-Core.Width / 2.0), (float)(-Core.Height / 2.0));

                    g.TranslateTransform(Core.renderOffset.X, Core.renderOffset.Y);

                    DrawMap(g);

                    g.ResetTransform();
                    g.TranslateTransform(Core.renderOffset.X, Core.renderOffset.Y);

                    OnPaintOverlays(g);
                }
                else
                {
                    if (!MobileMode)
                    {
                        g.TranslateTransform(Core.renderOffset.X, Core.renderOffset.Y);
                    }
                    DrawMap(g);
                    OnPaintOverlays(g);
                }
            }
        }

        void DrawMap(Graphics g)
        {
            if (Core.updatingBounds || MapProvider == EmptyProvider.Instance || MapProvider == null)
            {
                return;
            }

            Core.tileDrawingListLock.AcquireReaderLock();
            Core.Matrix.EnterReadLock();

            try
            {
                foreach (var tilePoint in Core.tileDrawingList)
                {
                    {
                        Core.tileRect.Location = tilePoint.PosPixel;
                        if (ForceDoubleBuffer)
                        {
                            if (MobileMode)
                            {
                                Core.tileRect.Offset(Core.renderOffset);
                            }
                        }
                        Core.tileRect.OffsetNegative(Core.compensationOffset);

                        {
                            bool found = false;

                            Tile t = Core.Matrix.GetTileWithNoLock(Core.Zoom, tilePoint.PosXY);
                            if (t.NotEmpty)
                            {
                                // Render tile
                                {
                                    foreach (GMapImage img in t.Overlays)
                                    {
                                        if (img != null && img.Img != null)
                                        {
                                            if (!found)
                                                found = true;

                                            if (!img.IsParent)
                                            {
                                                if (!_mapRenderTransform.HasValue && !IsRotated)
                                                {
                                                    g.DrawImage(img.Img, Core.tileRect.X, Core.tileRect.Y, Core.tileRect.Width, Core.tileRect.Height);
                                                }
                                                else
                                                {
                                                    g.DrawImage(img.Img, new Rectangle((int)Core.tileRect.X, (int)Core.tileRect.Y, (int)Core.tileRect.Width, (int)Core.tileRect.Height), 0, 0, Core.tileRect.Width, Core.tileRect.Height, GraphicsUnit.Pixel, TileFlipXYAttributes);
                                                }
                                            }
                                            else
                                            {
                                                // TODO: Move calculations to loader thread
                                                RectangleF srcRect = new RectangleF((float)(img.Xoff * (img.Img.Width / img.Ix)), (float)(img.Yoff * (img.Img.Height / img.Ix)), (img.Img.Width / img.Ix), (img.Img.Height / img.Ix));
                                                Rectangle dst = new Rectangle((int)Core.tileRect.X, (int)Core.tileRect.Y, (int)Core.tileRect.Width, (int)Core.tileRect.Height);

                                                g.DrawImage(img.Img, dst, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, TileFlipXYAttributes);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (FillEmptyTiles && MapProvider.Projection is Projections.MercatorProjection)
                            {
                                int zoomOffset = 1;
                                Tile parentTile = Tile.Empty;
                                long Ix = 0;

                                while (!parentTile.NotEmpty && zoomOffset < Core.Zoom && zoomOffset <= LevelsKeepInMemmory)
                                {
                                    Ix = (long)Math.Pow(2, zoomOffset);
                                    parentTile = Core.Matrix.GetTileWithNoLock(Core.Zoom - zoomOffset++, new GPoint((int)(tilePoint.PosXY.X / Ix), (int)(tilePoint.PosXY.Y / Ix)));
                                }

                                if (parentTile.NotEmpty)
                                {
                                    long Xoff = Math.Abs(tilePoint.PosXY.X - (parentTile.Pos.X * Ix));
                                    long Yoff = Math.Abs(tilePoint.PosXY.Y - (parentTile.Pos.Y * Ix));

                                    // Render tile 
                                    {
                                        foreach (GMapImage img in parentTile.Overlays)
                                        {
                                            if (img != null && img.Img != null && !img.IsParent)
                                            {
                                                if (!found)
                                                    found = true;

                                                RectangleF srcRect = new RectangleF((float)(Xoff * (img.Img.Width / Ix)), (float)(Yoff * (img.Img.Height / Ix)), (img.Img.Width / Ix), (img.Img.Height / Ix));
                                                Rectangle dst = new Rectangle((int)Core.tileRect.X, (int)Core.tileRect.Y, (int)Core.tileRect.Width, (int)Core.tileRect.Height);

                                                g.DrawImage(img.Img, dst, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, TileFlipXYAttributes);
                                                g.FillRectangle(SelectedAreaFill, dst);
                                            }
                                        }
                                    }
                                }
                            }

                            if (!found)
                            {
                                lock (Core.FailedLoads)
                                {
                                    var lt = new LoadTask(tilePoint.PosXY, Core.Zoom);
                                    if (Core.FailedLoads.ContainsKey(lt))
                                    {
                                        var ex = Core.FailedLoads[lt];

                                        g.FillRectangle(EmptytileBrush, new RectangleF(Core.tileRect.X, Core.tileRect.Y, Core.tileRect.Width, Core.tileRect.Height));

                                        g.DrawString("Exception: " + ex.Message, MissingDataFont, Brushes.Red, new RectangleF(Core.tileRect.X + 11, Core.tileRect.Y + 11, Core.tileRect.Width - 11, Core.tileRect.Height - 11));

                                        g.DrawString(EmptyTileText, MissingDataFont, Brushes.Blue, new RectangleF(Core.tileRect.X, Core.tileRect.Y, Core.tileRect.Width, Core.tileRect.Height), CenterFormat);

                                        g.DrawRectangle(EmptyTileBorders, (int)Core.tileRect.X, (int)Core.tileRect.Y, (int)Core.tileRect.Width, (int)Core.tileRect.Height);
                                    }
                                }
                            }

                            if (ShowTileGridLines)
                            {
                                g.DrawRectangle(EmptyTileBorders, (int)Core.tileRect.X, (int)Core.tileRect.Y, (int)Core.tileRect.Width, (int)Core.tileRect.Height);
                                {
                                    g.DrawString((tilePoint.PosXY == Core.centerTileXYLocation ? "CENTER: " : "TILE: ") + tilePoint, MissingDataFont, Brushes.Red, new RectangleF(Core.tileRect.X, Core.tileRect.Y, Core.tileRect.Width, Core.tileRect.Height), CenterFormat);
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                Core.Matrix.LeaveReadLock();
                Core.tileDrawingListLock.ReleaseReaderLock();
            }
        }

        protected virtual void OnPaintOverlays(Graphics g)
        {
#if DEBUG
            if (GuiThread != Thread.CurrentThread)
            {
                return;
            }
#endif

            g.SmoothingMode = SmoothingMode.HighQuality;
            foreach (GMapOverlay o in Overlays)
            {
                if (o.IsVisibile)
                {
                    o.OnRender(g);
                }
            }

#if DEBUG
            if (!IsRotated)
            {
                g.DrawLine(ScalePen, -20, 0, 20, 0);
                g.DrawLine(ScalePen, 0, -20, 0, 20);
                g.DrawString("debug build", CopyrightFont, Brushes.Blue, 2, CopyrightFont.Height);
            }
#endif

            if (!MobileMode)
            {
                g.ResetTransform();
            }

            if (!SelectedArea.IsEmpty && MouseWheelZoomEnabled)
            {
                GPoint p1 = FromLatLngToLocal(SelectedArea.LocationTopLeft);
                GPoint p2 = FromLatLngToLocal(SelectedArea.LocationRightBottom);

                long x1 = p1.X;
                long y1 = p1.Y;
                long x2 = p2.X;
                long y2 = p2.Y;

                g.DrawRectangle(SelectionPen, x1, y1, x2 - x1, y2 - y1);
                g.FillRectangle(SelectedAreaFill, x1, y1, x2 - x1, y2 - y1);
            }

            if (renderHelperLine)
            {
                int mouseX, mouseY;
                base.GetPointer(out mouseX, out mouseY);

                g.DrawLine(HelperLinePen, mouseX, 0, mouseX, Allocation.Height);
                g.DrawLine(HelperLinePen, 0, mouseY, Allocation.Width, mouseY);
            }
            if (ShowCenter)
            {
                g.DrawLine(CenterPen, Allocation.Width / 2 - 5, Allocation.Height / 2, Allocation.Width / 2 + 5, Allocation.Height / 2);
                g.DrawLine(CenterPen, Allocation.Width / 2, Allocation.Height / 2 - 5, Allocation.Width / 2, Allocation.Height / 2 + 5);
            }

            if (!string.IsNullOrEmpty(Core.provider.Copyright))
            {
                g.DrawString(Core.provider.Copyright, CopyrightFont, Brushes.Navy, 3, Allocation.Height - CopyrightFont.Height - 5);
            }

            if (MapScaleInfoEnabled)
            {
                if (Allocation.Width > Core.pxRes5000km)
                {
                    g.DrawRectangle(ScalePen, 10, 10, Core.pxRes5000km, 10);
                    g.DrawString("5000Km", ScaleFont, Brushes.Blue, Core.pxRes5000km + 10, 11);
                }
                if (Allocation.Width > Core.pxRes1000km)
                {
                    g.DrawRectangle(ScalePen, 10, 10, Core.pxRes1000km, 10);
                    g.DrawString("1000Km", ScaleFont, Brushes.Blue, Core.pxRes1000km + 10, 11);
                }
                if (Allocation.Width > Core.pxRes100km && Zoom > 2)
                {
                    g.DrawRectangle(ScalePen, 10, 10, Core.pxRes100km, 10);
                    g.DrawString("100Km", ScaleFont, Brushes.Blue, Core.pxRes100km + 10, 11);
                }
                if (Allocation.Width > Core.pxRes10km && Zoom > 5)
                {
                    g.DrawRectangle(ScalePen, 10, 10, Core.pxRes10km, 10);
                    g.DrawString("10Km", ScaleFont, Brushes.Blue, Core.pxRes10km + 10, 11);
                }
                if (Allocation.Width > Core.pxRes1000m && Zoom >= 10)
                {
                    g.DrawRectangle(ScalePen, 10, 10, Core.pxRes1000m, 10);
                    g.DrawString("1000m", ScaleFont, Brushes.Blue, Core.pxRes1000m + 10, 11);
                }
                if (Allocation.Width > Core.pxRes100m && Zoom > 11)
                {
                    g.DrawRectangle(ScalePen, 10, 10, Core.pxRes100m, 10);
                    g.DrawString("100m", ScaleFont, Brushes.Blue, Core.pxRes100m + 9, 11);
                }
            }
        }

        readonly Matrix rotationMatrix = new Matrix();
        readonly Matrix rotationMatrixInvert = new Matrix();

        void UpdateRotationMatrix()
        {
            PointF center = new PointF(Core.Width / 2, Core.Height / 2);

            rotationMatrix.Reset();
            rotationMatrix.RotateAt(-Bearing, center);

            rotationMatrixInvert.Reset();
            rotationMatrixInvert.RotateAt(-Bearing, center);
            rotationMatrixInvert.Invert();
        }

        [Browsable(false)]
        public bool IsRotated
        {
            get
            {
                return Core.IsRotated;
            }
        }

        [Category("GMap.NET")]
        public float Bearing
        {
            get
            {
                return Core.bearing;
            }
            set
            {
                if (Core.bearing != value)
                {
                    bool resize = Core.bearing == 0;
                    Core.bearing = value;

                    UpdateRotationMatrix();

                    if (value != 0 && value % 360 != 0)
                    {
                        Core.IsRotated = true;

                        if (Core.tileRectBearing.Size == Core.tileRect.Size)
                        {
                            Core.tileRectBearing = Core.tileRect;
                            Core.tileRectBearing.Inflate(1, 1);
                        }
                    }
                    else
                    {
                        Core.IsRotated = false;
                        Core.tileRectBearing = Core.tileRect;
                    }

                    if (resize)
                    {
                        Core.OnMapSizeChanged(Allocation.Width, Allocation.Height);
                    }

                    if (!HoldInvalidation && Core.IsStarted)
                    {
                        ForceUpdateOverlays();
                    }
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool VirtualSizeEnabled
        {
            get
            {
                return Core.VirtualSizeEnabled;
            }
            set
            {
                Core.VirtualSizeEnabled = value;
            }
        }

        protected override void OnSizeAllocated(Gdk.Rectangle box)
        {
            base.OnSizeAllocated(box);

            if (box.Width == 0 || box.Height == 0)
            {
                return;
            }

            if (box.Width == Core.Width && box.Height == Core.Height)
            {
                return;
            }

            if (!IsDesignerHosted)
            {
                if (ForceDoubleBuffer)
                {
                    UpdateBackBuffer();
                }

                if (VirtualSizeEnabled)
                {
                    Core.OnMapSizeChanged(Core.vWidth, Core.vHeight);
                }
                else
                {
                    Core.OnMapSizeChanged(box.Width, box.Height);
                }

                if (Visible && Core.IsStarted)
                {
                    if (IsRotated)
                    {
                        UpdateRotationMatrix();
                    }
                    ForceUpdateOverlays();
                }
            }
        }

        void UpdateBackBuffer()
        {
            ClearBackBuffer();

            _backBuffer = new Bitmap(Allocation.Width, Allocation.Height);
            _gxOff = Graphics.FromImage(_backBuffer);
        }

        private void ClearBackBuffer()
        {
            if (_backBuffer != null)
            {
                _backBuffer.Dispose();
                _backBuffer = null;
            }
            if (_gxOff != null)
            {
                _gxOff.Dispose();
                _gxOff = null;
            }
        }

        protected override bool OnButtonPressEvent(Gdk.EventButton e)
        {
            if (!IsMouseOverMarker)
            {
                if (e.Button == DragButton && CanDragMap)
                {
                    Core.mouseDown = ApplyRotationInversion((int)e.X, (int)e.Y);
                    this.Invalidate();
                }
                else if (!_isSelected)
                {
                    _isSelected = true;
                    SelectedArea = RectLatLng.Empty;
                    _selectionEnd = PointLatLng.Empty;
                    _selectionStart = FromLocalToLatLng((int)e.X, (int)e.Y);
                }
            }
            else
            {
                var markerPosition = FromLocalToLatLng((int)e.X, (int)e.Y);

                foreach (GMapOverlay o in Overlays)
                {
                    foreach (GMapMarker m in o.Markers)
                    {
                        if (m.IsVisible && m.IsHitTestVisible && m.IsMouseOver)
                        {
                            OnMarkerClick?.Invoke(m);
                            break;
                        }
                    }
                }
            }

            return base.OnButtonPressEvent(e);
        }

        protected override bool OnButtonReleaseEvent(Gdk.EventButton e)
        {
            if (_isSelected)
            {
                _isSelected = false;
            }

            if (Core.IsDragging)
            {
                if (_isDragging)
                {
                    _isDragging = false;
                    _currentCursorType = Gdk.CursorType.LeftPtr;
                    this.GdkWindow.Cursor = new Gdk.Cursor(_currentCursorType);
                }

                Core.EndDrag();

                if (BoundsOfMap.HasValue && !BoundsOfMap.Value.Contains(Position))
                {
                    if (Core.LastLocationInBounds.HasValue)
                    {
                        Position = Core.LastLocationInBounds.Value;
                    }
                }
            }
            else
            {
                if (e.Button == DragButton)
                {
                    Core.mouseDown = GPoint.Empty;
                }

                if (!_selectionEnd.IsEmpty && !_selectionStart.IsEmpty)
                {
                    bool zoomtofit = false;

                    if (!SelectedArea.IsEmpty)
                    {
                        zoomtofit = SetZoomToFitRect(SelectedArea);
                    }

                    OnSelectionChange?.Invoke(SelectedArea, zoomtofit);
                }
                else
                {
                    Invalidate();
                }
            }
            return base.OnButtonReleaseEvent(e);
        }

        GPoint ApplyRotationInversion(int x, int y)
        {
            GPoint ret = new GPoint(x, y);

            if (IsRotated)
            {
                Point[] tt = new Point[] { new System.Drawing.Point(x, y) };
                rotationMatrixInvert.TransformPoints(tt);
                var f = tt[0];

                ret.X = f.X;
                ret.Y = f.Y;
            }

            return ret;
        }

        GPoint ApplyRotation(int x, int y)
        {
            GPoint ret = new GPoint(x, y);

            if (IsRotated)
            {
                Point[] tt = new Point[] { new Point(x, y) };
                rotationMatrix.TransformPoints(tt);
                var f = tt[0];

                ret.X = f.X;
                ret.Y = f.Y;
            }

            return ret;
        }

        public Size DragSize = new Size(4, 4);

        protected override bool OnMotionNotifyEvent(Gdk.EventMotion e)
        {
            if (!Core.IsDragging && !Core.mouseDown.IsEmpty)
            {
                GPoint p = ApplyRotationInversion((int)e.X, (int)e.Y);
                if (Math.Abs(p.X - Core.mouseDown.X) * 2 >= DragSize.Width || Math.Abs(p.Y - Core.mouseDown.Y) * 2 >= DragSize.Height)
                {
                    Core.BeginDrag(Core.mouseDown);
                }
            }

            if (Core.IsDragging)
            {
                if (!_isDragging)
                {
                    _isDragging = true;

                    _currentCursorType = Gdk.CursorType.Fleur;
                    GdkWindow.Cursor = new Gdk.Cursor(_currentCursorType);
                }

                if (BoundsOfMap.HasValue && !BoundsOfMap.Value.Contains(Position))
                {
                    // ...
                }
                else
                {
                    Core.mouseCurrent = ApplyRotationInversion((int)e.X, (int)e.Y);
                    Core.Drag(Core.mouseCurrent);
                    if (MobileMode || IsRotated)
                    {
                        ForceUpdateOverlays();
                    }

                    base.QueueDraw();
                }
            }
            else
            {
                if ((_isSelected || DisableAltForSelection) && MouseWheelZoomEnabled)
                {
                    _selectionEnd = FromLocalToLatLng((int)e.X, (int)e.Y);
                    {
                        PointLatLng p1 = _selectionStart;
                        PointLatLng p2 = _selectionEnd;

                        double x1 = Math.Min(p1.Lng, p2.Lng);
                        double y1 = Math.Max(p1.Lat, p2.Lat);
                        double x2 = Math.Max(p1.Lng, p2.Lng);
                        double y2 = Math.Min(p1.Lat, p2.Lat);

                        SelectedArea = new RectLatLng(y1, x1, x2 - x1, y1 - y2);
                    }
                }
                else
                    if (Core.mouseDown.IsEmpty)
                {
                    for (int i = Overlays.Count - 1; i >= 0; i--)
                    {
                        GMapOverlay o = Overlays[i];
                        if (o != null && o.IsVisibile)
                        {
                            foreach (GMapMarker m in o.Markers)
                            {
                                if (m.IsVisible && m.IsHitTestVisible)
                                {
                                    GPoint rp = new GPoint((long)e.X, (long)e.Y);

                                    if (!MobileMode)
                                    {
                                        rp.OffsetNegative(Core.renderOffset);
                                    }

                                    if (m.LocalArea.Contains((int)rp.X, (int)rp.Y))
                                    {
                                        if (!m.IsMouseOver)
                                        {
                                            SetCursorHandOnEnter();
                                            m.IsMouseOver = true;
                                            IsMouseOverMarker = true;
                                            OnMarkerEnter?.Invoke(m);

                                            Invalidate();
                                        }
                                    }
                                    else if (m.IsMouseOver)
                                    {
                                        m.IsMouseOver = false;
                                        IsMouseOverMarker = false;
                                        RestoreCursorOnLeave();
                                        OnMarkerLeave?.Invoke(m);

                                        Invalidate();
                                    }
                                }
                            }
                        }
                    }
                }

                if (renderHelperLine)
                {
                    base.QueueDraw();
                }
            }
            return base.OnMotionNotifyEvent(e);
        }

        internal void RestoreCursorOnLeave()
        {
            if (overObjectCount <= 0 && _currentCursorType != Gdk.CursorType.LeftPtr)
            {
                overObjectCount = 0;
                _currentCursorType = Gdk.CursorType.LeftPtr;
                GdkWindow.Cursor = new Gdk.Cursor(_currentCursorType);
            }
        }

        internal void SetCursorHandOnEnter()
        {
            if (overObjectCount <= 0 && _currentCursorType != Gdk.CursorType.Hand1)
            {
                overObjectCount = 0;
                _currentCursorType = Gdk.CursorType.Hand1;
                GdkWindow.Cursor = new Gdk.Cursor(_currentCursorType);
            }
        }

        public bool DisableFocusOnMouseEnter = false;

        protected override bool OnEnterNotifyEvent(Gdk.EventCrossing e)
        {
            if (!DisableFocusOnMouseEnter)
            {
                GrabFocus();
            }

            return base.OnEnterNotifyEvent(e);
        }

        public bool InvertedMouseWheelZooming = false;

        public bool IgnoreMarkerOnMouseWheel = false;

        protected override bool OnScrollEvent(Gdk.EventScroll e)
        {
            if (MouseWheelZoomEnabled && (!IsMouseOverMarker || IgnoreMarkerOnMouseWheel) && !Core.IsDragging)
            {
                if (Core.mouseLastZoom.X != (int)e.X && Core.mouseLastZoom.Y != (int)e.Y)
                {
                    if (MouseWheelZoomType == MouseWheelZoomType.MousePositionAndCenter)
                    {
                        Core.position = FromLocalToLatLng((int)e.X, (int)e.Y);
                    }
                    else if (MouseWheelZoomType == MouseWheelZoomType.ViewCenter)
                    {
                        Core.position = FromLocalToLatLng((int)Allocation.Width / 2, (int)Allocation.Height / 2);
                    }
                    else if (MouseWheelZoomType == MouseWheelZoomType.MousePositionWithoutCenter)
                    {
                        Core.position = FromLocalToLatLng((int)e.X, (int)e.Y);
                    }

                    Core.mouseLastZoom.X = (int)e.X;
                    Core.mouseLastZoom.Y = (int)e.Y;
                }

                if (MouseWheelZoomType != MouseWheelZoomType.MousePositionWithoutCenter)
                {
                    if (!GMaps.Instance.IsRunningOnMono)
                    {
                        //FIXME
                        //System.Drawing.Point p = PointToScreen(new System.Drawing.Point(Width / 2, Height / 2));
                        //Stuff.SetCursorPos((int)p.X, (int)p.Y);
                    }
                }

                Core.MouseWheelZooming = true;

                if (e.Direction == Gdk.ScrollDirection.Up)
                {
                    if (!InvertedMouseWheelZooming)
                    {
                        Zoom = ((int)Zoom) + 1;
                    }
                    else
                    {
                        Zoom = ((int)(Zoom + 0.99)) - 1;
                    }
                }
                else if (e.Direction == Gdk.ScrollDirection.Down)
                {
                    if (!InvertedMouseWheelZooming)
                    {
                        Zoom = ((int)(Zoom + 0.99)) - 1;
                    }
                    else
                    {
                        Zoom = ((int)Zoom) + 1;
                    }
                }

                Core.MouseWheelZooming = false;
            }
            return base.OnScrollEvent(e);
        }

        public void ReloadMap()
        {
            Core.ReloadMap();
        }

        public GeoCoderStatusCode SetPositionByKeywords(string keys)
        {
            GeoCoderStatusCode status = GeoCoderStatusCode.Unknow;

            GeocodingProvider gp = MapProvider as GeocodingProvider;
            if (gp == null)
            {
                gp = GMapProviders.OpenStreetMap as GeocodingProvider;
            }

            if (gp != null)
            {
                var pt = gp.GetPoint(keys, out status);
                if (status == GeoCoderStatusCode.G_GEO_SUCCESS && pt.HasValue)
                {
                    Position = pt.Value;
                }
            }

            return status;
        }

        public PointLatLng FromLocalToLatLng(GPoint point)
        {
            return FromLocalToLatLng((int)point.X, (int)point.Y);
        }

        public PointLatLng FromLocalToLatLng(int x, int y)
        {
            if (_mapRenderTransform.HasValue)
            {
                x = (int)(Core.renderOffset.X + ((x - Core.renderOffset.X) / _mapRenderTransform.Value));
                y = (int)(Core.renderOffset.Y + ((y - Core.renderOffset.Y) / _mapRenderTransform.Value));
            }

            if (IsRotated)
            {
                Point[] tt = new Point[] { new Point(x, y) };
                rotationMatrixInvert.TransformPoints(tt);
                var f = tt[0];

                if (VirtualSizeEnabled)
                {
                    f.X += (Allocation.Width - Core.vWidth) / 2;
                    f.Y += (Allocation.Height - Core.vHeight) / 2;
                }

                x = f.X;
                y = f.Y;
            }
            return Core.FromLocalToLatLng(x, y);
        }

        public GPoint FromLatLngToLocal(PointLatLng point)
        {
            GPoint ret = Core.FromLatLngToLocal(point);

            if (_mapRenderTransform.HasValue)
            {
                ret.X = (int)(Core.renderOffset.X + ((Core.renderOffset.X - ret.X) * -_mapRenderTransform.Value));
                ret.Y = (int)(Core.renderOffset.Y + ((Core.renderOffset.Y - ret.Y) * -_mapRenderTransform.Value));
            }

            if (IsRotated)
            {
                Point[] tt = new Point[] { new Point((int)ret.X, (int)ret.Y) };
                rotationMatrix.TransformPoints(tt);
                var f = tt[0];

                if (VirtualSizeEnabled)
                {
                    f.X += (Allocation.Width - Core.vWidth) / 2;
                    f.Y += (Allocation.Height - Core.vHeight) / 2;
                }

                ret.X = f.X;
                ret.Y = f.Y;
            }

            return ret;
        }

        public bool ShowExportDialog()
        {
            return false;
        }

        public bool ShowImportDialog()
        {
            return false;
        }

        [Category("GMap.NET")]
        [Description("map scale type")]
        public ScaleModes ScaleMode
        {
            get
            {
                return _scaleMode;
            }
            set
            {
                _scaleMode = value;
            }
        }

        [Category("GMap.NET"), DefaultValue(0)]
        public double Zoom
        {
            get
            {
                return _zoomReal;
            }
            set
            {
                if (_zoomReal != value)
                {
                    if (value > MaxZoom)
                    {
                        _zoomReal = MaxZoom;
                    }
                    else if (value < MinZoom)
                    {
                        _zoomReal = MinZoom;
                    }
                    else
                    {
                        _zoomReal = value;
                    }

                    double remainder = value % 1;
                    if (ScaleMode == ScaleModes.Fractional && remainder != 0)
                    {
                        float scaleValue = (float)Math.Pow(2d, remainder);
                        {
                            _mapRenderTransform = scaleValue;
                        }

                        ZoomStep = Convert.ToInt32(value - remainder);
                    }
                    else
                    {
                        _mapRenderTransform = null;
                        ZoomStep = (int)Math.Floor(value);
                    }

                    if (Core.IsStarted && !IsDragging)
                    {
                        ForceUpdateOverlays();
                    }
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal int ZoomStep
        {
            get
            {
                return Core.Zoom;
            }
            set
            {
                if (value > MaxZoom)
                {
                    Core.Zoom = MaxZoom;
                }
                else if (value < MinZoom)
                {
                    Core.Zoom = MinZoom;
                }
                else
                {
                    Core.Zoom = value;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public PointLatLng Position
        {
            get
            {
                return Core.Position;
            }
            set
            {
                Core.Position = value;

                if (Core.IsStarted)
                {
                    ForceUpdateOverlays();
                }
            }
        }

        [Browsable(false)]
        public GPoint PositionPixel
        {
            get
            {
                return Core.PositionPixel;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public string CacheLocation
        {
            get
            {
                return CacheLocator.Location;
            }
            set
            {
                CacheLocator.Location = value;
            }
        }

        [Browsable(false)]
        public bool IsDragging
        {
            get
            {
                return _isDragging;
            }
        }

        internal int overObjectCount = 0;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsMouseOverMarker
        {
            get
            {
                return _isMouseOverMarker;
            }
            internal set
            {
                _isMouseOverMarker = value;
                overObjectCount += value ? 1 : -1;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsMouseOverRoute
        {
            get
            {
                return _isMouseOverRoute;
            }
            internal set
            {
                _isMouseOverRoute = value;
                overObjectCount += value ? 1 : -1;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsMouseOverPolygon
        {
            get
            {
                return _isMouseOverPolygon;
            }
            internal set
            {
                _isMouseOverPolygon = value;
                overObjectCount += value ? 1 : -1;
            }
        }

        [Browsable(false)]
        public RectLatLng ViewArea
        {
            get
            {
                if (!IsRotated)
                {
                    return Core.ViewArea;
                }
                else if (Core.Provider.Projection != null)
                {
                    var p1 = FromLocalToLatLng(0, 0);
                    var p2 = FromLocalToLatLng(Allocation.Width, Allocation.Height);

                    return RectLatLng.FromLTRB(p1.Lng, p1.Lat, p2.Lng, p2.Lat);
                }

                return RectLatLng.Empty;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GMapProvider MapProvider
        {
            get
            {
                return Core.Provider;
            }
            set
            {
                if (Core.Provider == null || !Core.Provider.Equals(value))
                {
                    RectLatLng viewarea = SelectedArea;
                    if (viewarea != RectLatLng.Empty)
                    {
                        Position = new PointLatLng(viewarea.Lat - viewarea.HeightLat / 2, viewarea.Lng + viewarea.WidthLng / 2);
                    }
                    else
                    {
                        viewarea = ViewArea;
                    }

                    Core.Provider = value;

                    if (Core.IsStarted)
                    {
                        if (Core.zoomToArea)
                        {
                            if (viewarea != RectLatLng.Empty && viewarea != ViewArea)
                            {
                                int bestZoom = Core.GetMaxZoomToFitRect(viewarea);
                                if (bestZoom > 0 && Zoom != bestZoom)
                                {
                                    Zoom = bestZoom;
                                }
                            }
                        }
                        else
                        {
                            ForceUpdateOverlays();
                        }
                    }
                }
            }
        }

        [Category("GMap.NET")]
        public bool RoutesEnabled
        {
            get
            {
                return Core.RoutesEnabled;
            }
            set
            {
                Core.RoutesEnabled = value;
            }
        }

        [Category("GMap.NET")]
        public bool PolygonsEnabled
        {
            get
            {
                return Core.PolygonsEnabled;
            }
            set
            {
                Core.PolygonsEnabled = value;
            }
        }

        [Category("GMap.NET")]
        public bool MarkersEnabled
        {
            get
            {
                return Core.MarkersEnabled;
            }
            set
            {
                Core.MarkersEnabled = value;
            }
        }

        [Category("GMap.NET")]
        public bool CanDragMap
        {
            get
            {
                return Core.CanDragMap;
            }
            set
            {
                Core.CanDragMap = value;
            }
        }

        [Browsable(false)]
        public RenderMode RenderMode
        {
            get
            {
                return Core.RenderMode;
            }
            internal set
            {
                Core.RenderMode = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GMaps Manager
        {
            get
            {
                return GMaps.Instance;
            }
        }

        public event PositionChanged OnPositionChanged
        {
            add
            {
                Core.OnCurrentPositionChanged += value;
            }
            remove
            {
                Core.OnCurrentPositionChanged -= value;
            }
        }

        public event TileLoadComplete OnTileLoadComplete
        {
            add
            {
                Core.OnTileLoadComplete += value;
            }
            remove
            {
                Core.OnTileLoadComplete -= value;
            }
        }

        public event TileLoadStart OnTileLoadStart
        {
            add
            {
                Core.OnTileLoadStart += value;
            }
            remove
            {
                Core.OnTileLoadStart -= value;
            }
        }

        public event MapDrag OnMapDrag
        {
            add
            {
                Core.OnMapDrag += value;
            }
            remove
            {
                Core.OnMapDrag -= value;
            }
        }

        public event MapZoomChanged OnMapZoomChanged
        {
            add
            {
                Core.OnMapZoomChanged += value;
            }
            remove
            {
                Core.OnMapZoomChanged -= value;
            }
        }

        public event MapTypeChanged OnMapTypeChanged
        {
            add
            {
                Core.OnMapTypeChanged += value;
            }
            remove
            {
                Core.OnMapTypeChanged -= value;
            }
        }

        public event EmptyTileError OnEmptyTileError
        {
            add
            {
                Core.OnEmptyTileError += value;
            }
            remove
            {
                Core.OnEmptyTileError -= value;
            }
        }

        static readonly BinaryFormatter BinaryFormatter = new BinaryFormatter();

        public void SerializeOverlays(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            GMapOverlay[] overlayArray = new GMapOverlay[Overlays.Count];
            Overlays.CopyTo(overlayArray, 0);

            BinaryFormatter.Serialize(stream, overlayArray);
        }

        public void DeserializeOverlays(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            GMapOverlay[] overlayArray = BinaryFormatter.Deserialize(stream) as GMapOverlay[];

            foreach (GMapOverlay overlay in overlayArray)
            {
                overlay.Control = this;
                Overlays.Add(overlay);
            }

            ForceUpdateOverlays();
        }
    }

    public delegate void SelectionChange(RectLatLng Selection, bool ZoomToFit);
}