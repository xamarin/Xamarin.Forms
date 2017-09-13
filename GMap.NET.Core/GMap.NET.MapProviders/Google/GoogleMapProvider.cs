namespace GMap.NET.MapProviders
{
    using System;
    using Projections;
    using System.Security.Cryptography;
    using System.Diagnostics;
    using System.Net;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading;
    using Internals;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml;
    using System.Text;
#if PocketPC
    using OpenNETCF.Security.Cryptography;
#endif

    public abstract class GoogleMapProviderBase : GMapProvider, RoutingProvider, GeocodingProvider, DirectionsProvider
    {
        public GoogleMapProviderBase()
        {
            MaxZoom = null;
            RefererUrl = string.Format("http://maps.{0}/", Server);
            Copyright = string.Format("©{0} Google - Map data ©{0} Tele Atlas, Imagery ©{0} TerraMetrics", DateTime.Today.Year);
        }

        public readonly string ServerAPIs /* ;}~~ */ = Stuff.GString(/*{^_^}*/"9gERyvblybF8iMuCt/LD6w=="/*d{'_'}b*/);
        public readonly string Server /* ;}~~~~ */ = Stuff.GString(/*{^_^}*/"gosr2U13BoS+bXaIxt6XWg=="/*d{'_'}b*/);
        public readonly string ServerChina /* ;}~ */ = Stuff.GString(/*{^_^}*/"gosr2U13BoTEJoJJuO25gQ=="/*d{'_'}b*/);
        public readonly string ServerKorea /* ;}~~ */ = Stuff.GString(/*{^_^}*/"8ZVBOEsBinzi+zmP7y7pPA=="/*d{'_'}b*/);
        public readonly string ServerKoreaKr /* ;}~ */ = Stuff.GString(/*{^_^}*/"gosr2U13BoQyz1gkC4QLfg=="/*d{'_'}b*/);

        public string SecureWord = "Galileo";

        /// <summary>
        /// Your application's API key, obtained from the Google Developers Console.
        /// This key identifies your application for purposes of quota management. 
        /// Must provide either API key or Maps for Work credentials.
        /// </summary>
        public string ApiKey = string.Empty;

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

        GMapProvider [] overlays;
        public override GMapProvider [] Overlays
        {
            get
            {
                if (overlays == null)
                {
                    overlays = new GMapProvider [] { this };
                }
                return overlays;
            }
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            throw new NotImplementedException();
        }
        #endregion

        public bool TryCorrectVersion = true;
        static bool init = false;

        public override void OnInitialized()
        {
            if (!init && TryCorrectVersion)
            {               
                string url = string.Format("https://maps.{0}/maps/api/js?client=google-maps-lite&amp;libraries=search&amp;language=en&amp;region=", ServerAPIs);
                try
                {
                    string html = GMaps.Instance.UseUrlCache ? Cache.Instance.GetContent(url, CacheType.UrlCache, TimeSpan.FromHours(8)) : string.Empty;

                    if (string.IsNullOrEmpty(html))
                    {
                        html = GetContentUsingHttp(url);
                        if (!string.IsNullOrEmpty(html))
                        {
                            if (GMaps.Instance.UseUrlCache)
                            {
                                Cache.Instance.SaveContent(url, CacheType.UrlCache, html);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(html))
                    {
                        #region -- match versions --
                        Regex reg = new Regex(string.Format(@"https?://mts?\d.{0}/maps/vt\?lyrs=m@(\d*)", Server), RegexOptions.IgnoreCase);
                        Match mat = reg.Match(html);
                        if (mat.Success)
                        {
                            GroupCollection gc = mat.Groups;
                            int count = gc.Count;
                            if (count > 0)
                            {
                                string ver = string.Format("m@{0}", gc [1].Value);
                                string old = GMapProviders.GoogleMap.Version;

                                GMapProviders.GoogleMap.Version = ver;
                                GMapProviders.GoogleChinaMap.Version = ver;
                                
                                string verh = string.Format("h@{0}", gc [1].Value);
                                string oldh = GMapProviders.GoogleHybridMap.Version;

                                GMapProviders.GoogleHybridMap.Version = verh;
                                GMapProviders.GoogleChinaHybridMap.Version = verh;
#if DEBUG
                                if (Debugger.IsAttached && ver != old)
                                {
                                    Thread.Sleep(1111);
                                }
#endif
                            }
                        }

                        reg = new Regex(string.Format(@"https?://khms?\d.{0}/kh\?v=(\d*)", Server), RegexOptions.IgnoreCase);
                        mat = reg.Match(html);
                        if (mat.Success)
                        {
                            GroupCollection gc = mat.Groups;
                            int count = gc.Count;
                            if (count > 0)
                            {
                                string ver = gc [1].Value;
                                string old = GMapProviders.GoogleSatelliteMap.Version;

                                GMapProviders.GoogleSatelliteMap.Version = ver;
                                GMapProviders.GoogleKoreaSatelliteMap.Version = ver;
                                GMapProviders.GoogleChinaSatelliteMap.Version = "s@" + ver;
#if DEBUG
                                if (Debugger.IsAttached && ver != old)
                                {
                                    Thread.Sleep(1111);
                                }
#endif
                            }
                        }

                        reg = new Regex(string.Format(@"https?://mts?\d.{0}/maps/vt\?lyrs=t@(\d*),r@(\d*)", Server), RegexOptions.IgnoreCase);
                        mat = reg.Match(html);
                        if (mat.Success)
                        {
                            GroupCollection gc = mat.Groups;
                            int count = gc.Count;
                            if (count > 1)
                            {
                                string ver = string.Format("t@{0},r@{1}", gc [1].Value, gc [2].Value);
                                string old = GMapProviders.GoogleTerrainMap.Version;

                                GMapProviders.GoogleTerrainMap.Version = ver;
                                GMapProviders.GoogleChinaTerrainMap.Version = ver;
#if DEBUG
                                if (Debugger.IsAttached && ver != old)
                                {
                                    Thread.Sleep(1111);
                                }
#endif
                            }
                        }
                        #endregion
                    }

                    init = true; // try it only once
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("TryCorrectGoogleVersions failed: " + ex.ToString());
                }
            }
        }

        internal void GetSecureWords(GPoint pos, out string sec1, out string sec2)
        {
            sec1 = string.Empty; // after &x=...
            sec2 = string.Empty; // after &zoom=...
            int seclen = (int)((pos.X * 3) + pos.Y) % 8;
            sec2 = SecureWord.Substring(0, seclen);
            if (pos.Y >= 10000 && pos.Y < 100000)
            {
                sec1 = Sec1;
            }
        }

        static readonly string Sec1 = "&s=";

        #region RoutingProvider Members

        public MapRoute GetRoute(PointLatLng start, PointLatLng end, bool avoidHighways, bool walkingMode, int Zoom)
        {
            string tooltip;
            int numLevels;
            int zoomFactor;
            MapRoute ret = null;
            List<PointLatLng> points = GetRoutePoints(MakeRouteUrl(start, end, LanguageStr, avoidHighways, walkingMode), Zoom, out tooltip, out numLevels, out zoomFactor);
            if (points != null)
            {
                ret = new MapRoute(points, tooltip);
            }
            return ret;
        }

        public MapRoute GetRoute(string start, string end, bool avoidHighways, bool walkingMode, int Zoom)
        {
            string tooltip;
            int numLevels;
            int zoomFactor;
            MapRoute ret = null;
            List<PointLatLng> points = GetRoutePoints(MakeRouteUrl(start, end, LanguageStr, avoidHighways, walkingMode), Zoom, out tooltip, out numLevels, out zoomFactor);
            if (points != null)
            {
                ret = new MapRoute(points, tooltip);
            }
            return ret;
        }

        #region -- internals --

        string MakeRouteUrl(PointLatLng start, PointLatLng end, string language, bool avoidHighways, bool walkingMode)
        {
            string opt = walkingMode ? WalkingStr : (avoidHighways ? RouteWithoutHighwaysStr : RouteStr);
            return string.Format(CultureInfo.InvariantCulture, RouteUrlFormatPointLatLng, language, opt, start.Lat, start.Lng, end.Lat, end.Lng, Server);
        }

        string MakeRouteUrl(string start, string end, string language, bool avoidHighways, bool walkingMode)
        {
            string opt = walkingMode ? WalkingStr : (avoidHighways ? RouteWithoutHighwaysStr : RouteStr);
            return string.Format(RouteUrlFormatStr, language, opt, start.Replace(' ', '+'), end.Replace(' ', '+'), Server);
        }

        List<PointLatLng> GetRoutePoints(string url, int zoom, out string tooltipHtml, out int numLevel, out int zoomFactor)
        {
            List<PointLatLng> points = null;
            tooltipHtml = string.Empty;
            numLevel = -1;
            zoomFactor = -1;
            try
            {
                string route = GMaps.Instance.UseRouteCache ? Cache.Instance.GetContent(url, CacheType.RouteCache) : string.Empty;

                if (string.IsNullOrEmpty(route))
                {
                    route = GetContentUsingHttp(url);

                    if (!string.IsNullOrEmpty(route))
                    {
                        if (GMaps.Instance.UseRouteCache)
                        {
                            Cache.Instance.SaveContent(url, CacheType.RouteCache, route);
                        }
                    }
                }

                // parse values
                if (!string.IsNullOrEmpty(route))
                {
                    //{
                    //tooltipHtml:" (300\x26#160;km / 2 valandos 59 min.)",
                    //polylines:
                    //[{
                    //   id:"route0",
                    //   points:"cy~rIcvp`ClJ~v@jHpu@N|BB~A?tA_@`J@nAJrB|AhEf@h@~@^pANh@Mr@a@`@_@x@cBPk@ZiBHeDQ{C]wAc@mAqCeEoA_C{@_Cy@iDoEaW}AsJcJ}t@iWowB{C_Vyw@gvGyTyjBu@gHwDoZ{W_zBsX}~BiA_MmAyOcAwOs@yNy@eTk@mVUmTE}PJ_W`@cVd@cQ`@}KjA_V`AeOn@oItAkOdAaKfBaOhDiVbD}RpBuKtEkTtP}q@fr@ypCfCmK|CmNvEqVvCuQ`BgLnAmJ`CgTpA_N~@sLlBwYh@yLp@cSj@e]zFkzKHaVViSf@wZjFwqBt@{Wr@qS`AaUjAgStBkYrEwe@xIuw@`Gmj@rFok@~BkYtCy_@|KccBvBgZjC}[tD__@pDaYjB_MpBuLhGi[fC}KfFcSnEkObFgOrFkOzEoLt[ys@tJeUlIsSbKqXtFiPfKi]rG_W|CiNhDkPfDuQlDoShEuXrEy[nOgiAxF{`@|DoVzFk[fDwPlXupA~CoPfDuQxGcd@l@yEdH{r@xDam@`AiWz@mYtAq~@p@uqAfAqx@|@kZxA}^lBq\\|Be\\lAaO~Dm`@|Gsj@tS_~AhCyUrCeZrByWv@uLlUiyDpA}NdHkn@pGmb@LkAtAoIjDqR`I{`@`BcH|I_b@zJcd@lKig@\\_CbBaIlJ}g@lIoj@pAuJtFoh@~Eqs@hDmv@h@qOfF{jBn@gSxCio@dAuQn@gIVoBjAiOlCqWbCiT`PekAzKiu@~EgYfIya@fA{ExGwWnDkMdHiU|G}R`HgQhRsa@hW}g@jVsg@|a@cbAbJkUxKoYxLa_@`IiZzHu[`DoOXsBhBuJbCwNdBaL`EkYvAwM`CeVtEwj@nDqj@BkAnB{YpGgeAn@eJ`CmYvEid@tBkQpGkd@rE}UxB}JdJo_@nDcNfSan@nS}j@lCeIvDsMbC{J|CyNbAwFfCgPz@uGvBiSdD}`@rFon@nKaqAxDmc@xBuT|Fqc@nC_PrEcUtC_MpFcT`GqQxJmXfXwq@jQgh@hBeGhG_U|BaK|G}[nRikAzIam@tDsYfE}^v@_MbAwKn@oIr@yLrBub@jAoa@b@sRdDmjBx@aZdA}XnAqVpAgTlAqPn@oGvFye@dCeRzGwb@xT_}A`BcPrAoOvCad@jAmXv@eV`BieA~@a[fBg_@`CiZ~A_OhHqk@hHcn@tEwe@rDub@nBoW~@sN|BeZnAgMvDm\\hFs^hSigArFaY`Gc\\`C}OhD}YfByQdAaNbAkOtOu~Cn@wKz@uLfCeY|CkW~B}OhCmO|AcI~A_IvDoPpEyPdImWrDuKnL_YjI{Ptl@qfAle@u|@xI}PbImQvFwMbGgOxFkOpdAosCdD_KxGsU|E}RxFcXhCwNjDwTvBiPfBqOrAyMfBcTxAaVhAwVrCy_Al@iPt@_OtA}Q`AuJ`AgIzAkK`EoUtBsJhCaKxCaKdDaKhQeg@jGiRfGaSrFyR`HsWvL}f@xp@grC`Sq|@pEsVdAoGjF{XlkAgwHxHgj@|Jex@fg@qlEjQs{AdHwh@zDkVhEkVzI_e@v}AgzHpK_l@tE}YtEy[rC}TpFme@jg@cpEbF{d@~BoXfBqUbAyOx@yN|Ao]bAo[tIazC`@iLb@aJ~AkWbBgRdBgPjA{IdCePlAmHfBmJdCiL~CuM|DoNxhDezKdDkLvBoInFqVbCuMxBqNnAeJ~CwXdBoSb^crElFsl@`Dy[zDu^xBiRzc@aaE|Fsd@vCkShDmTpG}^lD}QzDoR|zAcdHvIob@dKoj@jDmSlKiq@xVacBhEqXnBqL|Ga^zJke@`y@ktD~Mop@tP}_AdOg`AtCiQxCyOlDkPfDoN`GiTfGkRjEwLvEsL|HkQtEkJdE{HrwAkaCrT{a@rpDiuHtE_KvLuV|{AwaDzAqCb@mAf{Ac`D~FqL~y@_fBlNmZbGaNtF}Mpn@s~AlYss@dFgK|DoGhBoCrDuE~AcBtGaGnByAnDwBnCwAfDwAnFaBjGkA~[{E`iEkn@pQaDvIwBnIiCl\\qLn}J{pDhMcGrFcDhGeEvoDehC|AsArCwChBaC`C_EzC_HbBcFd@uB`@qAn@gDdB}Kz@}Hn@iPjByx@jDcvAj@}RDsEn@yTv@a]VcPtEamFBcHT_LNkEdAiShDsi@`GudAbFgx@`@iKdP}yFhBgs@p@yRjCo_AJwCXeEb@uEz@_H|@yEnBqHrCiIpAmE`o@qhBxC_IjIuVdIcXh{AgmG`i@_{BfCuLrhAssGfFeXxbBklInCsN|_AoiGpGs_@pl@w}Czy@_kEvG{]h}@ieFbQehAdHye@lPagA|Eu\\tAmI|CwWjn@mwGj@eH|]azFl@kPjAqd@jJe|DlD}vAxAeh@@eBvVk}JzIkqDfE_aBfA{YbBk[zp@e}LhAaObCeUlAuIzAeJrb@q`CjCcOnAaIpBwOtBkTjDsg@~AiPvBwOlAcH|AkIlCkLlYudApDoN`BgHhBaJvAeIvAqJbAuHrBqQbAsLx@oL`MwrCXkFr@uJh@{FhBsOvXwoB|EqVdBmHxC}KtCcJtDgKjDoIxE}JdHcMdCuDdIoKlmB}|BjJuMfFgIlE{HlEyIdEeJ~FaOvCgInCuI`EmN`J}]rEsP`EuMzCoIxGwPpi@cnAhGgPzCiJvFmRrEwQbDyOtCoPbDwTxDq\\rAsK`BgLhB{KxBoLfCgLjDqKdBqEfEkJtSy^`EcJnDuJjAwDrCeK\\}AjCaNr@qEjAaJtNaqAdCqQ`BsItS}bAbQs{@|Kor@xBmKz}@}uDze@{zAjk@}fBjTsq@r@uCd@aDFyCIwCWcCY}Aq_@w|A{AwF_DyHgHwOgu@m_BSb@nFhL",
                    //   levels:"B?@?????@?@???A???@?@????@??@????????@????@???A????@????@??@???@??@???A???@??@???A??@???@????A??@???@??@????@??@???@????@???@??A@?@???@????A????@??@?@???@???????@??@?@????@????@?A??@???@????@??@?A??????@???????@??A???@??@???@??@????@??@?@?????@?@?A?@????@???@??@??@????@?@??@?@??@??????@???@?@????@???B???@??@??????@??@???A?????@????@???A??@??????@??@??A?@???@???@??A????@???@???@????A????@@??A???@???@??@??A????@??????@??@???@???B????@?@????????@????@????A?????@????@??A???@???@???B???@?????@???@????@????@???A???????@??A@??@?@??@@?????A?@@????????@??@?A????@?????@???@???@???@???@?@?A???@??@?@??@???@?????@???A??@???????@????@???@????@????@@???A????@?@??@?B",
                    //   numLevels:4,
                    //   zoomFactor:16
                    //}]
                    //}

                    #region -- title --
                    int tooltipEnd = 0;
                    {
                        int x = route.IndexOf("tooltipHtml:") + 13;
                        if (x >= 13)
                        {
                            tooltipEnd = route.IndexOf("\"", x + 1);
                            if (tooltipEnd > 0)
                            {
                                int l = tooltipEnd - x;
                                if (l > 0)
                                {
                                    tooltipHtml = route.Substring(x, l).Replace(@"\x26#160;", " ");
                                }
                            }
                        }
                    }
                    #endregion

                    #region -- points --
                    int pointsEnd = 0;
                    {
                        int x = route.IndexOf("points:", tooltipEnd >= 0 ? tooltipEnd : 0) + 8;
                        if (x >= 8)
                        {
                            pointsEnd = route.IndexOf("\"", x + 1);
                            if (pointsEnd > 0)
                            {
                                int l = pointsEnd - x;
                                if (l > 0)
                                {
                                    /*
                                    while(l % 5 != 0)
                                    {
                                       l--;
                                    }
                                    */

                                    points = new List<PointLatLng>();
                                    DecodePointsInto(points, route.Substring(x, l));
                                }
                            }
                        }
                    }
                    #endregion

                    #region -- levels --
                    string levels = string.Empty;
                    int levelsEnd = 0;
                    {
                        int x = route.IndexOf("levels:", pointsEnd >= 0 ? pointsEnd : 0) + 8;
                        if (x >= 8)
                        {
                            levelsEnd = route.IndexOf("\"", x + 1);
                            if (levelsEnd > 0)
                            {
                                int l = levelsEnd - x;
                                if (l > 0)
                                {
                                    levels = route.Substring(x, l);
                                }
                            }
                        }
                    }
                    #endregion

                    #region -- numLevel --
                    int numLevelsEnd = 0;
                    {
                        int x = route.IndexOf("numLevels:", levelsEnd >= 0 ? levelsEnd : 0) + 10;
                        if (x >= 10)
                        {
                            numLevelsEnd = route.IndexOf(",", x);
                            if (numLevelsEnd > 0)
                            {
                                int l = numLevelsEnd - x;
                                if (l > 0)
                                {
                                    numLevel = int.Parse(route.Substring(x, l));
                                }
                            }
                        }
                    }
                    #endregion

                    #region -- zoomFactor --
                    {
                        int x = route.IndexOf("zoomFactor:", numLevelsEnd >= 0 ? numLevelsEnd : 0) + 11;
                        if (x >= 11)
                        {
                            int end = route.IndexOf("}", x);
                            if (end > 0)
                            {
                                int l = end - x;
                                if (l > 0)
                                {
                                    zoomFactor = int.Parse(route.Substring(x, l));
                                }
                            }
                        }
                    }
                    #endregion

                    #region -- trim point overload --
                    if (points != null && numLevel > 0 && !string.IsNullOrEmpty(levels))
                    {
                        if (points.Count - levels.Length > 0)
                        {
                            points.RemoveRange(levels.Length, points.Count - levels.Length);
                        }

                        //http://facstaff.unca.edu/mcmcclur/GoogleMaps/EncodePolyline/description.html
                        //
                        string allZlevels = "TSRPONMLKJIHGFEDCBA@?";
                        if (numLevel > allZlevels.Length)
                        {
                            numLevel = allZlevels.Length;
                        }

                        // used letters in levels string
                        string pLevels = allZlevels.Substring(allZlevels.Length - numLevel);

                        // remove useless points at zoom
                        {
                            List<PointLatLng> removedPoints = new List<PointLatLng>();

                            for (int i = 0; i < levels.Length; i++)
                            {
                                int zi = pLevels.IndexOf(levels [i]);
                                if (zi > 0)
                                {
                                    if (zi * numLevel > zoom)
                                    {
                                        removedPoints.Add(points [i]);
                                    }
                                }
                            }

                            foreach (var v in removedPoints)
                            {
                                points.Remove(v);
                            }
                            removedPoints.Clear();
                            removedPoints = null;
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                points = null;
                Debug.WriteLine("GetRoutePoints: " + ex);
            }
            return points;
        }

        static readonly string RouteUrlFormatPointLatLng = "http://maps.{6}/maps?f=q&output=dragdir&doflg=p&hl={0}{1}&q=&saddr=@{2},{3}&daddr=@{4},{5}";
        static readonly string RouteUrlFormatStr = "http://maps.{4}/maps?f=q&output=dragdir&doflg=p&hl={0}{1}&q=&saddr=@{2}&daddr=@{3}";

        static readonly string WalkingStr = "&mra=ls&dirflg=w";
        static readonly string RouteWithoutHighwaysStr = "&mra=ls&dirflg=dh";
        static readonly string RouteStr = "&mra=ls&dirflg=d";

        #endregion

        #endregion

        #region GeocodingProvider Members

        public GeoCoderStatusCode GetPoints(string keywords, out List<PointLatLng> pointList)
        {
            return GetLatLngFromGeocoderUrl(MakeGeocoderUrl(keywords, LanguageStr), out pointList);
        }

        public PointLatLng? GetPoint(string keywords, out GeoCoderStatusCode status)
        {
            List<PointLatLng> pointList;
            status = GetPoints(keywords, out pointList);
            return pointList != null && pointList.Count > 0 ? pointList [0] : (PointLatLng?)null;
        }

        /// <summary>
        /// NotImplemented
        /// </summary>
        /// <param name="placemark"></param>
        /// <param name="pointList"></param>
        /// <returns></returns>
        public GeoCoderStatusCode GetPoints(Placemark placemark, out List<PointLatLng> pointList)
        {
            throw new NotImplementedException("use GetPoints(string keywords...");
        }

        /// <summary>
        /// NotImplemented
        /// </summary>
        /// <param name="placemark"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public PointLatLng? GetPoint(Placemark placemark, out GeoCoderStatusCode status)
        {
            throw new NotImplementedException("use GetPoint(string keywords...");
        }

        public GeoCoderStatusCode GetPlacemarks(PointLatLng location, out List<Placemark> placemarkList)
        {
            return GetPlacemarkFromReverseGeocoderUrl(MakeReverseGeocoderUrl(location, LanguageStr), out placemarkList);
        }

        public Placemark? GetPlacemark(PointLatLng location, out GeoCoderStatusCode status)
        {
            List<Placemark> pointList;
            status = GetPlacemarks(location, out pointList);
            return pointList != null && pointList.Count > 0 ? pointList [0] : (Placemark?)null;
        }

        #region -- internals --

        // The Coogle Geocoding API: http://tinyurl.com/cdlj889

        string MakeGeocoderUrl(string keywords, string language)
        {
            if (string.IsNullOrEmpty(keywords))
                return string.Empty;

            return string.Format(CultureInfo.InvariantCulture, GeocoderUrlFormat, ServerAPIs, Uri.EscapeDataString(keywords).Replace(' ', '+'), language);
        }

        string MakeReverseGeocoderUrl(PointLatLng pt, string language)
        {
            return string.Format(CultureInfo.InvariantCulture, ReverseGeocoderUrlFormat, ServerAPIs, pt.Lat, pt.Lng, language);
        }

        GeoCoderStatusCode GetLatLngFromGeocoderUrl(string url, out List<PointLatLng> pointList)
        {
            var status = GeoCoderStatusCode.Unknow;
            pointList = null;

            try
            {
                string geo = GMaps.Instance.UseGeocoderCache ? Cache.Instance.GetContent(url, CacheType.GeocoderCache) : string.Empty;

                bool cache = false;

                if (string.IsNullOrEmpty(geo))
                {
                    string urls = url;

                    // Must provide either API key or Maps for Work credentials.
                    if (!string.IsNullOrEmpty(ClientId))
                    {
                        urls = GetSignedUri(url);
                    }
                    else if (!string.IsNullOrEmpty(ApiKey))
                    {
                        urls += "&key=" + ApiKey;
                    }

                    geo = GetContentUsingHttp(urls);

                    if (!string.IsNullOrEmpty(geo))
                    {
                        cache = true;
                    }
                }

                if (!string.IsNullOrEmpty(geo))
                {
                    if (geo.StartsWith("<?xml"))
                    {
                        #region -- xml response --
                        //<?xml version="1.0" encoding="UTF-8"?>
                        //<GeocodeResponse>
                        // <status>OK</status>
                        // <result>
                        //  <type>locality</type>
                        //  <type>political</type>
                        //  <formatted_address>Vilnius, Lithuania</formatted_address>
                        //  <address_component>
                        //   <long_name>Vilnius</long_name>
                        //   <short_name>Vilnius</short_name>
                        //   <type>locality</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Vilniaus miesto savivaldyb??</long_name>
                        //   <short_name>Vilniaus m. sav.</short_name>
                        //   <type>administrative_area_level_2</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Vilnius County</long_name>
                        //   <short_name>Vilnius County</short_name>
                        //   <type>administrative_area_level_1</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Lithuania</long_name>
                        //   <short_name>LT</short_name>
                        //   <type>country</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <geometry>
                        //   <location>
                        //    <lat>54.6871555</lat>
                        //    <lng>25.2796514</lng>
                        //   </location>
                        //   <location_type>APPROXIMATE</location_type>
                        //   <viewport>
                        //    <southwest>
                        //     <lat>54.5677980</lat>
                        //     <lng>25.0243760</lng>
                        //    </southwest>
                        //    <northeast>
                        //     <lat>54.8325440</lat>
                        //     <lng>25.4814883</lng>
                        //    </northeast>
                        //   </viewport>
                        //   <bounds>
                        //    <southwest>
                        //     <lat>54.5677980</lat>
                        //     <lng>25.0243760</lng>
                        //    </southwest>
                        //    <northeast>
                        //     <lat>54.8325440</lat>
                        //     <lng>25.4814883</lng>
                        //    </northeast>
                        //   </bounds>
                        //  </geometry>
                        // </result>
                        // <result>
                        //  <type>airport</type>
                        //  <type>transit_station</type>
                        //  <type>establishment</type>
                        //  <formatted_address>Vilnius International Airport (VNO), 10A, Vilnius, Lithuania</formatted_address>
                        //  <address_component>
                        //   <long_name>Vilnius International Airport</long_name>
                        //   <short_name>Vilnius International Airport</short_name>
                        //   <type>establishment</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>10A</long_name>
                        //   <short_name>10A</short_name>
                        //   <type>street_number</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Vilnius</long_name>
                        //   <short_name>Vilnius</short_name>
                        //   <type>locality</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Vilniaus miesto savivaldyb??</long_name>
                        //   <short_name>Vilniaus m. sav.</short_name>
                        //   <type>administrative_area_level_2</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Vilnius County</long_name>
                        //   <short_name>Vilnius County</short_name>
                        //   <type>administrative_area_level_1</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Lithuania</long_name>
                        //   <short_name>LT</short_name>
                        //   <type>country</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <geometry>
                        //   <location>
                        //    <lat>54.6369440</lat>
                        //    <lng>25.2877780</lng>
                        //   </location>
                        //   <location_type>APPROXIMATE</location_type>
                        //   <viewport>
                        //    <southwest>
                        //     <lat>54.6158331</lat>
                        //     <lng>25.2723832</lng>
                        //    </southwest>
                        //    <northeast>
                        //     <lat>54.6538331</lat>
                        //     <lng>25.3034219</lng>
                        //    </northeast>
                        //   </viewport>
                        //   <bounds>
                        //    <southwest>
                        //     <lat>54.6158331</lat>
                        //     <lng>25.2723832</lng>
                        //    </southwest>
                        //    <northeast>
                        //     <lat>54.6538331</lat>
                        //     <lng>25.3034219</lng>
                        //    </northeast>
                        //   </bounds>
                        //  </geometry>
                        // </result>
                        //</GeocodeResponse>

                        #endregion

                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(geo);

                        XmlNode nn = doc.SelectSingleNode("//status");
                        if (nn != null)
                        {
                            if (nn.InnerText != "OK")
                            {
                                Debug.WriteLine("GetLatLngFromGeocoderUrl: " + nn.InnerText);
                            }
                            else
                            {
                                status = GeoCoderStatusCode.G_GEO_SUCCESS;

                                if (cache && GMaps.Instance.UseGeocoderCache)
                                {
                                    Cache.Instance.SaveContent(url, CacheType.GeocoderCache, geo);
                                }

                                pointList = new List<PointLatLng>();

                                XmlNodeList l = doc.SelectNodes("//result");
                                if (l != null)
                                {
                                    foreach (XmlNode n in l)
                                    {
                                        nn = n.SelectSingleNode("geometry/location/lat");
                                        if (nn != null)
                                        {
                                            double lat = double.Parse(nn.InnerText, CultureInfo.InvariantCulture);

                                            nn = n.SelectSingleNode("geometry/location/lng");
                                            if (nn != null)
                                            {
                                                double lng = double.Parse(nn.InnerText, CultureInfo.InvariantCulture);
                                                pointList.Add(new PointLatLng(lat, lng));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                status = GeoCoderStatusCode.ExceptionInCode;
                Debug.WriteLine("GetLatLngFromGeocoderUrl: " + ex);
            }

            return status;
        }

        GeoCoderStatusCode GetPlacemarkFromReverseGeocoderUrl(string url, out List<Placemark> placemarkList)
        {
            GeoCoderStatusCode status = GeoCoderStatusCode.Unknow;
            placemarkList = null;

            try
            {
                string reverse = GMaps.Instance.UsePlacemarkCache ? Cache.Instance.GetContent(url, CacheType.PlacemarkCache) : string.Empty;

                bool cache = false;

                if (string.IsNullOrEmpty(reverse))
                {
                    string urls = url;

                    // Must provide either API key or Maps for Work credentials.
                    if (!string.IsNullOrEmpty(ClientId))
                    {
                        urls = GetSignedUri(url);
                    }
                    else if (!string.IsNullOrEmpty(ApiKey))
                    {
                        urls += "&key=" + ApiKey;
                    }

                    reverse = GetContentUsingHttp(urls);

                    if (!string.IsNullOrEmpty(reverse))
                    {
                        cache = true;
                    }
                }

                if (!string.IsNullOrEmpty(reverse))
                {
                    if (reverse.StartsWith("<?xml"))
                    {
                        #region -- xml response --
                        //<?xml version="1.0" encoding="UTF-8"?>
                        //<GeocodeResponse>
                        // <status>OK</status>
                        // <result>
                        //  <type>street_address</type>
                        //  <formatted_address>Tuskul??n?? gatv?? 2, Vilnius 09213, Lithuania</formatted_address>
                        //  <address_component>
                        //   <long_name>2</long_name>
                        //   <short_name>2</short_name>
                        //   <type>street_number</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Tuskul??n?? gatv??</long_name>
                        //   <short_name>Tuskul??n?? g.</short_name>
                        //   <type>route</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Vilnius</long_name>
                        //   <short_name>Vilnius</short_name>
                        //   <type>locality</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Vilniaus miesto savivaldyb??</long_name>
                        //   <short_name>Vilniaus m. sav.</short_name>
                        //   <type>administrative_area_level_2</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Vilnius County</long_name>
                        //   <short_name>Vilnius County</short_name>
                        //   <type>administrative_area_level_1</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Lithuania</long_name>
                        //   <short_name>LT</short_name>
                        //   <type>country</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>09213</long_name>
                        //   <short_name>09213</short_name>
                        //   <type>postal_code</type>
                        //  </address_component>
                        //  <geometry>
                        //   <location>
                        //    <lat>54.6963339</lat>
                        //    <lng>25.2968939</lng>
                        //   </location>
                        //   <location_type>ROOFTOP</location_type>
                        //   <viewport>
                        //    <southwest>
                        //     <lat>54.6949849</lat>
                        //     <lng>25.2955449</lng>
                        //    </southwest>
                        //    <northeast>
                        //     <lat>54.6976829</lat>
                        //     <lng>25.2982429</lng>
                        //    </northeast>
                        //   </viewport>
                        //  </geometry>
                        // </result>
                        // <result>
                        //  <type>postal_code</type>
                        //  <formatted_address>Vilnius 09213, Lithuania</formatted_address>
                        //  <address_component>
                        //   <long_name>09213</long_name>
                        //   <short_name>09213</short_name>
                        //   <type>postal_code</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Vilnius</long_name>
                        //   <short_name>Vilnius</short_name>
                        //   <type>locality</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Vilniaus miesto savivaldyb??</long_name>
                        //   <short_name>Vilniaus m. sav.</short_name>
                        //   <type>administrative_area_level_2</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Vilnius County</long_name>
                        //   <short_name>Vilnius County</short_name>
                        //   <type>administrative_area_level_1</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Lithuania</long_name>
                        //   <short_name>LT</short_name>
                        //   <type>country</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <geometry>
                        //   <location>
                        //    <lat>54.6963032</lat>
                        //    <lng>25.2967390</lng>
                        //   </location>
                        //   <location_type>APPROXIMATE</location_type>
                        //   <viewport>
                        //    <southwest>
                        //     <lat>54.6950889</lat>
                        //     <lng>25.2958851</lng>
                        //    </southwest>
                        //    <northeast>
                        //     <lat>54.6977869</lat>
                        //     <lng>25.2985830</lng>
                        //    </northeast>
                        //   </viewport>
                        //   <bounds>
                        //    <southwest>
                        //     <lat>54.6956179</lat>
                        //     <lng>25.2958871</lng>
                        //    </southwest>
                        //    <northeast>
                        //     <lat>54.6972579</lat>
                        //     <lng>25.2985810</lng>
                        //    </northeast>
                        //   </bounds>
                        //  </geometry>
                        // </result>
                        // <result>
                        //  <type>neighborhood</type>
                        //  <type>political</type>
                        //  <formatted_address>??irm??nai, Vilnius, Lithuania</formatted_address>
                        //  <address_component>
                        //   <long_name>??irm??nai</long_name>
                        //   <short_name>??irm??nai</short_name>
                        //   <type>neighborhood</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Vilnius</long_name>
                        //   <short_name>Vilnius</short_name>
                        //   <type>locality</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Vilniaus miesto savivaldyb??</long_name>
                        //   <short_name>Vilniaus m. sav.</short_name>
                        //   <type>administrative_area_level_2</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Vilnius County</long_name>
                        //   <short_name>Vilnius County</short_name>
                        //   <type>administrative_area_level_1</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Lithuania</long_name>
                        //   <short_name>LT</short_name>
                        //   <type>country</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <geometry>
                        //   <location>
                        //    <lat>54.7117424</lat>
                        //    <lng>25.2974345</lng>
                        //   </location>
                        //   <location_type>APPROXIMATE</location_type>
                        //   <viewport>
                        //    <southwest>
                        //     <lat>54.6888939</lat>
                        //     <lng>25.2838700</lng>
                        //    </southwest>
                        //    <northeast>
                        //     <lat>54.7304441</lat>
                        //     <lng>25.3133630</lng>
                        //    </northeast>
                        //   </viewport>
                        //   <bounds>
                        //    <southwest>
                        //     <lat>54.6888939</lat>
                        //     <lng>25.2838700</lng>
                        //    </southwest>
                        //    <northeast>
                        //     <lat>54.7304441</lat>
                        //     <lng>25.3133630</lng>
                        //    </northeast>
                        //   </bounds>
                        //  </geometry>
                        // </result>
                        // <result>
                        //  <type>administrative_area_level_3</type>
                        //  <type>political</type>
                        //  <formatted_address>??irm??n?? seni??nija, Lithuania</formatted_address>
                        //  <address_component>
                        //   <long_name>??irm??n?? seni??nija</long_name>
                        //   <short_name>??irm??n?? sen.</short_name>
                        //   <type>administrative_area_level_3</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Vilniaus miesto savivaldyb??</long_name>
                        //   <short_name>Vilniaus m. sav.</short_name>
                        //   <type>administrative_area_level_2</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Vilnius County</long_name>
                        //   <short_name>Vilnius County</short_name>
                        //   <type>administrative_area_level_1</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Lithuania</long_name>
                        //   <short_name>LT</short_name>
                        //   <type>country</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <geometry>
                        //   <location>
                        //    <lat>54.7117424</lat>
                        //    <lng>25.2974345</lng>
                        //   </location>
                        //   <location_type>APPROXIMATE</location_type>
                        //   <viewport>
                        //    <southwest>
                        //     <lat>54.6892135</lat>
                        //     <lng>25.2837150</lng>
                        //    </southwest>
                        //    <northeast>
                        //     <lat>54.7305878</lat>
                        //     <lng>25.3135630</lng>
                        //    </northeast>
                        //   </viewport>
                        //   <bounds>
                        //    <southwest>
                        //     <lat>54.6892135</lat>
                        //     <lng>25.2837150</lng>
                        //    </southwest>
                        //    <northeast>
                        //     <lat>54.7305878</lat>
                        //     <lng>25.3135630</lng>
                        //    </northeast>
                        //   </bounds>
                        //  </geometry>
                        // </result>
                        // <result>
                        //  <type>locality</type>
                        //  <type>political</type>
                        //  <formatted_address>Vilnius, Lithuania</formatted_address>
                        //  <address_component>
                        //   <long_name>Vilnius</long_name>
                        //   <short_name>Vilnius</short_name>
                        //   <type>locality</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Vilniaus miesto savivaldyb??</long_name>
                        //   <short_name>Vilniaus m. sav.</short_name>
                        //   <type>administrative_area_level_2</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Vilnius County</long_name>
                        //   <short_name>Vilnius County</short_name>
                        //   <type>administrative_area_level_1</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Lithuania</long_name>
                        //   <short_name>LT</short_name>
                        //   <type>country</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <geometry>
                        //   <location>
                        //    <lat>54.6871555</lat>
                        //    <lng>25.2796514</lng>
                        //   </location>
                        //   <location_type>APPROXIMATE</location_type>
                        //   <viewport>
                        //    <southwest>
                        //     <lat>54.5677980</lat>
                        //     <lng>25.0243760</lng>
                        //    </southwest>
                        //    <northeast>
                        //     <lat>54.8325440</lat>
                        //     <lng>25.4814883</lng>
                        //    </northeast>
                        //   </viewport>
                        //   <bounds>
                        //    <southwest>
                        //     <lat>54.5677980</lat>
                        //     <lng>25.0243760</lng>
                        //    </southwest>
                        //    <northeast>
                        //     <lat>54.8325440</lat>
                        //     <lng>25.4814883</lng>
                        //    </northeast>
                        //   </bounds>
                        //  </geometry>
                        // </result>
                        // <result>
                        //  <type>administrative_area_level_2</type>
                        //  <type>political</type>
                        //  <formatted_address>Vilniaus miesto savivaldyb??, Lithuania</formatted_address>
                        //  <address_component>
                        //   <long_name>Vilniaus miesto savivaldyb??</long_name>
                        //   <short_name>Vilniaus m. sav.</short_name>
                        //   <type>administrative_area_level_2</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Vilnius County</long_name>
                        //   <short_name>Vilnius County</short_name>
                        //   <type>administrative_area_level_1</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Lithuania</long_name>
                        //   <short_name>LT</short_name>
                        //   <type>country</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <geometry>
                        //   <location>
                        //    <lat>54.6759715</lat>
                        //    <lng>25.2867413</lng>
                        //   </location>
                        //   <location_type>APPROXIMATE</location_type>
                        //   <viewport>
                        //    <southwest>
                        //     <lat>54.5677980</lat>
                        //     <lng>25.0243760</lng>
                        //    </southwest>
                        //    <northeast>
                        //     <lat>54.8325440</lat>
                        //     <lng>25.4814883</lng>
                        //    </northeast>
                        //   </viewport>
                        //   <bounds>
                        //    <southwest>
                        //     <lat>54.5677980</lat>
                        //     <lng>25.0243760</lng>
                        //    </southwest>
                        //    <northeast>
                        //     <lat>54.8325440</lat>
                        //     <lng>25.4814883</lng>
                        //    </northeast>
                        //   </bounds>
                        //  </geometry>
                        // </result>
                        // <result>
                        //  <type>administrative_area_level_1</type>
                        //  <type>political</type>
                        //  <formatted_address>Vilnius County, Lithuania</formatted_address>
                        //  <address_component>
                        //   <long_name>Vilnius County</long_name>
                        //   <short_name>Vilnius County</short_name>
                        //   <type>administrative_area_level_1</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <address_component>
                        //   <long_name>Lithuania</long_name>
                        //   <short_name>LT</short_name>
                        //   <type>country</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <geometry>
                        //   <location>
                        //    <lat>54.8086502</lat>
                        //    <lng>25.2182138</lng>
                        //   </location>
                        //   <location_type>APPROXIMATE</location_type>
                        //   <viewport>
                        //    <southwest>
                        //     <lat>54.1276599</lat>
                        //     <lng>24.3863751</lng>
                        //    </southwest>
                        //    <northeast>
                        //     <lat>55.5174369</lat>
                        //     <lng>26.7602130</lng>
                        //    </northeast>
                        //   </viewport>
                        //   <bounds>
                        //    <southwest>
                        //     <lat>54.1276599</lat>
                        //     <lng>24.3863751</lng>
                        //    </southwest>
                        //    <northeast>
                        //     <lat>55.5174369</lat>
                        //     <lng>26.7602130</lng>
                        //    </northeast>
                        //   </bounds>
                        //  </geometry>
                        // </result>
                        // <result>
                        //  <type>country</type>
                        //  <type>political</type>
                        //  <formatted_address>Lithuania</formatted_address>
                        //  <address_component>
                        //   <long_name>Lithuania</long_name>
                        //   <short_name>LT</short_name>
                        //   <type>country</type>
                        //   <type>political</type>
                        //  </address_component>
                        //  <geometry>
                        //   <location>
                        //    <lat>55.1694380</lat>
                        //    <lng>23.8812750</lng>
                        //   </location>
                        //   <location_type>APPROXIMATE</location_type>
                        //   <viewport>
                        //    <southwest>
                        //     <lat>53.8968787</lat>
                        //     <lng>20.9543679</lng>
                        //    </southwest>
                        //    <northeast>
                        //     <lat>56.4503209</lat>
                        //     <lng>26.8355913</lng>
                        //    </northeast>
                        //   </viewport>
                        //   <bounds>
                        //    <southwest>
                        //     <lat>53.8968787</lat>
                        //     <lng>20.9543679</lng>
                        //    </southwest>
                        //    <northeast>
                        //     <lat>56.4503209</lat>
                        //     <lng>26.8355913</lng>
                        //    </northeast>
                        //   </bounds>
                        //  </geometry>
                        // </result>
                        //</GeocodeResponse>

                        #endregion

                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(reverse);

                        XmlNode nn = doc.SelectSingleNode("//status");
                        if (nn != null)
                        {
                            if (nn.InnerText != "OK")
                            {
                                Debug.WriteLine("GetPlacemarkFromReverseGeocoderUrl: " + nn.InnerText);
                            }
                            else
                            {
                                status = GeoCoderStatusCode.G_GEO_SUCCESS;

                                if (cache && GMaps.Instance.UsePlacemarkCache)
                                {
                                    Cache.Instance.SaveContent(url, CacheType.PlacemarkCache, reverse);
                                }

                                placemarkList = new List<Placemark>();

                                #region -- placemarks --
                                XmlNodeList l = doc.SelectNodes("//result");
                                if (l != null)
                                {
                                    foreach (XmlNode n in l)
                                    {
                                        nn = n.SelectSingleNode("formatted_address");
                                        if (nn != null)
                                        {
                                            var ret = new Placemark(nn.InnerText);

                                            nn = n.SelectSingleNode("type");

                                            if (nn != null)
                                            {
                                                Debug.WriteLine("type: " + nn.InnerText);
                                            }

                                            // TODO: fill Placemark details

                                            XmlNodeList acl = n.SelectNodes("address_component");
                                            foreach (XmlNode ac in acl)
                                            {
                                                nn = ac.SelectSingleNode("type");
                                                if (nn != null)
                                                {
                                                    var type = nn.InnerText;
                                                    Debug.Write(" - [" + type + "], ");

                                                    nn = ac.SelectSingleNode("long_name");
                                                    if (nn != null)
                                                    {
                                                        switch (type)
                                                        {
                                                            case "street_address":
                                                            {
                                                                ret.StreetNumber = nn.InnerText;
                                                            }
                                                            break;

                                                            case "route":
                                                            {
                                                                ret.ThoroughfareName = nn.InnerText;
                                                            }
                                                            break;

                                                            case "postal_code":
                                                            {
                                                                ret.PostalCodeNumber = nn.InnerText;
                                                            }
                                                            break;

                                                            case "country":
                                                            {
                                                                ret.CountryName = nn.InnerText;
                                                            }
                                                            break;

                                                            case "locality":
                                                            {
                                                                ret.LocalityName = nn.InnerText;
                                                            }
                                                            break;

                                                            case "administrative_area_level_2":
                                                            {
                                                              ret.DistrictName = nn.InnerText;
                                                            }
                                                            break;

                                                            case "administrative_area_level_1":
                                                            {
                                                                ret.AdministrativeAreaName = nn.InnerText;
                                                            }
                                                            break;

                                                            case "administrative_area_level_3":
                                                            {
                                                                ret.SubAdministrativeAreaName = nn.InnerText;
                                                            }
                                                            break;

                                                            case "neighborhood":
                                                            {
                                                                ret.Neighborhood = nn.InnerText;
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                            }                                            

                                            placemarkList.Add(ret);
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                status = GeoCoderStatusCode.ExceptionInCode;
                placemarkList = null;
                Debug.WriteLine("GetPlacemarkReverseGeocoderUrl: " + ex.ToString());
            }

            return status;
        }

        static readonly string ReverseGeocoderUrlFormat = "http://maps.{0}/maps/api/geocode/xml?latlng={1},{2}&language={3}&sensor=false";
        static readonly string GeocoderUrlFormat = "http://maps.{0}/maps/api/geocode/xml?address={1}&language={2}&sensor=false";

        #endregion

        #region DirectionsProvider Members

        public DirectionsStatusCode GetDirections(out GDirections direction, PointLatLng start, PointLatLng end, bool avoidHighways, bool avoidTolls, bool walkingMode, bool sensor, bool metric)
        {
            return GetDirectionsUrl(MakeDirectionsUrl(start, end, LanguageStr, avoidHighways, avoidTolls, walkingMode, sensor, metric), out direction);
        }

        public DirectionsStatusCode GetDirections(out GDirections direction, string start, string end, bool avoidHighways, bool avoidTolls, bool walkingMode, bool sensor, bool metric)
        {
            return GetDirectionsUrl(MakeDirectionsUrl(start, end, LanguageStr, avoidHighways, avoidTolls, walkingMode, sensor, metric), out direction);
        }

        /// <summary>
        /// NotImplemented
        /// </summary>
        /// <param name="status"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="avoidHighways"></param>
        /// <param name="avoidTolls"></param>
        /// <param name="walkingMode"></param>
        /// <param name="sensor"></param>
        /// <param name="metric"></param>
        /// <returns></returns>
        public IEnumerable<GDirections> GetDirections(out DirectionsStatusCode status, string start, string end, bool avoidHighways, bool avoidTolls, bool walkingMode, bool sensor, bool metric)
        {
            // TODO: add alternative directions

            throw new NotImplementedException();
        }

        /// <summary>
        /// NotImplemented
        /// </summary>
        /// <param name="status"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="avoidHighways"></param>
        /// <param name="avoidTolls"></param>
        /// <param name="walkingMode"></param>
        /// <param name="sensor"></param>
        /// <param name="metric"></param>
        /// <returns></returns>
        public IEnumerable<GDirections> GetDirections(out DirectionsStatusCode status, PointLatLng start, PointLatLng end, bool avoidHighways, bool avoidTolls, bool walkingMode, bool sensor, bool metric)
        {
            // TODO: add alternative directions

            throw new NotImplementedException();
        }

        public DirectionsStatusCode GetDirections(out GDirections direction, PointLatLng start, IEnumerable<PointLatLng> wayPoints, PointLatLng end, bool avoidHighways, bool avoidTolls, bool walkingMode, bool sensor, bool metric)
        {
            return GetDirectionsUrl(MakeDirectionsUrl(start, wayPoints, end, LanguageStr, avoidHighways, avoidTolls, walkingMode, sensor, metric), out direction);
        }

        public DirectionsStatusCode GetDirections(out GDirections direction, string start, IEnumerable<string> wayPoints, string end, bool avoidHighways, bool avoidTolls, bool walkingMode, bool sensor, bool metric)
        {
            return GetDirectionsUrl(MakeDirectionsUrl(start, wayPoints, end, LanguageStr, avoidHighways, avoidTolls, walkingMode, sensor, metric), out direction);
        }

        #region -- internals --

        // The Coogle Directions API: http://tinyurl.com/6vv4cac

        string MakeDirectionsUrl(PointLatLng start, PointLatLng end, string language, bool avoidHighways, bool avoidTolls, bool walkingMode, bool sensor, bool metric)
        {
            string av = (avoidHighways ? "&avoid=highways" : string.Empty) + (avoidTolls ? "&avoid=tolls" : string.Empty); // 6
            string mt = "&units=" + (metric ? "metric" : "imperial");     // 7
            string wk = "&mode=" + (walkingMode ? "walking" : "driving"); // 8

            return string.Format(CultureInfo.InvariantCulture, DirectionUrlFormatPoint, start.Lat, start.Lng, end.Lat, end.Lng, sensor.ToString().ToLower(), language, av, mt, wk, ServerAPIs);
        }

        string MakeDirectionsUrl(string start, string end, string language, bool avoidHighways, bool walkingMode, bool avoidTolls, bool sensor, bool metric)
        {
            string av = (avoidHighways ? "&avoid=highways" : string.Empty) + (avoidTolls ? "&avoid=tolls" : string.Empty); // 4
            string mt = "&units=" + (metric ? "metric" : "imperial");     // 5
            string wk = "&mode=" + (walkingMode ? "walking" : "driving"); // 6

            return string.Format(DirectionUrlFormatStr, start.Replace(' ', '+'), end.Replace(' ', '+'), sensor.ToString().ToLower(), language, av, mt, wk, ServerAPIs);
        }

        string MakeDirectionsUrl(PointLatLng start, IEnumerable<PointLatLng> wayPoints, PointLatLng end, string language, bool avoidHighways, bool avoidTolls, bool walkingMode, bool sensor, bool metric)
        {
            string av = (avoidHighways ? "&avoid=highways" : string.Empty) + (avoidTolls ? "&avoid=tolls" : string.Empty); // 6
            string mt = "&units=" + (metric ? "metric" : "imperial"); // 7
            string wk = "&mode=" + (walkingMode ? "walking" : "driving"); // 8

            string wpLatLng = string.Empty;
            int i = 0;
            foreach (var wp in wayPoints)
            {
                wpLatLng += string.Format(CultureInfo.InvariantCulture, i++ == 0 ? "{0},{1}" : "|{0},{1}", wp.Lat, wp.Lng);
            }

            return string.Format(CultureInfo.InvariantCulture, DirectionUrlFormatWaypoint, start.Lat, start.Lng, wpLatLng, sensor.ToString().ToLower(), language, av, mt, wk, ServerAPIs, end.Lat, end.Lng);
        }

        string MakeDirectionsUrl(string start, IEnumerable<string> wayPoints, string end, string language, bool avoidHighways, bool avoidTolls, bool walkingMode, bool sensor, bool metric)
        {
            string av = (avoidHighways ? "&avoid=highways" : string.Empty) + (avoidTolls ? "&avoid=tolls" : string.Empty); // 6
            string mt = "&units=" + (metric ? "metric" : "imperial"); // 7
            string wk = "&mode=" + (walkingMode ? "walking" : "driving"); // 8

            string wpLatLng = string.Empty;
            int i = 0;
            foreach (var wp in wayPoints)
            {
                wpLatLng += string.Format(CultureInfo.InvariantCulture, i++ == 0 ? "{0}" : "|{0}", wp.Replace(' ', '+'));
            }

            return string.Format(CultureInfo.InvariantCulture, DirectionUrlFormatWaypointStr, start.Replace(' ', '+'), wpLatLng, sensor.ToString().ToLower(), language, av, mt, wk, ServerAPIs, end.Replace(' ', '+'));
        }

        DirectionsStatusCode GetDirectionsUrl(string url, out GDirections direction)
        {
            DirectionsStatusCode ret = DirectionsStatusCode.UNKNOWN_ERROR;
            direction = null;

            try
            {
                string kml = GMaps.Instance.UseDirectionsCache ? Cache.Instance.GetContent(url, CacheType.DirectionsCache) : string.Empty;

                bool cache = false;

                if (string.IsNullOrEmpty(kml))
                {
                    string urls = url;

                    // Must provide either API key or Maps for Work credentials.
                    if (!string.IsNullOrEmpty(ClientId))
                    {
                        urls = GetSignedUri(url);
                    }
                    else if (!string.IsNullOrEmpty(ApiKey))
                    {
                        urls += "&key=" + ApiKey;
                    }

                    kml = GetContentUsingHttp(urls);

                    if (!string.IsNullOrEmpty(kml))
                    {
                        cache = true;
                    }
                }

                if (!string.IsNullOrEmpty(kml))
                {
                    #region -- kml response --
                    //<?xml version="1.0" encoding="UTF-8"?>
                    //<DirectionsResponse>
                    // <status>OK</status>
                    // <route>
                    //  <summary>A1/E85</summary>
                    //  <leg>
                    //   <step>
                    //    <travel_mode>DRIVING</travel_mode>
                    //    <start_location>
                    //     <lat>54.6893800</lat>
                    //     <lng>25.2800500</lng>
                    //    </start_location>
                    //    <end_location>
                    //     <lat>54.6907800</lat>
                    //     <lng>25.2798000</lng>
                    //    </end_location>
                    //    <polyline>
                    //     <points>soxlIiohyCYLkCJ}@Vs@?</points>
                    //    </polyline>
                    //    <duration>
                    //     <value>32</value>
                    //     <text>1 min</text>
                    //    </duration>
                    //    <html_instructions>Head &lt;b&gt;north&lt;/b&gt; on &lt;b&gt;Vilniaus gatvė&lt;/b&gt; toward &lt;b&gt;Tilto gatvė&lt;/b&gt;</html_instructions>
                    //    <distance>
                    //     <value>157</value>
                    //     <text>0.2 km</text>
                    //    </distance>
                    //   </step>
                    //   <step>
                    //    <travel_mode>DRIVING</travel_mode>
                    //    <start_location>
                    //     <lat>54.6907800</lat>
                    //     <lng>25.2798000</lng>
                    //    </start_location>
                    //    <end_location>
                    //     <lat>54.6942500</lat>
                    //     <lng>25.2621300</lng>
                    //    </end_location>
                    //    <polyline>
                    //     <points>kxxlIwmhyCmApUF`@GvAYpD{@dGcCjIoIvOuAhDwAtEa@vBUnDAhB?~AThDRxAh@hBtAdC</points>
                    //    </polyline>
                    //    <duration>
                    //     <value>133</value>
                    //     <text>2 mins</text>
                    //    </duration>
                    //    <html_instructions>Turn &lt;b&gt;left&lt;/b&gt; onto &lt;b&gt;A. Goštauto gatvė&lt;/b&gt;</html_instructions>
                    //    <distance>
                    //     <value>1326</value>
                    //     <text>1.3 km</text>
                    //    </distance>
                    //   </step>
                    //   <step>
                    //    <travel_mode>DRIVING</travel_mode>
                    //    <start_location>
                    //     <lat>54.6942500</lat>
                    //     <lng>25.2621300</lng>
                    //    </start_location>
                    //    <end_location>
                    //     <lat>54.6681200</lat>
                    //     <lng>25.2377500</lng>
                    //    </end_location>
                    //    <polyline>
                    //     <points>anylIi_eyC`AwD~@oBLKr@K`U|FdF|@`J^~E[j@Lh@\hB~Bn@tBZhBLrC?zIJ~DzA~OVrELlG^lDdAtDh@hAfApA`EzCvAp@jUpIpAl@bBpAdBpBxA|BdLpV`BxClAbBhBlBbChBpBhAdAXjBHlE_@t@?|@Lt@X</points>
                    //    </polyline>
                    //    <duration>
                    //     <value>277</value>
                    //     <text>5 mins</text>
                    //    </duration>
                    //    <html_instructions>Turn &lt;b&gt;left&lt;/b&gt; to merge onto &lt;b&gt;Geležinio Vilko gatvė&lt;/b&gt;</html_instructions>
                    //    <distance>
                    //     <value>3806</value>
                    //     <text>3.8 km</text>
                    //    </distance>
                    //   </step>
                    //   <step>
                    //    <travel_mode>DRIVING</travel_mode>
                    //    <start_location>
                    //     <lat>54.6681200</lat>
                    //     <lng>25.2377500</lng>
                    //    </start_location>
                    //    <end_location>
                    //     <lat>54.6584100</lat>
                    //     <lng>25.1411300</lng>
                    //    </end_location>
                    //    <polyline>
                    //     <points>wjtlI}f`yC~FhBlFr@jD|A~EbC~VjNxBbBdA`BnvA|zCba@l`Bt@tDTbBJpBBfBMvDaAzF}bBjiF{HnXiHxZ</points>
                    //    </polyline>
                    //    <duration>
                    //     <value>539</value>
                    //     <text>9 mins</text>
                    //    </duration>
                    //    <html_instructions>Continue onto &lt;b&gt;Savanorių prospektas&lt;/b&gt;</html_instructions>
                    //    <distance>
                    //     <value>8465</value>
                    //     <text>8.5 km</text>
                    //    </distance>
                    //   </step>
                    //   <step>
                    //    <travel_mode>DRIVING</travel_mode>
                    //    <start_location>
                    //     <lat>54.6584100</lat>
                    //     <lng>25.1411300</lng>
                    //    </start_location>
                    //    <end_location>
                    //     <lat>54.9358200</lat>
                    //     <lng>23.9260000</lng>
                    //    </end_location>
                    //    <polyline>
                    //     <points>anrlIakmxCiq@|qCuBbLcK~n@wUrkAcPnw@gCnPoQt}AoB`MuAdHmAdFoCtJqClImBxE{DrIkQ|ZcEvIkDzIcDhKyBxJ{EdXuCtS_G`g@mF|\eF`WyDhOiE~NiErMaGpOoj@ppAoE|K_EzKeDtKkEnOsLnd@mDzLgI~U{FrNsEvJoEtI_FpI{J`O_EjFooBf_C{GdJ_FjIsH`OoFhMwH`UcDtL{CzMeDlQmAzHuU~bBiArIwApNaBfWaLfiCoBpYsDf\qChR_FlVqEpQ_ZbfA}CfN{A~HwCtRiAfKmBlVwBx[gBfRcBxMaLdp@sXrzAaE~UqCzRyC`[_q@z|LgC|e@m@vNqp@b}WuLraFo@jPaS~bDmJryAeo@v|G}CnWsm@~`EoKvo@kv@lkEkqBrlKwBvLkNj|@cu@`~EgCnNuiBpcJakAx|GyB`KqdC~fKoIfYicAxtCiDrLu@hDyBjQm@xKoGdxBmQhoGuUn|Dc@nJ[`OW|VaEn|Ee@`X</points>
                    //    </polyline>
                    //    <duration>
                    //     <value>3506</value>
                    //     <text>58 mins</text>
                    //    </duration>
                    //    <html_instructions>Continue onto &lt;b&gt;A1/E85&lt;/b&gt;</html_instructions>
                    //    <distance>
                    //     <value>85824</value>
                    //     <text>85.8 km</text>
                    //    </distance>
                    //   </step>
                    //   <step>
                    //    <travel_mode>DRIVING</travel_mode>
                    //    <start_location>
                    //     <lat>54.9358200</lat>
                    //     <lng>23.9260000</lng>
                    //    </start_location>
                    //    <end_location>
                    //     <lat>54.9376500</lat>
                    //     <lng>23.9195600</lng>
                    //    </end_location>
                    //    <polyline>
                    //     <points>{shnIo``qCQ^MnD[lBgA`DqBdEu@xB}@zJCjB</points>
                    //    </polyline>
                    //    <duration>
                    //     <value>39</value>
                    //     <text>1 min</text>
                    //    </duration>
                    //    <html_instructions>Take the exit toward &lt;b&gt;Senamiestis/Aleksotas&lt;/b&gt;</html_instructions>
                    //    <distance>
                    //     <value>476</value>
                    //     <text>0.5 km</text>
                    //    </distance>
                    //   </step>
                    //   <step>
                    //    <travel_mode>DRIVING</travel_mode>
                    //    <start_location>
                    //     <lat>54.9376500</lat>
                    //     <lng>23.9195600</lng>
                    //    </start_location>
                    //    <end_location>
                    //     <lat>54.9361300</lat>
                    //     <lng>23.9189700</lng>
                    //    </end_location>
                    //    <polyline>
                    //     <points>i_inIgx~pCnHtB</points>
                    //    </polyline>
                    //    <duration>
                    //     <value>28</value>
                    //     <text>1 min</text>
                    //    </duration>
                    //    <html_instructions>Turn &lt;b&gt;left&lt;/b&gt; onto &lt;b&gt;Kleboniškio gatvė&lt;/b&gt;</html_instructions>
                    //    <distance>
                    //     <value>173</value>
                    //     <text>0.2 km</text>
                    //    </distance>
                    //   </step>
                    //   <step>
                    //    <travel_mode>DRIVING</travel_mode>
                    //    <start_location>
                    //     <lat>54.9361300</lat>
                    //     <lng>23.9189700</lng>
                    //    </start_location>
                    //    <end_location>
                    //     <lat>54.9018900</lat>
                    //     <lng>23.8937000</lng>
                    //    </end_location>
                    //    <polyline>
                    //     <points>yuhnIqt~pCvAb@JLrOvExSdHvDdAv`@pIpHnAdl@hLdB`@nDvAtEjDdCvCjLzOvAzBhC`GpHfRbQd^`JpMPt@ClA</points>
                    //    </polyline>
                    //    <duration>
                    //     <value>412</value>
                    //     <text>7 mins</text>
                    //    </duration>
                    //    <html_instructions>Continue onto &lt;b&gt;Jonavos gatvė&lt;/b&gt;</html_instructions>
                    //    <distance>
                    //     <value>4302</value>
                    //     <text>4.3 km</text>
                    //    </distance>
                    //   </step>
                    //   <step>
                    //    <travel_mode>DRIVING</travel_mode>
                    //    <start_location>
                    //     <lat>54.9018900</lat>
                    //     <lng>23.8937000</lng>
                    //    </start_location>
                    //    <end_location>
                    //     <lat>54.8985600</lat>
                    //     <lng>23.8933400</lng>
                    //    </end_location>
                    //    <polyline>
                    //     <points>y_bnIsvypCMf@FnARlAf@zAl@^v@EZ_@pAe@x@k@xBPpA@pAQNSf@oB</points>
                    //    </polyline>
                    //    <duration>
                    //     <value>69</value>
                    //     <text>1 min</text>
                    //    </duration>
                    //    <html_instructions>At the roundabout, take the &lt;b&gt;3rd&lt;/b&gt; exit and stay on &lt;b&gt;Jonavos gatvė&lt;/b&gt;</html_instructions>
                    //    <distance>
                    //     <value>478</value>
                    //     <text>0.5 km</text>
                    //    </distance>
                    //   </step>
                    //   <step>   
                    //    <travel_mode>DRIVING</travel_mode>
                    //    <start_location>
                    //     <lat>54.8985600</lat>
                    //     <lng>23.8933400</lng>
                    //    </start_location>
                    //    <end_location>
                    //     <lat>54.8968500</lat>
                    //     <lng>23.8930000</lng>
                    //    </end_location>
                    //    <polyline>
                    //     <points>_kanIktypCbEx@pCH</points>
                    //    </polyline>
                    //    <duration>
                    //     <value>38</value>
                    //     <text>1 min</text>
                    //    </duration>
                    //    <html_instructions>Turn &lt;b&gt;right&lt;/b&gt; onto &lt;b&gt;A. Mapu gatvė&lt;/b&gt;&lt;div style=&quot;font-size:0.9em&quot;&gt;Destination will be on the right&lt;/div&gt;</html_instructions>
                    //    <distance>
                    //     <value>192</value>
                    //     <text>0.2 km</text>
                    //    </distance>
                    //   </step>
                    //   <duration>
                    //    <value>5073</value>
                    //    <text>1 hour 25 mins</text>
                    //   </duration>
                    //   <distance>
                    //    <value>105199</value>
                    //    <text>105 km</text>
                    //   </distance>
                    //   <start_location>
                    //    <lat>54.6893800</lat>
                    //    <lng>25.2800500</lng>
                    //   </start_location>
                    //   <end_location>
                    //    <lat>54.8968500</lat>
                    //    <lng>23.8930000</lng>
                    //   </end_location>
                    //   <start_address>Vilnius, Lithuania</start_address>
                    //   <end_address>Kaunas, Lithuania</end_address>
                    //  </leg>
                    //  <copyrights>Map data ©2011 Tele Atlas</copyrights>
                    //  <overview_polyline>
                    //   <points>soxlIiohyCYL}Fb@mApUF`@GvAYpD{@dGcCjIoIvOwBpFy@xC]jBSxCC~E^~Er@lCtAdC`AwD~@oB`AW`U|FdF|@`J^~E[tAj@hB~BjA~ELrCJzOzA~Od@`N^lDdAtDt@xAjAnApDlCbXbKpAl@bBpAdBpBxA|BdLpV`BxCvDpEbChBpBhAdAXjBHbG_@|@LtHbClFr@jK`F~VjNxBbB`@h@rwAt|Cba@l`BjAxGNxEMvDaAzF}bBjiFcFbQ_y@|gD{CxMeBnJcK~n@wh@dkCkAlIoQt}AeEfV}EzQqClImBxE{DrIkQ|ZcEvIkDzIcDhKyBxJ{EdXuCtS_G`g@mF|\eF`WyDhOiE~NiErMaGpOoj@ppAoE|K_EzKeDtKmXzbAgI~U{FrNsEvJoLfT{J`O_EjFooBf_C{GdJkLtSwI`SyClI}CrJcDtL{CzMeDlQcXzlBiArIwApNaBfWaLfiCoBpYsDf\qChR_FlVqEpQ_ZbfAyFfXwCtRiAfKeFfs@gBfRcBxMaLdp@sXrzAaE~UqCzRyC`[_q@z|LuDtu@qp@b}WuLraFo@jPo^r}Faq@pfHaBtMsm@~`EoKvo@kv@lkEcuBjzKkNj|@cu@`~EgCnNuiBpcJakAx|GyB`KqdC~fKoIfYidAbwCoD|MeAbHcA|Im@xK}YnhKyV~gEs@~f@aEn|Ee@`XQ^MnD[lBoF`N}@zJCjBfKxCJLdj@bQv`@pIpHnAdl@hLdB`@nDvAtEjDdCvCbOvSzLhZbQd^`JpMPt@QtBFnAz@hDl@^j@?f@e@pAe@x@k@xBPfCEf@Uj@wBbEx@pCH</points>
                    //  </overview_polyline>
                    //  <bounds>
                    //   <southwest>
                    //    <lat>54.6389500</lat>
                    //    <lng>23.8920900</lng>
                    //   </southwest>
                    //   <northeast>
                    //    <lat>54.9376500</lat>
                    //    <lng>25.2800500</lng>
                    //   </northeast>
                    //  </bounds>
                    // </route>
                    //</DirectionsResponse> 
                    #endregion

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(kml);

                    XmlNode nn = doc.SelectSingleNode("/DirectionsResponse/status");
                    if (nn != null)
                    {
                        ret = (DirectionsStatusCode)Enum.Parse(typeof(DirectionsStatusCode), nn.InnerText, false);
                        if (ret == DirectionsStatusCode.OK)
                        {
                            if (cache && GMaps.Instance.UseDirectionsCache)
                            {
                                Cache.Instance.SaveContent(url, CacheType.DirectionsCache, kml);
                            }

                            direction = new GDirections();

                            nn = doc.SelectSingleNode("/DirectionsResponse/route/summary");
                            if (nn != null)
                            {
                                direction.Summary = nn.InnerText;
                                Debug.WriteLine("summary: " + direction.Summary);
                            }

                            nn = doc.SelectSingleNode("/DirectionsResponse/route/leg/duration");
                            if (nn != null)
                            {
                                var t = nn.SelectSingleNode("text");
                                if (t != null)
                                {
                                    direction.Duration = t.InnerText;
                                }

                                t = nn.SelectSingleNode("value");
                                if (t != null)
                                {
                                    if (!string.IsNullOrEmpty(t.InnerText))
                                    {
                                        direction.DurationValue = uint.Parse(t.InnerText);
                                    }
                                }
                            }

                            nn = doc.SelectSingleNode("/DirectionsResponse/route/leg/distance");
                            if (nn != null)
                            {
                                var t = nn.SelectSingleNode("text");
                                if (t != null)
                                {
                                    direction.Distance = t.InnerText;
                                }

                                t = nn.SelectSingleNode("value");
                                if (t != null)
                                {
                                    if (!string.IsNullOrEmpty(t.InnerText))
                                    {
                                        direction.DistanceValue = uint.Parse(t.InnerText);
                                    }
                                }
                            }

                            nn = doc.SelectSingleNode("/DirectionsResponse/route/leg/start_location");
                            if (nn != null)
                            {
                                var pt = nn.SelectSingleNode("lat");
                                if (pt != null)
                                {
                                    direction.StartLocation.Lat = double.Parse(pt.InnerText, CultureInfo.InvariantCulture);
                                }

                                pt = nn.SelectSingleNode("lng");
                                if (pt != null)
                                {
                                    direction.StartLocation.Lng = double.Parse(pt.InnerText, CultureInfo.InvariantCulture);
                                }
                            }

                            nn = doc.SelectSingleNode("/DirectionsResponse/route/leg/end_location");
                            if (nn != null)
                            {
                                var pt = nn.SelectSingleNode("lat");
                                if (pt != null)
                                {
                                    direction.EndLocation.Lat = double.Parse(pt.InnerText, CultureInfo.InvariantCulture);
                                }

                                pt = nn.SelectSingleNode("lng");
                                if (pt != null)
                                {
                                    direction.EndLocation.Lng = double.Parse(pt.InnerText, CultureInfo.InvariantCulture);
                                }
                            }

                            nn = doc.SelectSingleNode("/DirectionsResponse/route/leg/start_address");
                            if (nn != null)
                            {
                                direction.StartAddress = nn.InnerText;
                            }

                            nn = doc.SelectSingleNode("/DirectionsResponse/route/leg/end_address");
                            if (nn != null)
                            {
                                direction.EndAddress = nn.InnerText;
                            }

                            nn = doc.SelectSingleNode("/DirectionsResponse/route/copyrights");
                            if (nn != null)
                            {
                                direction.Copyrights = nn.InnerText;
                            }

                            nn = doc.SelectSingleNode("/DirectionsResponse/route/overview_polyline/points");
                            if (nn != null)
                            {
                                direction.Route = new List<PointLatLng>();
                                DecodePointsInto(direction.Route, nn.InnerText);
                            }

                            XmlNodeList steps = doc.SelectNodes("/DirectionsResponse/route/leg/step");
                            if (steps != null)
                            {
                                if (steps.Count > 0)
                                {
                                    direction.Steps = new List<GDirectionStep>();
                                }

                                foreach (XmlNode s in steps)
                                {
                                    GDirectionStep step = new GDirectionStep();

                                    nn = s.SelectSingleNode("travel_mode");
                                    if (nn != null)
                                    {
                                        step.TravelMode = nn.InnerText;
                                    }

                                    nn = s.SelectSingleNode("start_location");
                                    if (nn != null)
                                    {
                                        var pt = nn.SelectSingleNode("lat");
                                        if (pt != null)
                                        {
                                            step.StartLocation.Lat = double.Parse(pt.InnerText, CultureInfo.InvariantCulture);
                                        }

                                        pt = nn.SelectSingleNode("lng");
                                        if (pt != null)
                                        {
                                            step.StartLocation.Lng = double.Parse(pt.InnerText, CultureInfo.InvariantCulture);
                                        }
                                    }

                                    nn = s.SelectSingleNode("end_location");
                                    if (nn != null)
                                    {
                                        var pt = nn.SelectSingleNode("lat");
                                        if (pt != null)
                                        {
                                            step.EndLocation.Lat = double.Parse(pt.InnerText, CultureInfo.InvariantCulture);
                                        }

                                        pt = nn.SelectSingleNode("lng");
                                        if (pt != null)
                                        {
                                            step.EndLocation.Lng = double.Parse(pt.InnerText, CultureInfo.InvariantCulture);
                                        }
                                    }

                                    nn = s.SelectSingleNode("duration");
                                    if (nn != null)
                                    {
                                        nn = nn.SelectSingleNode("text");
                                        if (nn != null)
                                        {
                                            step.Duration = nn.InnerText;
                                        }
                                    }

                                    nn = s.SelectSingleNode("distance");
                                    if (nn != null)
                                    {
                                        nn = nn.SelectSingleNode("text");
                                        if (nn != null)
                                        {
                                            step.Distance = nn.InnerText;
                                        }
                                    }

                                    nn = s.SelectSingleNode("html_instructions");
                                    if (nn != null)
                                    {
                                        step.HtmlInstructions = nn.InnerText;
                                    }

                                    nn = s.SelectSingleNode("polyline");
                                    if (nn != null)
                                    {
                                        nn = nn.SelectSingleNode("points");
                                        if (nn != null)
                                        {
                                            step.Points = new List<PointLatLng>();
                                            DecodePointsInto(step.Points, nn.InnerText);
                                        }
                                    }

                                    direction.Steps.Add(step);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                direction = null;
                ret = DirectionsStatusCode.ExceptionInCode;
                Debug.WriteLine("GetDirectionsUrl: " + ex);
            }
            return ret;
        }

        static void DecodePointsInto(List<PointLatLng> path, string encodedPath)
        {
            // https://github.com/googlemaps/google-maps-services-java/blob/master/src/main/java/com/google/maps/internal/PolylineEncoding.java
            int len = encodedPath.Length;
            int index = 0;
            int lat = 0;
            int lng = 0;
            while (index < len)
            {
                int result = 1;
                int shift = 0;
                int b;
                do
                {
                    b = encodedPath[index++] - 63 - 1;
                    result += b << shift;
                    shift += 5;
                } while (b >= 0x1f && index < len);
                lat += (result & 1) != 0 ? ~(result >> 1) : (result >> 1);

                result = 1;
                shift = 0;

                if (index < len)
                {
                    do
                    {
                        b = encodedPath[index++] - 63 - 1;
                        result += b << shift;
                        shift += 5;
                    } while (b >= 0x1f && index < len);
                    lng += (result & 1) != 0 ? ~(result >> 1) : (result >> 1);
                }

                path.Add(new PointLatLng(lat * 1e-5, lng * 1e-5));
            }
        }

        static readonly string DirectionUrlFormatStr = "http://maps.{7}/maps/api/directions/xml?origin={0}&destination={1}&sensor={2}&language={3}{4}{5}{6}";
        static readonly string DirectionUrlFormatPoint = "http://maps.{9}/maps/api/directions/xml?origin={0},{1}&destination={2},{3}&sensor={4}&language={5}{6}{7}{8}";
        static readonly string DirectionUrlFormatWaypoint = "http://maps.{8}/maps/api/directions/xml?origin={0},{1}&waypoints={2}&destination={9},{10}&sensor={3}&language={4}{5}{6}{7}";
        static readonly string DirectionUrlFormatWaypointStr = "http://maps.{7}/maps/api/directions/xml?origin={0}&waypoints={1}&destination={8}&sensor={2}&language={3}{4}{5}{6}";

        #endregion

        #endregion

        #region -- Maps API for Work --
        /// <summary>
        /// https://developers.google.com/maps/documentation/business/webservices/auth#how_do_i_get_my_signing_key
        /// To access the special features of the Google Maps API for Work you must provide a client ID
        /// when accessing any of the API libraries or services.
        /// When registering for Google Google Maps API for Work you will receive this client ID from Enterprise Support.
        /// All client IDs begin with a gme- prefix. Your client ID is passed as the value of the client parameter.
        /// Generally, you should store your private key someplace safe and read them into your code
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="privateKey"></param>
        public void SetEnterpriseCredentials(string clientId, string privateKey)
        {
            privateKey = privateKey.Replace("-", "+").Replace("_", "/");
            _privateKeyBytes = Convert.FromBase64String(privateKey);
            _clientId = clientId;
        }
        private byte[] _privateKeyBytes;

        private string _clientId = string.Empty;

        /// <summary>
        /// Your client ID. To access the special features of the Google Maps API for Work
        /// you must provide a client ID when accessing any of the API libraries or services.
        /// When registering for Google Google Maps API for Work you will receive this client ID
        /// from Enterprise Support. All client IDs begin with a gme- prefix.
        /// </summary>
        public string ClientId
        {
            get
            {
                return _clientId;
            }
        }

        string GetSignedUri(Uri uri)
        {
            var builder = new UriBuilder(uri);
            builder.Query = builder.Query.Substring(1) + "&client=" + _clientId;
            uri = builder.Uri;
            string signature = GetSignature(uri);

            return uri.Scheme + "://" + uri.Host + uri.LocalPath + uri.Query + "&signature=" + signature;
        }

        string GetSignedUri(string url)
        {
            return GetSignedUri(new Uri(url));
        }

        string GetSignature(Uri uri)
        {
            byte[] encodedPathQuery = Encoding.ASCII.GetBytes(uri.LocalPath + uri.Query);
            var hashAlgorithm = new HMACSHA1(_privateKeyBytes);
            byte[] hashed = hashAlgorithm.ComputeHash(encodedPathQuery);
            return Convert.ToBase64String(hashed).Replace("+", "-").Replace("/", "_");
        } 
        #endregion
    }

    /// <summary>
    /// GoogleMap provider
    /// </summary>
    public class GoogleMapProvider : GoogleMapProviderBase
    {
        public static readonly GoogleMapProvider Instance;

        GoogleMapProvider()
        {
        }

        static GoogleMapProvider()
        {
            Instance = new GoogleMapProvider();
        }

        public string Version = "m@333000000";

        #region GMapProvider Members

        readonly Guid id = new Guid("D7287DA0-A7FF-405F-8166-B6BAF26D066C");
        public override Guid Id
        {
            get
            {
                return id;
            }
        }

        readonly string name = "GoogleMap";
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
            string sec1 = string.Empty; // after &x=...
            string sec2 = string.Empty; // after &zoom=...
            GetSecureWords(pos, out sec1, out sec2);

            return string.Format(UrlFormat, UrlFormatServer, GetServerNum(pos, 4), UrlFormatRequest, Version, language, pos.X, sec1, pos.Y, zoom, sec2, Server);
        }

        static readonly string UrlFormatServer = "mt";
        static readonly string UrlFormatRequest = "vt";
        static readonly string UrlFormat = "http://{0}{1}.{10}/maps/{2}/lyrs={3}&hl={4}&x={5}{6}&y={7}&z={8}&s={9}";
    }
}