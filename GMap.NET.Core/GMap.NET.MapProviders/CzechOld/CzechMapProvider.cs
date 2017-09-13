
namespace GMap.NET.MapProviders
{
   using System;
   using GMap.NET.Projections;

   public abstract class CzechMapProviderBaseOld : GMapProvider
   {
      public CzechMapProviderBaseOld()
      {
         RefererUrl = "http://www.mapy.cz/";
         Area = new RectLatLng(51.2024819920053, 11.8401353319027, 7.22833716731277, 2.78312271922872);
      }

      #region GMapProvider Members
      public override Guid Id
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public override string Name
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public override PureProjection Projection
      {
         get
         {
            return MapyCZProjection.Instance;
         }
      }

      GMapProvider[] overlays;
      public override GMapProvider[] Overlays
      {
         get
         {
            if(overlays == null)
            {
               overlays = new GMapProvider[] { this };
            }
            return overlays;
         }
      }

      public override PureImage GetTileImage(GPoint pos, int zoom)
      {
         throw new NotImplementedException();
      }
      #endregion
   }

   /// <summary>
   /// CzechMap provider, http://www.mapy.cz/
   /// </summary>
   public class CzechMapProviderOld : CzechMapProviderBaseOld
   {
      public static readonly CzechMapProviderOld Instance;

      CzechMapProviderOld()
      {
      }

      static CzechMapProviderOld()
      {
         Instance = new CzechMapProviderOld();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("6A1AF99A-84C6-4EF6-91A5-77B9D03257C2");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "CzechOldMap";
      public override string Name
      {
         get
         {
            return name;
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
         // ['base','ophoto','turist','army2']  
         // http://m1.mapserver.mapy.cz/base-n/3_8000000_8000000

         long xx = pos.X << (28 - zoom);
         long yy = ((((long)Math.Pow(2.0, (double)zoom)) - 1) - pos.Y) << (28 - zoom);

         return string.Format(UrlFormat, GetServerNum(pos, 3) + 1, zoom, xx, yy);
      }

      static readonly string UrlFormat = "http://m{0}.mapserver.mapy.cz/base-n/{1}_{2:x7}_{3:x7}";
   }
}