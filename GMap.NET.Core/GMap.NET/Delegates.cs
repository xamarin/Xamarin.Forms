
namespace GMap.NET
{
   using GMap.NET.MapProviders;

   public delegate void PositionChanged(PointLatLng point);

   public delegate void TileLoadComplete(long ElapsedMilliseconds);
   public delegate void TileLoadStart();

   public delegate void TileCacheComplete();
   public delegate void TileCacheStart();
   public delegate void TileCacheProgress(int tilesLeft);

   public delegate void MapDrag();
   public delegate void MapZoomChanged();
   public delegate void MapTypeChanged(GMapProvider type);

   public delegate void EmptyTileError(int zoom, GPoint pos);
}
