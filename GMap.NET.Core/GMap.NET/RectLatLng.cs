
namespace GMap.NET
{
   using System;
   using System.Globalization;

   /// <summary>
   /// the rect of coordinates
   /// </summary>
   public struct RectLatLng
   {
      public static readonly RectLatLng Empty;
      private double lng;
      private double lat;
      private double widthLng;
      private double heightLat;

      public RectLatLng(double lat, double lng, double widthLng, double heightLat)
      {
         this.lng = lng;
         this.lat = lat;
         this.widthLng = widthLng;
         this.heightLat = heightLat;
         NotEmpty = true;
      }

      public RectLatLng(PointLatLng location, SizeLatLng size)
      {
         this.lng = location.Lng;
         this.lat = location.Lat;
         this.widthLng = size.WidthLng;
         this.heightLat = size.HeightLat;
         NotEmpty = true;
      }

      public static RectLatLng FromLTRB(double leftLng, double topLat, double rightLng, double bottomLat)
      {
         return new RectLatLng(topLat, leftLng, rightLng - leftLng, topLat - bottomLat);
      }

      public PointLatLng LocationTopLeft
      {
         get
         {
            return new PointLatLng(this.Lat, this.Lng);
         }
         set
         {
            this.Lng = value.Lng;
            this.Lat = value.Lat;
         }
      }

      public PointLatLng LocationRightBottom
      {
         get
         {
            PointLatLng ret = new PointLatLng(this.Lat, this.Lng);
            ret.Offset(HeightLat, WidthLng);
            return ret;
         }
      }

      public PointLatLng LocationMiddle
      {
         get
         {
            PointLatLng ret = new PointLatLng(this.Lat, this.Lng);
            ret.Offset(HeightLat / 2, WidthLng / 2);
            return ret;
         }
      }

      public SizeLatLng Size
      {
         get
         {
            return new SizeLatLng(this.HeightLat, this.WidthLng);
         }
         set
         {
            this.WidthLng = value.WidthLng;
            this.HeightLat = value.HeightLat;
         }
      }

      public double Lng
      {
         get
         {
            return this.lng;
         }
         set
         {
            this.lng = value;
         }
      }

      public double Lat
      {
         get
         {
            return this.lat;
         }
         set
         {
            this.lat = value;
         }
      }

      public double WidthLng
      {
         get
         {
            return this.widthLng;
         }
         set
         {
            this.widthLng = value;
         }
      }

      public double HeightLat
      {
         get
         {
            return this.heightLat;
         }
         set
         {
            this.heightLat = value;
         }
      }

      public double Left
      {
         get
         {
            return this.Lng;
         }
      }

      public double Top
      {
         get
         {
            return this.Lat;
         }
      }

      public double Right
      {
         get
         {
            return (this.Lng + this.WidthLng);
         }
      }

      public double Bottom
      {
         get
         {
            return (this.Lat - this.HeightLat);
         }
      }

      bool NotEmpty;

      /// <summary>
      /// returns true if coordinates wasn't assigned
      /// </summary>
      public bool IsEmpty
      {
          get
          {
              return !NotEmpty;
          }
      }

      public override bool Equals(object obj)
      {
         if(!(obj is RectLatLng))
         {
            return false;
         }
         RectLatLng ef = (RectLatLng)obj;
         return ((((ef.Lng == this.Lng) && (ef.Lat == this.Lat)) && (ef.WidthLng == this.WidthLng)) && (ef.HeightLat == this.HeightLat));
      }

      public static bool operator ==(RectLatLng left, RectLatLng right)
      {
         return ((((left.Lng == right.Lng) && (left.Lat == right.Lat)) && (left.WidthLng == right.WidthLng)) && (left.HeightLat == right.HeightLat));
      }

      public static bool operator !=(RectLatLng left, RectLatLng right)
      {
         return !(left == right);
      }

