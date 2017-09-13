
namespace GMap.NET.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using GMap.NET.Projections;
    using System.IO;
    using GMap.NET.MapProviders;
    using System.ComponentModel;

#if NET40
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
#endif

#if PocketPC
   using OpenNETCF.ComponentModel;
   using OpenNETCF.Threading;
   using Thread=OpenNETCF.Threading.Thread2;
#endif

    /// <summary>
    /// internal map control core
    /// </summary>
    public class Core : IDisposable
    {
        public PointLatLng position;
        public GPoint positionPixel;

        public GPoint renderOffset;
        public GPoint centerTileXYLocation;
        public GPoint centerTileXYLocationLast;
        public GPoint dragPoint;
        public GPoint compensationOffset;

        public GPoint mouseDown;
        public GPoint mouseCurrent;
        public GPoint mouseLastZoom;

        public MouseWheelZoomType MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
        public bool MouseWheelZoomEnabled = true;

        public PointLatLng? LastLocationInBounds = null;
        public bool VirtualSizeEnabled = false;

        public GSize sizeOfMapArea;
        public GSize minOfTiles;
        public GSize maxOfTiles;

        public GRect tileRect;
        public GRect tileRectBearing;
        //public GRect currentRegion;
        public float bearing = 0;
        public bool IsRotated = false;

        public bool fillEmptyTiles = true;

        public TileMatrix Matrix = new TileMatrix();

        public List<DrawTile> tileDrawingList = new List<DrawTile>();
        public FastReaderWriterLock tileDrawingListLock = new FastReaderWriterLock();

#if !NET40
        public readonly Stack<LoadTask> tileLoadQueue = new Stack<LoadTask>();
#endif

#if !PocketPC
        static readonly int GThreadPoolSize = 4;
#else
        static readonly int GThreadPoolSize = 2;
#endif

        DateTime LastTileLoadStart = DateTime.Now;
        DateTime LastTileLoadEnd = DateTime.Now;
        public volatile bool IsStarted = false;
        int zoom;

        public double scaleX = 1;
        public double scaleY = 1;

        public int maxZoom = 2;
        public int minZoom = 2;
        public int Width;
        public int Height;

        public int pxRes100m;  // 100 meters
        public int pxRes1000m;  // 1km  
        public int pxRes10km; // 10km
        public int pxRes100km; // 100km
        public int pxRes1000km; // 1000km
        public int pxRes5000km; // 5000km

        /// <summary>
        /// is user dragging map
        /// </summary>
        public bool IsDragging = false;

        public Core()
        {
            Provider = EmptyProvider.Instance;
        }

        /// <summary>
        /// map zoom
        /// </summary>
        public int Zoom
        {
            get
            {
                return zoom;
            }
            set
            {
                if (zoom != value && !IsDragging)
                {
                    zoom = value;

                    minOfTiles = Provider.Projection.GetTileMatrixMinXY(value);
                    maxOfTiles = Provider.Projection.GetTileMatrixMaxXY(value);

                    positionPixel = Provider.Projection.FromLatLngToPixel(Position, value);

                    if (IsStarted)
                    {
                        CancelAsyncTasks();

                        Matrix.ClearLevelsBelove(zoom - LevelsKeepInMemmory);
                        Matrix.ClearLevelsAbove(zoom + LevelsKeepInMemmory);

                        lock (FailedLoads)
                        {
                            FailedLoads.Clear();
                            RaiseEmptyTileError = true;
                        }

                        GoToCurrentPositionOnZoom();
                        UpdateBounds();

                        if (OnMapZoomChanged != null)
                        {
                            OnMapZoomChanged();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// current marker position in pixel coordinates
        /// </summary>
        public GPoint PositionPixel
        {
            get
            {
                return positionPixel;
            }
        }

        /// <summary>
        /// current marker position
        /// </summary>
        public PointLatLng Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                positionPixel = Provider.Projection.FromLatLngToPixel(value, Zoom);

                if (OnCurrentPositionChanged != null)
                    OnCurrentPositionChanged(position);

                if (IsStarted)
                {
                    if (!IsDragging)
                    {
                        GoToCurrentPosition();
                    }
                }
            }
        }

        public GMapProvider provider;
        public GMapProvider Provider
        {
            get
            {
                return provider;
            }
            set
            {
                if (provider == null || !provider.Equals(value))
                {
                    bool diffProjection = (provider == null || provider.Projection != value.Projection);

                    provider = value;

                    if (!provider.IsInitialized)
                    {
                        provider.IsInitialized = true;
                        provider.OnInitialized();
                    }

                    if (provider.Projection != null && diffProjection)
                    {
                        tileRect = new GRect(GPoint.Empty, Provider.Projection.TileSize);
                        tileRectBearing = tileRect;
                        if (IsRotated)
                        {
                            tileRectBearing.Inflate(1, 1);
                        }

                        minOfTiles = Provider.Projection.GetTileMatrixMinXY(Zoom);
                        maxOfTiles = Provider.Projection.GetTileMatrixMaxXY(Zoom);
                        positionPixel = Provider.Projection.FromLatLngToPixel(Position, Zoom);
                    }

                    if (IsStarted)
                    {
                        CancelAsyncTasks();
                        if (diffProjection)
                        {
                            OnMapSizeChanged(Width, Height);
                        }
                        ReloadMap();

                        if (minZoom < provider.MinZoom)
                        {
                            minZoom = provider.MinZoom;
                        }

                        //if(provider.MaxZoom.HasValue && maxZoom > provider.MaxZoom)
                        //{
                        //   maxZoom = provider.MaxZoom.Value;
                        //}

                        zoomToArea = true;

                        if (provider.Area.HasValue && !provider.Area.Value.Contains(Position))
                        {
                            SetZoomToFitRect(provider.Area.Value);
                            zoomToArea = false;
                        }

                        if (OnMapTypeChanged != null)
                        {
                            OnMapTypeChanged(value);
                        }
                    }
                }
            }
        }

        public bool zoomToArea = true;

        /// <summary>
        /// sets zoom to max to fit rect
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public bool SetZoomToFitRect(RectLatLng rect)
        {
            int mmaxZoom = GetMaxZoomToFitRect(rect);
            if (mmaxZoom > 0)
            {
                PointLatLng center = new PointLatLng(rect.Lat - (rect.HeightLat / 2), rect.Lng + (rect.WidthLng / 2));
                Position = center;

                if (mmaxZoom > maxZoom)
                {
                    mmaxZoom = maxZoom;
                }

                if (Zoom != mmaxZoom)
                {
                    Zoom = (int)mmaxZoom;
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// is polygons enabled
        /// </summary>
        public bool PolygonsEnabled = true;

        /// <summary>
        /// is routes enabled
        /// </summary>
        public bool RoutesEnabled = true;

        /// <summary>
        /// is markers enabled
        /// </summary>
        public bool MarkersEnabled = true;

        /// <summary>
        /// can user drag map
        /// </summary>
        public bool CanDragMap = true;

        /// <summary>
        /// retry count to get tile 
        /// </summary>
#if !PocketPC
        public int RetryLoadTile = 0;
#else
      public int RetryLoadTile = 1;
#endif

        /// <summary>
        /// how many levels of tiles are staying decompresed in memory
        /// </summary>
#if !PocketPC
        public int LevelsKeepInMemmory = 5;
#else
      public int LevelsKeepInMemmory = 1;
#endif

        /// <summary>
        /// map render mode
        /// </summary>
        public RenderMode RenderMode = RenderMode.GDI_PLUS;

        /// <summary>
        /// occurs when current position is changed
        /// </summary>
        public event PositionChanged OnCurrentPositionChanged;

        /// <summary>
        /// occurs when tile set load is complete
        /// </summary>
        public event TileLoadComplete OnTileLoadComplete;

        /// <summary>
        /// occurs when tile set is starting to load
        /// </summary>
        public event TileLoadStart OnTileLoadStart;

        /// <summary>
        /// occurs on empty tile displayed
        /// </summary>
        public event EmptyTileError OnEmptyTileError;

        /// <summary>
        /// occurs on map drag
        /// </summary>
        public event MapDrag OnMapDrag;

        /// <summary>
        /// occurs on map zoom changed
        /// </summary>
        public event MapZoomChanged OnMapZoomChanged;

        /// <summary>
        /// occurs on map type changed
        /// </summary>
        public event MapTypeChanged OnMapTypeChanged;

        readonly List<Thread> GThreadPool = new List<Thread>();
        // ^
        // should be only one pool for multiply controls, any ideas how to fix?
        //static readonly List<Thread> GThreadPool = new List<Thread>();

        // windows forms or wpf
        public string SystemType;

        internal static int instances = 0;

        BackgroundWorker invalidator;

        public BackgroundWorker OnMapOpen()
        {
            if (!IsStarted)
            {
                int x = Interlocked.Increment(ref instances);

                IsStarted = true;

                if (x == 1)
                {
                    GMaps.Instance.noMapInstances = false;
                }

                GoToCurrentPosition();

                invalidator = new BackgroundWorker();
                invalidator.WorkerSupportsCancellation = true;
                invalidator.WorkerReportsProgress = true;
                invalidator.DoWork += new DoWorkEventHandler(invalidatorWatch);
                invalidator.RunWorkerAsync();

                //if(x == 1)
                //{
                // first control shown
                //}
            }
            return invalidator;
        }

        public void OnMapClose()
        {
            Dispose();
        }

        public readonly object invalidationLock = new object();
        public DateTime lastInvalidation = DateTime.Now;

        void invalidatorWatch(object sender, DoWorkEventArgs e)
        {
            var w = sender as BackgroundWorker;

            TimeSpan span = TimeSpan.FromMilliseconds(111);
            int spanMs = (int)span.TotalMilliseconds;
            bool skiped = false;
            TimeSpan delta;
            DateTime now = DateTime.Now;

            while (Refresh != null && (!skiped && Refresh.WaitOne() || (Refresh.WaitOne(spanMs, false) || true)))
            {
                if (w.CancellationPending)
                    break;

                now = DateTime.Now;
                lock (invalidationLock)
                {
                    delta = now - lastInvalidation;
                }

                if (delta > span)
                {
                    lock (invalidationLock)
                    {
                        lastInvalidation = now;
                    }
                    skiped = false;

                    w.ReportProgress(1);
                }
                else
                {
                    skiped = true;
                }
            }
        }

        public void UpdateCenterTileXYLocation()
        {
            PointLatLng center = FromLocalToLatLng(Width / 2, Height / 2);
            GPoint centerPixel = Provider.Projection.FromLatLngToPixel(center, Zoom);
            centerTileXYLocation = Provider.Projection.FromPixelToTileXY(centerPixel);
        }

        public int vWidth = 800;
        public int vHeight = 400;

        public void OnMapSizeChanged(int width, int height)
        {
            this.Width = width;
            this.Height = height;

            if (IsRotated)
            {
#if !PocketPC
                int diag = (int)Math.Round(Math.Sqrt(Width * Width + Height * Height) / Provider.Projection.TileSize.Width, MidpointRounding.AwayFromZero);
#else
            int diag = (int) Math.Round(Math.Sqrt(Width * Width + Height * Height) / Provider.Projection.TileSize.Width);
#endif
                sizeOfMapArea.Width = 1 + (diag / 2);
                sizeOfMapArea.Height = 1 + (diag / 2);
            }
            else
            {
                sizeOfMapArea.Width = 1 + (Width / Provider.Projection.TileSize.Width) / 2;
                sizeOfMapArea.Height = 1 + (Height / Provider.Projection.TileSize.Height) / 2;
            }

            if (IsStarted)
            {
                UpdateBounds();
                GoToCurrentPosition();
            }
        }

        /// <summary>
        /// gets current map view top/left coordinate, width in Lng, height in Lat
        /// </summary>
        /// <returns></returns>
        public RectLatLng ViewArea
        {
            get
            {
                if (Provider.Projection != null)
                {
                    var p = FromLocalToLatLng(0, 0);
                    var p2 = FromLocalToLatLng(Width, Height);

                    return RectLatLng.FromLTRB(p.Lng, p.Lat, p2.Lng, p2.Lat);
                }
                return RectLatLng.Empty;
            }
        }

        /// <summary>
        /// gets lat/lng from local control coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public PointLatLng FromLocalToLatLng(long x, long y)
        {
            GPoint p = new GPoint(x, y);
            p.OffsetNegative(renderOffset);
            p.Offset(compensationOffset);

            return Provider.Projection.FromPixelToLatLng(p, Zoom);
        }

        /// <summary>
        /// return local coordinates from lat/lng
        /// </summary>
        /// <param name="latlng"></param>
        /// <returns></returns>
        public GPoint FromLatLngToLocal(PointLatLng latlng)
        {
            GPoint pLocal = Provider.Projection.FromLatLngToPixel(latlng, Zoom);
            pLocal.Offset(renderOffset);
            pLocal.OffsetNegative(compensationOffset);
            return pLocal;
        }

        /// <summary>
        /// gets max zoom level to fit rectangle
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public int GetMaxZoomToFitRect(RectLatLng rect)
        {
            int zoom = minZoom;

            if (rect.HeightLat == 0 || rect.WidthLng == 0)
            {
                zoom = maxZoom / 2;
            }
            else
            {
                for (int i = (int)zoom; i <= maxZoom; i++)
                {
                    GPoint p1 = Provider.Projection.FromLatLngToPixel(rect.LocationTopLeft, i);
                    GPoint p2 = Provider.Projection.FromLatLngToPixel(rect.LocationRightBottom, i);

                    if (((p2.X - p1.X) <= Width + 10) && (p2.Y - p1.Y) <= Height + 10)
                    {
                        zoom = i;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return zoom;
        }

        /// <summary>
        /// initiates map dragging
        /// </summary>
        /// <param name="pt"></param>
        public void BeginDrag(GPoint pt)
        {
            dragPoint.X = pt.X - renderOffset.X;
            dragPoint.Y = pt.Y - renderOffset.Y;
            IsDragging = true;
        }

        /// <summary>
        /// ends map dragging
        /// </summary>
        public void EndDrag()
        {
            IsDragging = false;
            mouseDown = GPoint.Empty;

            Refresh.Set();
        }

        /// <summary>
        /// reloads map
        /// </summary>
        public void ReloadMap()
        {
            if (IsStarted)
            {
                okZoom = 0;
                skipOverZoom = 0;

                CancelAsyncTasks();

                Matrix.ClearAllLevels();

                lock (FailedLoads)
                {
                    FailedLoads.Clear();
                    RaiseEmptyTileError = true;
                }

                Refresh.Set();

                UpdateBounds();
            }
            else
            {
                throw new Exception("Please, do not call ReloadMap before form is loaded, it's useless");
            }
        }

        /// <summary>
        /// moves current position into map center
        /// </summary>
        public void GoToCurrentPosition()
        {
            compensationOffset = positionPixel; // TODO: fix

            // reset stuff
            renderOffset = GPoint.Empty;
            dragPoint = GPoint.Empty;

            //var dd = new GPoint(-(CurrentPositionGPixel.X - Width / 2), -(CurrentPositionGPixel.Y - Height / 2));
            //dd.Offset(compensationOffset);

            var d = new GPoint(Width / 2, Height / 2);

            this.Drag(d);
        }

        public bool MouseWheelZooming = false;

        /// <summary>
        /// moves current position into map center
        /// </summary>
        internal void GoToCurrentPositionOnZoom()
        {
            compensationOffset = positionPixel; // TODO: fix

            // reset stuff
            renderOffset = GPoint.Empty;
            dragPoint = GPoint.Empty;

            // goto location and centering
            if (MouseWheelZooming)
            {
                if (MouseWheelZoomType != MouseWheelZoomType.MousePositionWithoutCenter)
                {
                    GPoint pt = new GPoint(-(positionPixel.X - Width / 2), -(positionPixel.Y - Height / 2));
                    pt.Offset(compensationOffset);
                    renderOffset.X = pt.X - dragPoint.X;
                    renderOffset.Y = pt.Y - dragPoint.Y;
                }
                else // without centering
                {
                    renderOffset.X = -positionPixel.X - dragPoint.X;
                    renderOffset.Y = -positionPixel.Y - dragPoint.Y;
                    renderOffset.Offset(mouseLastZoom);
                    renderOffset.Offset(compensationOffset);
                }
            }
            else // use current map center
            {
                mouseLastZoom = GPoint.Empty;

                GPoint pt = new GPoint(-(positionPixel.X - Width / 2), -(positionPixel.Y - Height / 2));
                pt.Offset(compensationOffset);
                renderOffset.X = pt.X - dragPoint.X;
                renderOffset.Y = pt.Y - dragPoint.Y;
            }

            UpdateCenterTileXYLocation();
        }

        /// <summary>
        /// darg map by offset in pixels
        /// </summary>
        /// <param name="offset"></param>
        public void DragOffset(GPoint offset)
        {
            renderOffset.Offset(offset);

            UpdateCenterTileXYLocation();

            if (centerTileXYLocation != centerTileXYLocationLast)
            {
                centerTileXYLocationLast = centerTileXYLocation;
                UpdateBounds();
            }

            {
                LastLocationInBounds = Position;

                IsDragging = true;
                Position = FromLocalToLatLng((int)Width / 2, (int)Height / 2);
                IsDragging = false;
            }

            if (OnMapDrag != null)
            {
                OnMapDrag();
            }
        }

        /// <summary>
        /// drag map
        /// </summary>
        /// <param name="pt"></param>
        public void Drag(GPoint pt)
        {
            renderOffset.X = pt.X - dragPoint.X;
            renderOffset.Y = pt.Y - dragPoint.Y;

            UpdateCenterTileXYLocation();

            if (centerTileXYLocation != centerTileXYLocationLast)
            {
                centerTileXYLocationLast = centerTileXYLocation;
                UpdateBounds();
            }

            if (IsDragging)
            {
                LastLocationInBounds = Position;
                Position = FromLocalToLatLng((int)Width / 2, (int)Height / 2);

                if (OnMapDrag != null)
                {
                    OnMapDrag();
                }
            }
        }

        /// <summary>
        /// cancels tile loaders and bounds checker
        /// </summary>
        public void CancelAsyncTasks()
        {
            if (IsStarted)
            {
#if NET40
                //TODO: clear loading
#else
                Monitor.Enter(tileLoadQueue);
                try
                {
                    tileLoadQueue.Clear();
                }
                finally
                {
                    Monitor.Exit(tileLoadQueue);
                }
#endif
            }
        }

        bool RaiseEmptyTileError = false;
        public Dictionary<LoadTask, Exception> FailedLoads = new Dictionary<LoadTask, Exception>(new LoadTaskComparer());

        internal static readonly int WaitForTileLoadThreadTimeout = 5 * 1000 * 60; // 5 min.

        volatile int okZoom = 0;
        volatile int skipOverZoom = 0;

#if NET40
        static readonly BlockingCollection<LoadTask> tileLoadQueue4 = new BlockingCollection<LoadTask>(new ConcurrentStack<LoadTask>());
        static List<Task> tileLoadQueue4Tasks;
        static int loadWaitCount = 0;
        void AddLoadTask(LoadTask t)
        {
            if (tileLoadQueue4Tasks == null)
            {
                lock (tileLoadQueue4)
                {
                    if (tileLoadQueue4Tasks == null)
                    {
                        tileLoadQueue4Tasks = new List<Task>();

                        while (tileLoadQueue4Tasks.Count < GThreadPoolSize)
                        {
                            Debug.WriteLine("creating ProcessLoadTask: " + tileLoadQueue4Tasks.Count);

                            tileLoadQueue4Tasks.Add(Task.Factory.StartNew(delegate ()
                            {
                                string ctid = "ProcessLoadTask[" + Thread.CurrentThread.ManagedThreadId + "]";
                                Thread.CurrentThread.Name = ctid;

                                Debug.WriteLine(ctid + ": started");
                                do
                                { 
                                    if (tileLoadQueue4.Count == 0)
                                    {
                                        Debug.WriteLine(ctid + ": ready");

                                        if (Interlocked.Increment(ref loadWaitCount) >= GThreadPoolSize)
                                        {
                                            Interlocked.Exchange(ref loadWaitCount, 0);
                                            OnLoadComplete(ctid);
                                        }
                                    }
                                    ProcessLoadTask(tileLoadQueue4.Take(), ctid);
                                }
                                while (!tileLoadQueue4.IsAddingCompleted);

                                Debug.WriteLine(ctid + ": exit");

                            }, TaskCreationOptions.LongRunning));
                        }
                    }
                }
            }
            tileLoadQueue4.Add(t);
        }
#else
        byte loadWaitCount = 0;

        void tileLoadThread()
        {
            LoadTask? task = null;
            bool stop = false;

#if !PocketPC
            Thread ct = Thread.CurrentThread;
            string ctid = "Thread[" + ct.ManagedThreadId + "]";
#else
            int ctid = 0;
#endif
            while (!stop && IsStarted)
            {
                task = null;

                Monitor.Enter(tileLoadQueue);
                try
                {
                    while (tileLoadQueue.Count == 0)
                    {
                        if (++loadWaitCount >= GThreadPoolSize)
                        {
                            loadWaitCount = 0;
                            OnLoadComplete(ctid);
                        }

                        if (!IsStarted || false == Monitor.Wait(tileLoadQueue, WaitForTileLoadThreadTimeout, false) || !IsStarted)
                        {
                            stop = true;
                            break;
                        }
                    }

                    if (IsStarted && !stop || tileLoadQueue.Count > 0)
                    {
                        task = tileLoadQueue.Pop();
                    }
                }
                finally
                {
                    Monitor.Exit(tileLoadQueue);
                }

                if (task.HasValue && IsStarted)
                {
                    ProcessLoadTask(task.Value, ctid);
                }
            }

#if !PocketPC
            Monitor.Enter(tileLoadQueue);
            try
            {
                lock (GThreadPool)
                {
                    GThreadPool.Remove(ct);
                }
            }
            finally
            {
                Monitor.Exit(tileLoadQueue);
            }
#endif
        }
#endif

        static void ProcessLoadTask(LoadTask task, string ctid)
        {
            try
            {
                #region -- execute --

                var m = task.Core.Matrix.GetTileWithReadLock(task.Zoom, task.Pos);
                if (!m.NotEmpty)
                {
                    Tile t = new Tile(task.Zoom, task.Pos);

                    foreach (var tl in task.Core.provider.Overlays)
                    {
                        int retry = 0;
                        do
                        {
                            PureImage img = null;
                            Exception ex = null;

                            if (task.Zoom >= task.Core.provider.MinZoom && (!task.Core.provider.MaxZoom.HasValue || task.Zoom <= task.Core.provider.MaxZoom))
                            {
                                if (task.Core.skipOverZoom == 0 || task.Zoom <= task.Core.skipOverZoom)
                                {
                                    // tile number inversion(BottomLeft -> TopLeft)
                                    if (tl.InvertedAxisY)
                                    {
                                        img = GMaps.Instance.GetImageFrom(tl, new GPoint(task.Pos.X, task.Core.maxOfTiles.Height - task.Pos.Y), task.Zoom, out ex);
                                    }
                                    else // ok
                                    {
                                        img = GMaps.Instance.GetImageFrom(tl, task.Pos, task.Zoom, out ex);
                                    }
                                }
                            }

                            if (img != null && ex == null)
                            {
                                if (task.Core.okZoom < task.Zoom)
                                {
                                    task.Core.okZoom = task.Zoom;
                                    task.Core.skipOverZoom = 0;
                                }
                            }
                            else if (ex != null)
                            {
                                if ((task.Core.skipOverZoom != task.Core.okZoom) && (task.Zoom > task.Core.okZoom))
                                {
                                    if (ex.Message.Contains("(404) Not Found"))
                                    {
                                        task.Core.skipOverZoom = task.Core.okZoom;
                                    }
                                }
                            }

                            // check for parent tiles if not found
                            if (img == null && task.Core.okZoom > 0 && task.Core.fillEmptyTiles && task.Core.Provider.Projection is MercatorProjection)
                            {
                                int zoomOffset = task.Zoom > task.Core.okZoom ? task.Zoom - task.Core.okZoom : 1;
                                long Ix = 0;
                                GPoint parentTile = GPoint.Empty;

                                while (img == null && zoomOffset < task.Zoom)
                                {
                                    Ix = (long)Math.Pow(2, zoomOffset);
                                    parentTile = new GMap.NET.GPoint((task.Pos.X / Ix), (task.Pos.Y / Ix));
                                    img = GMaps.Instance.GetImageFrom(tl, parentTile, task.Zoom - zoomOffset++, out ex);
                                }

                                if (img != null)
                                {
                                    // offsets in quadrant
                                    long Xoff = Math.Abs(task.Pos.X - (parentTile.X * Ix));
                                    long Yoff = Math.Abs(task.Pos.Y - (parentTile.Y * Ix));

                                    img.IsParent = true;
                                    img.Ix = Ix;
                                    img.Xoff = Xoff;
                                    img.Yoff = Yoff;

                                    // wpf
                                    //var geometry = new RectangleGeometry(new Rect(Core.tileRect.X + 0.6, Core.tileRect.Y + 0.6, Core.tileRect.Width + 0.6, Core.tileRect.Height + 0.6));
                                    //var parentImgRect = new Rect(Core.tileRect.X - Core.tileRect.Width * Xoff + 0.6, Core.tileRect.Y - Core.tileRect.Height * Yoff + 0.6, Core.tileRect.Width * Ix + 0.6, Core.tileRect.Height * Ix + 0.6);

                                    // gdi+
                                    //System.Drawing.Rectangle dst = new System.Drawing.Rectangle((int)Core.tileRect.X, (int)Core.tileRect.Y, (int)Core.tileRect.Width, (int)Core.tileRect.Height);
                                    //System.Drawing.RectangleF srcRect = new System.Drawing.RectangleF((float)(Xoff * (img.Img.Width / Ix)), (float)(Yoff * (img.Img.Height / Ix)), (img.Img.Width / Ix), (img.Img.Height / Ix));
                                }
                            }

                            if (img != null)
                            {
                                Debug.WriteLine(ctid + " - tile loaded: " + img.Data.Length / 1024 + "KB, " + task);
                                {
                                    t.AddOverlay(img);
                                }
                                break;
                            }
                            else
                            {
                                if (ex != null)
                                {
                                    lock (task.Core.FailedLoads)
                                    {
                                        if (!task.Core.FailedLoads.ContainsKey(task))
                                        {
                                            task.Core.FailedLoads.Add(task, ex);

                                            if (task.Core.OnEmptyTileError != null)
                                            {
                                                if (!task.Core.RaiseEmptyTileError)
                                                {
                                                    task.Core.RaiseEmptyTileError = true;
                                                    task.Core.OnEmptyTileError(task.Zoom, task.Pos);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (task.Core.RetryLoadTile > 0)
                                {
                                    Debug.WriteLine(ctid + " - ProcessLoadTask: " + task + " -> empty tile, retry " + retry);
                                    {
                                        Thread.Sleep(1111);
                                    }
                                }
                            }
                        }
                        while (++retry < task.Core.RetryLoadTile);
                    }

                    if (t.HasAnyOverlays && task.Core.IsStarted)
                    {
                        task.Core.Matrix.SetTile(t);
                    }
                    else
                    {
                        t.Dispose();
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ctid + " - ProcessLoadTask: " + ex.ToString());
            }
            finally
            {
                if (task.Core.Refresh != null)
                {
                    task.Core.Refresh.Set();
                }
            }
        }

        void OnLoadComplete(string ctid)
        {
            LastTileLoadEnd = DateTime.Now;
            long lastTileLoadTimeMs = (long)(LastTileLoadEnd - LastTileLoadStart).TotalMilliseconds;

            #region -- clear stuff--
            if (IsStarted)
            {
                GMaps.Instance.MemoryCache.RemoveOverload();

                tileDrawingListLock.AcquireReaderLock();
                try
                {
                    Matrix.ClearLevelAndPointsNotIn(Zoom, tileDrawingList);
                }
                finally
                {
                    tileDrawingListLock.ReleaseReaderLock();
                }
            }
            #endregion

            UpdateGroundResolution();
#if UseGC
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
#endif
            if (OnTileLoadComplete != null)
            {
                OnTileLoadComplete(lastTileLoadTimeMs);
            }
        }

        public AutoResetEvent Refresh = new AutoResetEvent(false);

        public bool updatingBounds = false;

        /// <summary>
        /// updates map bounds
        /// </summary>
        void UpdateBounds()
        {
            if (!IsStarted || Provider.Equals(EmptyProvider.Instance))
            {
                return;
            }

            updatingBounds = true;

            tileDrawingListLock.AcquireWriterLock();
            try
            {
                #region -- find tiles around --
                tileDrawingList.Clear();

                for (long i = (int)Math.Floor(-sizeOfMapArea.Width * scaleX), countI = (int)Math.Ceiling(sizeOfMapArea.Width * scaleX); i <= countI; i++)
                {
                    for (long j = (int)Math.Floor(-sizeOfMapArea.Height * scaleY), countJ = (int)Math.Ceiling(sizeOfMapArea.Height * scaleY); j <= countJ; j++)
                    {
                        GPoint p = centerTileXYLocation;
                        p.X += i;
                        p.Y += j;

#if ContinuesMap
               // ----------------------------
               if(p.X < minOfTiles.Width)
               {
                  p.X += (maxOfTiles.Width + 1);
               }

               if(p.X > maxOfTiles.Width)
               {
                  p.X -= (maxOfTiles.Width + 1);
               }
               // ----------------------------
#endif

                        if (p.X >= minOfTiles.Width && p.Y >= minOfTiles.Height && p.X <= maxOfTiles.Width && p.Y <= maxOfTiles.Height)
                        {
                            DrawTile dt = new DrawTile()
                            {
                                PosXY = p,
                                PosPixel = new GPoint(p.X * tileRect.Width, p.Y * tileRect.Height),
                                DistanceSqr = (centerTileXYLocation.X - p.X) * (centerTileXYLocation.X - p.X) + (centerTileXYLocation.Y - p.Y) * (centerTileXYLocation.Y - p.Y)
                            };

                            if (!tileDrawingList.Contains(dt))
                            {
                                tileDrawingList.Add(dt);
                            }
                        }
                    }
                }

                if (GMaps.Instance.ShuffleTilesOnLoad)
                {
                    Stuff.Shuffle<DrawTile>(tileDrawingList);
                }
                else
                {
                    tileDrawingList.Sort();
                }
                #endregion
            }
            finally
            {
                tileDrawingListLock.ReleaseWriterLock();
            }

#if NET40
            Interlocked.Exchange(ref loadWaitCount, 0);
#else
            Monitor.Enter(tileLoadQueue);
            try
            {
#endif
            tileDrawingListLock.AcquireReaderLock();
            try
            {
                foreach (DrawTile p in tileDrawingList)
                {
                    LoadTask task = new LoadTask(p.PosXY, Zoom, this);
#if NET40
                    AddLoadTask(task);
#else
                        {
                            if (!tileLoadQueue.Contains(task))
                            {
                                tileLoadQueue.Push(task);
                            }
                        }
#endif
                }
            }
            finally
            {
                tileDrawingListLock.ReleaseReaderLock();
            }

#if !NET40
            #region -- starts loader threads if needed --

                lock (GThreadPool)
                {
                    while (GThreadPool.Count < GThreadPoolSize)
                    {
                        Thread t = new Thread(new ThreadStart(tileLoadThread));
                        {
                            t.Name = "TileLoader: " + GThreadPool.Count;
                            t.IsBackground = true;
                            t.Priority = ThreadPriority.BelowNormal;
                        }
                        GThreadPool.Add(t);

                        t.Start();
                    }
                }
            #endregion
#endif
            {
                LastTileLoadStart = DateTime.Now;
            }
#if !NET40
                loadWaitCount = 0;
                Monitor.PulseAll(tileLoadQueue);
            }
            finally
            {
                Monitor.Exit(tileLoadQueue);
            }
#endif
            updatingBounds = false;

            if (OnTileLoadStart != null)
            {
                OnTileLoadStart();
            }
        }

        /// <summary>
        /// updates ground resolution info
        /// </summary>
        void UpdateGroundResolution()
        {
            double rez = Provider.Projection.GetGroundResolution(Zoom, Position.Lat);
            pxRes100m = (int)(100.0 / rez); // 100 meters
            pxRes1000m = (int)(1000.0 / rez); // 1km  
            pxRes10km = (int)(10000.0 / rez); // 10km
            pxRes100km = (int)(100000.0 / rez); // 100km
            pxRes1000km = (int)(1000000.0 / rez); // 1000km
            pxRes5000km = (int)(5000000.0 / rez); // 5000km
        }

        #region IDisposable Members

        ~Core()
        {
            Dispose(false);
        }

        void Dispose(bool disposing)
        {
            if (IsStarted)
            {
                if (invalidator != null)
                {
                    invalidator.CancelAsync();
                    invalidator.DoWork -= new DoWorkEventHandler(invalidatorWatch);
                    invalidator.Dispose();
                    invalidator = null;
                }

                if (Refresh != null)
                {
                    Refresh.Set();
                    Refresh.Close();
                    Refresh = null;
                }

                int x = Interlocked.Decrement(ref instances);

                CancelAsyncTasks();
                IsStarted = false;

                if (Matrix != null)
                {
                    Matrix.Dispose();
                    Matrix = null;
                }

                if (FailedLoads != null)
                {
                    lock (FailedLoads)
                    {
                        FailedLoads.Clear();
                        RaiseEmptyTileError = false;
                    }
                    FailedLoads = null;
                }

                tileDrawingListLock.AcquireWriterLock();
                try
                {
                    tileDrawingList.Clear();
                }
                finally
                {
                    tileDrawingListLock.ReleaseWriterLock();
                }

#if NET40
                //TODO: maybe
#else
                // cancel waiting loaders
                Monitor.Enter(tileLoadQueue);
                try
                {
                    Monitor.PulseAll(tileLoadQueue);
                }
                finally
                {
                    Monitor.Exit(tileLoadQueue);
                }

                lock (GThreadPool)
                {
#if PocketPC
                Debug.WriteLine("waiting until loaders are stopped...");
                while(GThreadPool.Count > 0)
                {
                    var t = GThreadPool[0];

                    if (t.State != ThreadState.Stopped)
                    {
                        var tr = t.Join(1111);

                        Debug.WriteLine(t.Name + ", " + t.State);

                        if (!tr)
                        {
                            continue;
                        }
                        else
                        {
                            GThreadPool.Remove(t);
                        }
                    }
                    else
                    {
                        GThreadPool.Remove(t);
                    }
                }
                Thread.Sleep(1111);
#endif
                }
#endif

                if (tileDrawingListLock != null)
                {
                    tileDrawingListLock.Dispose();
                    tileDrawingListLock = null;
                    tileDrawingList = null;
                }

                if (x == 0)
                {
#if DEBUG
                    GMaps.Instance.CancelTileCaching();
#endif
                    GMaps.Instance.noMapInstances = true;
                    GMaps.Instance.WaitForCache.Set();
                    if (disposing)
                    {
                        GMaps.Instance.MemoryCache.Clear();
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
