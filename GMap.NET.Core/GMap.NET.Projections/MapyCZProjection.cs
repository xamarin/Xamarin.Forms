namespace GMap.NET.Projections
{
    using System;

    /// <summary>
    /// GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.01745329251994328,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]]
    /// PROJCS["Mapy.cz",GEOGCS["GCS_WGS_1984",DATUM["D_WGS_1984",SPHEROID["WGS_1984",6378137,298.257223563]],PRIMEM["Greenwich",0],UNIT["Degree",0.017453292519943295]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",15],PARAMETER["scale_factor",0.9996],PARAMETER["false_easting",134400000],PARAMETER["false_northing",-41600000],UNIT["1/32meter",0.03125]]
    /// </summary>
    public class MapyCZProjection : PureProjection
    {
        public static readonly MapyCZProjection Instance = new MapyCZProjection();

        static readonly double MinLatitude = 26;
        static readonly double MaxLatitude = 76;
        static readonly double MinLongitude = -26;
        static readonly double MaxLongitude = 38;

        #region -- Common --
        static int getLCM(int zone)
        {
            if ((zone < 1) || (zone > 60))
            {
                throw new Exception("MapyCZProjection: UTM Zone number is not between 1 and 60.");
            }
            else
            {
                return ((zone * 6) - 183);
            }
        }

        static double roundoff(double xx, double yy)
        {
            var x = xx;
            var y = yy;
            x = Math.Round(x * Math.Pow(10, y)) / Math.Pow(10, y);
            return x;
        }

        static readonly double UTMSIZE = 2;
        static readonly double UNITS = 1;

        #endregion

        #region -- WGSToMapyCZ --

        public long[] WGSToPP(double la, double lo)
        {
            var utmEE = wgsToUTM(DegreesToRadians(la), DegreesToRadians(lo), 33);
            var pp = utmEEToPP(utmEE[0], utmEE[1]);
            return pp;
        }

        static long[] utmEEToPP(double east, double north)
        {
            var x = (Math.Round(east) - (-3700000.0)) * Math.Pow(2, 5);
            var y = (Math.Round(north) - (1300000.0)) * Math.Pow(2, 5);

            return new long[] { (long)x, (long)y };
        }

        double[] wgsToUTM(double la, double lo, int zone)
        {
            var latrad = la;
            var lonrad = lo;
            var latddd = RadiansToDegrees(la);
            var londdd = RadiansToDegrees(lo);

            var k = 0.9996f;
            var a = Axis;
            var f = Flattening;
            var b = a * (1.0 - f);
            var e2 = (a * a - b * b) / (a * a);
            var e = Math.Sqrt(e2);
            var ei2 = (a * a - b * b) / (b * b);
            var ei = Math.Sqrt(ei2);
            var n = (a - b) / (a + b);
            var G = a * (1.0 - n) * (1.0 - n * n) * (1.0 + (9 / 4.0) * n * n + (255.0 / 64.0) * Math.Pow(n, 4)) * (PI / 180.0);
            var w = londdd - ((double)(zone * 6 - 183));
            w = DegreesToRadians(w);
            var t = Math.Tan(latrad);
            var rho = a * (1.0 - e2) / Math.Pow(1.0 - (e2 * Math.Sin(latrad) * Math.Sin(latrad)), (3 / 2.0));
            var nu = a / Math.Sqrt(1.0 - (e2 * Math.Sin(latrad) * Math.Sin(latrad)));
            var psi = nu / rho;
            var coslat = Math.Cos(latrad);
            var sinlat = Math.Sin(latrad);
            var A0 = 1 - (e2 / 4.0) - (3 * e2 * e2 / 64.0) - (5 * Math.Pow(e2, 3) / 256.0);
            var A2 = (3 / 8.0) * (e2 + (e2 * e2 / 4.0) + (15 * Math.Pow(e2, 3) / 128.0));
            var A4 = (15 / 256.0) * (e2 * e2 + (3 * Math.Pow(e2, 3) / 4.0));
            var A6 = 35 * Math.Pow(e2, 3) / 3072.0;
            var m = a * ((A0 * latrad) - (A2 * Math.Sin(2 * latrad)) + (A4 * Math.Sin(4 * latrad)) - (A6 * Math.Sin(6 * latrad)));
            var eterm1 = (w * w / 6.0) * coslat * coslat * (psi - t * t);
            var eterm2 = (Math.Pow(w, 4) / 120.0) * Math.Pow(coslat, 4) * (4 * Math.Pow(psi, 3) * (1.0 - 6 * t * t) + psi * psi * (1.0 + 8 * t * t) - psi * 2 * t * t + Math.Pow(t, 4));
            var eterm3 = (Math.Pow(w, 6) / 5040.0) * Math.Pow(coslat, 6) * (61.0 - 479 * t * t + 179 * Math.Pow(t, 4) - Math.Pow(t, 6));
            var dE = k * nu * w * coslat * (1.0 + eterm1 + eterm2 + eterm3);
            var east = 500000.0 + (dE / UNITS);
            east = roundoff(east, UTMSIZE);
            var nterm1 = (w * w / 2.0) * nu * sinlat * coslat;
            var nterm2 = (Math.Pow(w, 4) / 24.0) * nu * sinlat * Math.Pow(coslat, 3) * (4 * psi * psi + psi - t * t);
            var nterm3 = (Math.Pow(w, 6) / 720.0) * nu * sinlat * Math.Pow(coslat, 5) * (8 * Math.Pow(psi, 4) * (11.0 - 24 * t * t) - 28 * Math.Pow(psi, 3) * (1.0 - 6 * t * t) + psi * psi * (1.0 - 32 * t * t) - psi * 2 * t * t + Math.Pow(t, 4));
            var nterm4 = (Math.Pow(w, 8) / 40320.0) * nu * sinlat * Math.Pow(coslat, 7) * (1385.0 - 3111 * t * t + 543 * Math.Pow(t, 4) - Math.Pow(t, 6));
            var dN = k * (m + nterm1 + nterm2 + nterm3 + nterm4);
            var north = (0.0 + (dN / UNITS));
            north = roundoff(north, UTMSIZE);

            return new double[] { east, north, zone };
        }

        #endregion

        #region -- MapyCZToWGS --

        public double[] PPToWGS(double x, double y)
        {
            var utmEE = ppToUTMEE(x, y);
            var ret = utmToWGS(utmEE[0], utmEE[1], 33);
            return ret;
        }

        double[] ppToUTMEE(double x, double y)
        {
            var north = y * Math.Pow(2, -5) + 1300000.0;
            var east = x * Math.Pow(2, -5) + (-3700000.0);
            east = roundoff(east, UTMSIZE);
            north = roundoff(north, UTMSIZE);

            return new double[] { east, north };
        }

        double[] utmToWGS(double eastIn, double northIn, int zone)
        {
            var k = 0.9996f;
            var a = Axis;
            var f = Flattening;
            var b = a * (1.0 - f);
            var e2 = (a * a - b * b) / (a * a);
            var e = Math.Sqrt(e2);
            var ei2 = (a * a - b * b) / (b * b);
            var ei = Math.Sqrt(ei2);
            var n = (a - b) / (a + b);
            var G = a * (1.0 - n) * (1.0 - n * n) * (1.0 + (9 / 4.0) * n * n + (255 / 64.0) * Math.Pow(n, 4)) * (PI / 180.0);
            var north = (northIn - 0) * UNITS;
            var east = (eastIn - 500000.0) * UNITS;
            var m = north / k;
            var sigma = (m * PI) / (180.0 * G);
            var footlat = sigma + ((3 * n / 2.0) - (27 * Math.Pow(n, 3) / 32.0)) * Math.Sin(2 * sigma) + ((21 * n * n / 16.0) - (55 * Math.Pow(n, 4) / 32.0)) * Math.Sin(4 * sigma) + (151 * Math.Pow(n, 3) / 96.0) * Math.Sin(6 * sigma) + (1097 * Math.Pow(n, 4) / 512.0) * Math.Sin(8 * sigma);
            var rho = a * (1.0 - e2) / Math.Pow(1.0 - (e2 * Math.Sin(footlat) * Math.Sin(footlat)), (3 / 2.0));
            var nu = a / Math.Sqrt(1.0 - (e2 * Math.Sin(footlat) * Math.Sin(footlat)));
            var psi = nu / rho;
            var t = Math.Tan(footlat);
            var x = east / (k * nu);
            var laterm1 = (t / (k * rho)) * (east * x / 2.0);
            var laterm2 = (t / (k * rho)) * (east * Math.Pow(x, 3) / 24.0) * (-4 * psi * psi + 9 * psi * (1 - t * t) + 12 * t * t);
            var laterm3 = (t / (k * rho)) * (east * Math.Pow(x, 5) / 720.0) * (8 * Math.Pow(psi, 4) * (11 - 24 * t * t) - 12 * Math.Pow(psi, 3) * (21.0 - 71 * t * t) + 15 * psi * psi * (15.0 - 98 * t * t + 15 * Math.Pow(t, 4)) + 180 * psi * (5 * t * t - 3 * Math.Pow(t, 4)) + 360 * Math.Pow(t, 4));
            var laterm4 = (t / (k * rho)) * (east * Math.Pow(x, 7) / 40320.0) * (1385.0 + 3633 * t * t + 4095 * Math.Pow(t, 4) + 1575 * Math.Pow(t, 6));
            var latrad = footlat - laterm1 + laterm2 - laterm3 + laterm4;
            var lat = RadiansToDegrees(latrad);
            var seclat = 1 / Math.Cos(footlat);
            var loterm1 = x * seclat;
            var loterm2 = (Math.Pow(x, 3) / 6.0) * seclat * (psi + 2 * t * t);
            var loterm3 = (Math.Pow(x, 5) / 120.0) * seclat * (-4 * Math.Pow(psi, 3) * (1 - 6 * t * t) + psi * psi * (9 - 68 * t * t) + 72 * psi * t * t + 24 * Math.Pow(t, 4));
            var loterm4 = (Math.Pow(x, 7) / 5040.0) * seclat * (61.0 + 662 * t * t + 1320 * Math.Pow(t, 4) + 720 * Math.Pow(t, 6));
            var w = loterm1 - loterm2 + loterm3 - loterm4;
            var longrad = DegreesToRadians(getLCM(zone)) + w;
            var lon = RadiansToDegrees(longrad);

            return new double[] { lat, lon, latrad, longrad };
        }

        #endregion

        public override RectLatLng Bounds
        {
            get
            {
                return RectLatLng.FromLTRB(MinLongitude, MaxLatitude, MaxLongitude, MinLatitude);
            }
        }

        public override GSize TileSize
        {
            get
            {
                return new GSize(256, 256);
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

            var size = GetTileMatrixSizePixel(zoom);
            {
                var l = WGSToPP(lat, lng);
                ret.X = (long)l[0] >> (20 - zoom);
                ret.Y = size.Height - ((long)l[1] >> (20 - zoom));
            }
            return ret;
        }

        public override PointLatLng FromPixelToLatLng(long x, long y, int zoom)
        {
            PointLatLng ret = PointLatLng.Empty;

            var size = GetTileMatrixSizePixel(zoom);

            var oX = x << (20 - zoom);
            var oY = (size.Height - y) << (20 - zoom);
            {
                var l = PPToWGS(oX, oY);
                ret.Lat = Clip(l[0], MinLatitude, MaxLatitude);
                ret.Lng = Clip(l[1], MinLongitude, MaxLongitude);
            }
            return ret;
        }

        public override GSize GetTileMatrixSizeXY(int zoom)
        {
            return new GSize((long)Math.Pow(2, zoom), (long)Math.Pow(2, zoom));
        }

        public override GSize GetTileMatrixSizePixel(int zoom)
        {
            GSize s = GetTileMatrixSizeXY(zoom);
            return new GSize(s.Width << 8, s.Height << 8);
        }

        public override GSize GetTileMatrixMinXY(int zoom)
        {
            long wh = zoom > 3 ? (3 * (long)Math.Pow(2, zoom - 4)) : 1;
            return new GSize(wh, wh);
        }

        public override GSize GetTileMatrixMaxXY(int zoom)
        {
            long wh = (long)Math.Pow(2, zoom) - (long)Math.Pow(2, zoom - 2);
            return new GSize(wh, wh);
        }
    }
}