      public bool Contains(double lat, double lng)
      {
         return ((((this.Lng <= lng) && (lng < (this.Lng + this.WidthLng))) && (this.Lat >= lat)) && (lat > (this.Lat - this.HeightLat)));
      }

      public bool Contains(PointLatLng pt)
      {
         return this.Contains(pt.Lat, pt.Lng);
      }

      public bool Contains(RectLatLng rect)
      {
         return ((((this.Lng <= rect.Lng) && ((rect.Lng + rect.WidthLng) <= (this.Lng + this.WidthLng))) && (this.Lat >= rect.Lat)) && ((rect.Lat - rect.HeightLat) >= (this.Lat - this.HeightLat)));
      }

      public override int GetHashCode()
      {
         if(this.IsEmpty)
         {
            return 0;
         }
         return (((this.Lng.GetHashCode() ^ this.Lat.GetHashCode()) ^ this.WidthLng.GetHashCode()) ^ this.HeightLat.GetHashCode());
      }

      // from here down need to test each function to be sure they work good
      // |
      // .

      #region -- unsure --
      public void Inflate(double lat, double lng)
      {
         this.Lng -= lng;
         this.Lat += lat;
         this.WidthLng += 2d * lng;
         this.HeightLat += 2d * lat;
      }

      public void Inflate(SizeLatLng size)
      {
         this.Inflate(size.HeightLat, size.WidthLng);
      }

      public static RectLatLng Inflate(RectLatLng rect, double lat, double lng)
      {
         RectLatLng ef = rect;
         ef.Inflate(lat, lng);
         return ef;
      }

      public void Intersect(RectLatLng rect)
      {
         RectLatLng ef = Intersect(rect, this);
         this.Lng = ef.Lng;
         this.Lat = ef.Lat;
         this.WidthLng = ef.WidthLng;
         this.HeightLat = ef.HeightLat;
      }

      // ok ???
      public static RectLatLng Intersect(RectLatLng a, RectLatLng b)
      {
         double lng = Math.Max(a.Lng, b.Lng);
         double num2 = Math.Min((double)(a.Lng + a.WidthLng), (double)(b.Lng + b.WidthLng));

         double lat = Math.Max(a.Lat, b.Lat);
         double num4 = Math.Min((double)(a.Lat + a.HeightLat), (double)(b.Lat + b.HeightLat));

         if((num2 >= lng) && (num4 >= lat))
         {
            return new RectLatLng(lat, lng, num2 - lng, num4 - lat);
         }
         return Empty;
      }

      // ok ???
      // http://greatmaps.codeplex.com/workitem/15981
      public bool IntersectsWith(RectLatLng a)
      {
         return this.Left < a.Right && this.Top > a.Bottom && this.Right > a.Left && this.Bottom < a.Top;
      }

      // ok ???
      // http://greatmaps.codeplex.com/workitem/15981
      public static RectLatLng Union(RectLatLng a, RectLatLng b)
      {
         return RectLatLng.FromLTRB(
            Math.Min(a.Left, b.Left),
            Math.Max(a.Top, b.Top),
            Math.Max(a.Right, b.Right),
            Math.Min(a.Bottom, b.Bottom));
      }
      #endregion

      // .
      // |
      // unsure ends here

      public void Offset(PointLatLng pos)
      {
         this.Offset(pos.Lat, pos.Lng);
      }

      public void Offset(double lat, double lng)
      {
         this.Lng += lng;
         this.Lat -= lat;
      }

      public override string ToString()
      {
         return ("{Lat=" + this.Lat.ToString(CultureInfo.CurrentCulture) + ",Lng=" + this.Lng.ToString(CultureInfo.CurrentCulture) + ",WidthLng=" + this.WidthLng.ToString(CultureInfo.CurrentCulture) + ",HeightLat=" + this.HeightLat.ToString(CultureInfo.CurrentCulture) + "}");
      }

      static RectLatLng()
      {
         Empty = new RectLatLng();
      }
   }
}