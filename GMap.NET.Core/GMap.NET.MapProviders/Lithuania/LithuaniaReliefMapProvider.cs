
namespace GMap.NET.MapProviders
{
   using System;
   using GMap.NET.Projections;

   /// <summary>
   /// LithuaniaReliefMap provider, http://www.maps.lt/map/
   /// </summary>
   public class LithuaniaReliefMapProvider : LithuaniaMapProviderBase
   {
      public static readonly LithuaniaReliefMapProvider Instance;

      LithuaniaReliefMapProvider()
      {

      }

      static LithuaniaReliefMapProvider()
      {
         Instance = new LithuaniaReliefMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("85F89205-1062-4F10-B536-90CD8B2F1B7D");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "LithuaniaReliefMap";
      public override string Name
      {
         get
         {
            return name;
         }
      }

      public override PureProjection Projection
      {
         get
         {
            return LKS94rProjection.Instance;
         }
      }

      public override PureImage GetTileImage(GPoint pos, int zoom)
      {
         string url = MakeTileImageUrl(pos, zoom, LanguageStr);

         return GetTileImageUsingHttp(url);
      }

      #endregion

      string MakeTileImageUrl(GPoint pos, int zoom, string language)
      {
         // http://dc5.maps.lt/cache/mapslt_relief_vector/map/_alllayers/L09/R00001892/C000020df.jpg

         return string.Format(UrlFormat, zoom, pos.Y, pos.X);
      }

      static readonly string UrlFormat = "http://dc5.maps.lt/cache/mapslt_relief_vector/map/_alllayers/L{0:00}/R{1:x8}/C{2:x8}.png";
   }
}