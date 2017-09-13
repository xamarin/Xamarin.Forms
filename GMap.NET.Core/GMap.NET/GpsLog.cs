
namespace GMap.NET
{
   using System;

   public struct GpsLog
   {
      public DateTime TimeUTC;
      public long SessionCounter;
      public double? Delta;
      public double? Speed;
      public double? SeaLevelAltitude;
      public double? EllipsoidAltitude;
      public short? SatellitesInView;
      public short? SatelliteCount;
      public PointLatLng Position;
      public double? PositionDilutionOfPrecision;
      public double? HorizontalDilutionOfPrecision;
      public double? VerticalDilutionOfPrecision;
      public FixQuality FixQuality;
      public FixType FixType;
      public FixSelection FixSelection;

      public override string ToString()
      {
         return string.Format("{0}: {1}", SessionCounter, TimeUTC);
      }
   }

   public enum FixQuality : int
   {
      Unknown=0,
      Gps,
      DGps
   }
   public enum FixType : int
   {
      Unknown=0,
      XyD,
      XyzD
   }

   public enum FixSelection : int
   {
      Unknown=0,
      Auto,
      Manual
   }
}
