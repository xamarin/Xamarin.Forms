namespace GMap.NET.MapProviders
{
    using System;

    /// <summary>
    /// CzechTuristMap provider, http://www.mapy.cz/
    /// </summary>
    public class CzechGeographicMapProvider : CzechMapProviderBase
    {
        public static readonly CzechGeographicMapProvider Instance;

        CzechGeographicMapProvider()
        {
        }

        static CzechGeographicMapProvider()
        {
            Instance = new CzechGeographicMapProvider();
        }

        #region GMapProvider Members

        readonly Guid id = new Guid("50EC9FCC-E4D7-4F53-8700-2D1DB73A1D48");
        public override Guid Id
        {
            get
            {
                return id;
            }
        }

        readonly string name = "CzechGeographicMap";
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
            // http://m3.mapserver.mapy.czzemepis-m/14-8802-5528

            return string.Format(UrlFormat, GetServerNum(pos, 3) + 1, zoom, pos.X, pos.Y);
        }

        static readonly string UrlFormat = "http://m{0}.mapserver.mapy.cz/zemepis-m/{1}-{2}-{3}";
    }
}