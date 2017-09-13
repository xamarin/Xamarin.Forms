
namespace GMap.NET
{
   using System.Globalization;

   /// <summary>
   /// the size of coordinates
   /// </summary>
   public struct SizeLatLng
   {
      public static readonly SizeLatLng Empty;

      private double heightLat;
      private double widthLng;

      public SizeLatLng(SizeLatLng size)
      {
         this.widthLng = size.widthLng;
         this.heightLat = size.heightLat;
      }

      public SizeLatLng(PointLatLng pt)
      {
         this.heightLat = pt.Lat;
         this.widthLng = pt.Lng;
      }

      public SizeLatLng(double heightLat, double widthLng)
      {
         this.heightLat = heightLat;
         this.widthLng = widthLng;
      }

      public static SizeLatLng operator+(SizeLatLng sz1, SizeLatLng sz2)
      {
         return Add(sz1, sz2);
      }

      public static SizeLatLng operator-(SizeLatLng sz1, SizeLatLng sz2)
      {
         return Subtract(sz1, sz2);
      }

      public static bool operator==(SizeLatLng sz1, SizeLatLng sz2)
      {
         return ((sz1.WidthLng == sz2.WidthLng) && (sz1.HeightLat == sz2.HeightLat));
      }

      public static bool operator!=(SizeLatLng sz1, SizeLatLng sz2)
      {
         return !(sz1 == sz2);
      }

      public static explicit operator PointLatLng(SizeLatLng size)
      {
         return new PointLatLng(size.HeightLat, size.WidthLng);
      }

      public bool IsEmpty
      {
         get
         {
            return ((this.widthLng == 0d) && (this.heightLat == 0d));
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

      public static SizeLatLng Add(SizeLatLng sz1, SizeLatLng sz2)
      {
         return new SizeLatLng(sz1.HeightLat + sz2.HeightLat, sz1.WidthLng + sz2.WidthLng);
      }

      public static SizeLatLng Subtract(SizeLatLng sz1, SizeLatLng sz2)
      {
         return new SizeLatLng(sz1.HeightLat - sz2.HeightLat, sz1.WidthLng - sz2.WidthLng);
      }

      public override bool Equals(object obj)
      {
         if(!(obj is SizeLatLng))
         {
            return false;
         }
         SizeLatLng ef = (SizeLatLng) obj;
         return (((ef.WidthLng == this.WidthLng) && (ef.HeightLat == this.HeightLat)) && ef.GetType().Equals(base.GetType()));
      }

      public override int GetHashCode()
      {
         if(this.IsEmpty)
         {
            return 0;
         }
         return (this.WidthLng.GetHashCode() ^ this.HeightLat.GetHashCode());
      }

      public PointLatLng ToPointLatLng()
      {
         return (PointLatLng) this;
      }

      public override string ToString()
      {
         return ("{WidthLng=" + this.widthLng.ToString(CultureInfo.CurrentCulture) + ", HeightLng=" + this.heightLat.ToString(CultureInfo.CurrentCulture) + "}");
      }

      static SizeLatLng()
      {
         Empty = new SizeLatLng();
      }
   }
}
