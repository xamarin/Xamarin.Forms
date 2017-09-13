
namespace GMap.NET.Internals
{
   using System.IO;
   using System;

   /// <summary>
   /// cache queue item
   /// </summary>
   internal struct CacheQueueItem
   {
      public RawTile Tile;
      public byte[] Img;
      public CacheUsage CacheType;

      public CacheQueueItem(RawTile tile, byte[] Img, CacheUsage cacheType)
      {
         this.Tile = tile;
         this.Img = Img;
         this.CacheType = cacheType;
      }

      public override string ToString()
      {
         return Tile + ", CacheType:" + CacheType;
      }

      public void Clear()
      {
         Img = null;
      }
   }

   internal enum CacheUsage
   {
      First = 2,
      Second = 4,
      Both = First | Second
   }
}
