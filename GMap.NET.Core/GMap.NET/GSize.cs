
namespace GMap.NET
{
   using System.Globalization;

   /// <summary>
   /// the size
   /// </summary>
   public struct GSize
   {
      public static readonly GSize Empty = new GSize();

      private long width;
      private long height;

      public GSize(GPoint pt)
      {
         width = pt.X;
         height = pt.Y;
      }

      public GSize(long width, long height)
      {
         this.width = width;
         this.height = height;
      }

      public static GSize operator +(GSize sz1, GSize sz2)
      {
         return Add(sz1, sz2);
      }

      public static GSize operator -(GSize sz1, GSize sz2)
      {
         return Subtract(sz1, sz2);
      }

      public static bool operator ==(GSize sz1, GSize sz2)
      {
         return sz1.Width == sz2.Width && sz1.Height == sz2.Height;
      }

      public static bool operator !=(GSize sz1, GSize sz2)
      {
         return !(sz1 == sz2);
      }

      public static explicit operator GPoint(GSize size)
      {
         return new GPoint(size.Width, size.Height);
      }

      public bool IsEmpty
      {
         get
         {
            return width == 0 && height == 0;
         }
      }

      public long Width
      {
         get
         {
            return width;
         }
         set
         {
            width = value;
         }
      }

      public long Height
      {
         get
         {
            return height;
         }
         set
         {
            height = value;
         }
      }

      public static GSize Add(GSize sz1, GSize sz2)
      {
         return new GSize(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
      }

      public static GSize Subtract(GSize sz1, GSize sz2)
      {
         return new GSize(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
      }

      public override bool Equals(object obj)
      {
         if(!(obj is GSize))
            return false;

         GSize comp = (GSize)obj;
         // Note value types can't have derived classes, so we don't need to
         //
         return (comp.width == this.width) &&
                   (comp.height == this.height);
      }

      public override int GetHashCode()
      {
         if(this.IsEmpty)
         {
            return 0;
         }
         return (Width.GetHashCode() ^ Height.GetHashCode());
      }

      public override string ToString()
      {
         return "{Width=" + width.ToString(CultureInfo.CurrentCulture) + ", Height=" + height.ToString(CultureInfo.CurrentCulture) + "}";
      }
   }
}
