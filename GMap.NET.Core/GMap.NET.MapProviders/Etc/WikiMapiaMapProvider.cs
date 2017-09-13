
namespace GMap.NET.MapProviders
{
   using System;
   using GMap.NET.Projections;

   public abstract class WikiMapiaMapProviderBase : GMapProvider
   {
      public WikiMapiaMapProviderBase()
      {
         MaxZoom = 22;
         RefererUrl = "http://wikimapia.org/";
         Copyright = string.Format("© WikiMapia.org - Map data ©{0} WikiMapia", DateTime.Today.Year);
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
            return MercatorProjection.Instance;
         }
      }

      public override GMapProvider[] Overlays
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public override PureImage GetTileImage(GPoint pos, int zoom)
      {
         throw new NotImplementedException();
      }

      #endregion

      public static int GetServerNum(GPoint pos)
      {
         return (int)(pos.X % 4 + (pos.Y % 4) * 4);
      }
   }

   /// <summary>
   /// WikiMapiaMap provider, http://wikimapia.org/
   /// </summary>
   public class WikiMapiaMapProvider : WikiMapiaMapProviderBase
   {
      public static readonly WikiMapiaMapProvider Instance;

      WikiMapiaMapProvider()
      {
      }

      static WikiMapiaMapProvider()
      {
         Instance = new WikiMapiaMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("7974022B-1AA6-41F1-8D01-F49940E4B48C");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "WikiMapiaMap";
      public override string Name
      {
         get
         {
            return name;
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
         string url = MakeTileImageUrl(pos, zoom, string.Empty);
         return GetTileImageUsingHttp(url);
      }

      #endregion

      string MakeTileImageUrl(GPoint pos, int zoom, string language)
      {
         return string.Format(UrlFormat, GetServerNum(pos), pos.X, pos.Y, zoom);
      }

      static readonly string UrlFormat = "http://i{0}.wikimapia.org/?x={1}&y={2}&zoom={3}";
   }
}
