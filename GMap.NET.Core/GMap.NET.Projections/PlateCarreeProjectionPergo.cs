namespace GMap.NET.Projections
{
#if OLD_PERGO
   using System;

   /// <summary>
   /// Plate Carrée (literally, “plane square”) projection
   /// PROJCS["WGS 84 / World Equidistant Cylindrical",GEOGCS["GCS_WGS_1984",DATUM["D_WGS_1984",SPHEROID["WGS_1984",6378137,298.257223563]],PRIMEM["Greenwich",0],UNIT["Degree",0.017453292519943295]],UNIT["Meter",1]]
   /// </summary>
   public class PlateCarreeProjectionPergo : PureProjection
   {
      public static readonly PlateCarreeProjectionPergo Instance = new PlateCarreeProjectionPergo();

      static readonly double MinLatitude = -85.05112878;
      static readonly double MaxLatitude = 85.05112878;
      static readonly double MinLongitude = -180;
      static readonly double MaxLongitude = 180;

      public override RectLatLng Bounds
      {
         get
         {
            return RectLatLng.FromLTRB(MinLongitude, MaxLatitude, MaxLongitude, MinLatitude);
         }
      }

      GSize tileSize = new GSize(256, 256);
      public override GSize TileSize
      {
         get
         {
            return tileSize;
         }
      }

      public override double Axis
      {
         get
         {
            return 6378137;
         }
      }

      public override double Flattening
      {
         get
         {
            return (1.0 / 298.257223563);
         }
      }

      public override GPoint FromLatLngToPixel(double lat, double lng, int zoom)
      {
         GPoint ret = GPoint.Empty;

         lat = Clip(lat, MinLatitude, MaxLatitude);
         lng = Clip(lng, MinLongitude, MaxLongitude);

         GSize s = GetTileMatrixSizePixel(zoom);
         double mapSizeX = s.Width;
         double mapSizeY = s.Height;

         double scale = 360.0 / mapSizeX;

         ret.Y = (long)((90.0 - lat) / scale);
         ret.X = (long)((lng + 180.0) / scale);

         return ret;
      }

      public override PointLatLng FromPixelToLatLng(long x, long y, int zoom)
      {
         PointLatLng ret = PointLatLng.Empty;

         GSize s = GetTileMatrixSizePixel(zoom);
         double mapSizeX = s.Width;
         double mapSizeY = s.Height;

         double scale = 360.0 / mapSizeX;

         ret.Lat = 90 - (y * scale);
         ret.Lng = (x * scale) - 180;

         return ret;
      }

      public override GSize GetTileMatrixMaxXY(int zoom)
      {
         long y = (long)Math.Pow(2, zoom);
         return new GSize((2*y) - 1, y - 1);
      }

      public override GSize GetTileMatrixMinXY(int zoom)
      {
         return new GSize(0, 0);
      }
   }
#endif
}
