
namespace GMap.NET
{
   using System;
   using System.Globalization;

   /// <summary>
   /// the point of coordinates
   /// </summary>
   [Serializable]
   public struct PointLatLng
   {
      public static readonly PointLatLng Empty = new PointLatLng();
      private double lat;
      private double lng;

      bool NotEmpty;

      public PointLatLng(double lat, double lng)
      {
         this.lat = lat;
         this.lng = lng;
         NotEmpty = true;
      }

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

      public double Lat
      {
         get
         {
            return this.lat;
         }
         set
         {
            this.lat = value;
            NotEmpty = true;
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
            NotEmpty = true;
         }
      }

      public static PointLatLng operator +(PointLatLng pt, SizeLatLng sz)
      {
         return Add(pt, sz);
      }

      public static PointLatLng operator -(PointLatLng pt, SizeLatLng sz)
      {
         return Subtract(pt, sz);
      }

      public static SizeLatLng operator -(PointLatLng pt1, PointLatLng pt2)
      {
          return new SizeLatLng(pt1.Lat - pt2.Lat, pt2.Lng - pt1.Lng);
      }

      public static bool operator ==(PointLatLng left, PointLatLng right)
      {
         return ((left.Lng == right.Lng) && (left.Lat == right.Lat));
      }

      public static bool operator !=(PointLatLng left, PointLatLng right)
      {
         return !(left == right);
      }

      public static PointLatLng Add(PointLatLng pt, SizeLatLng sz)
      {
         return new PointLatLng(pt.Lat - sz.HeightLat, pt.Lng + sz.WidthLng);
      }

      public static PointLatLng Subtract(PointLatLng pt, SizeLatLng sz)
      {
         return new PointLatLng(pt.Lat + sz.HeightLat, pt.Lng - sz.WidthLng);
      }

      public override bool Equals(object obj)
      {
         if(!(obj is PointLatLng))
         {
            return false;
         }
         PointLatLng tf = (PointLatLng)obj;
         return (((tf.Lng == this.Lng) && (tf.Lat == this.Lat)) && tf.GetType().Equals(base.GetType()));
      }

      public void Offset(PointLatLng pos)
      {
         this.Offset(pos.Lat, pos.Lng);
      }

      public void Offset(double lat, double lng)
      {
         this.Lng += lng;
         this.Lat -= lat;
      }

      public override int GetHashCode()
      {
         return (this.Lng.GetHashCode() ^ this.Lat.GetHashCode());
      }

      public override string ToString()
      {
         return string.Format(CultureInfo.CurrentCulture, "{{Lat={0}, Lng={1}}}", this.Lat, this.Lng);
      }
   }
}