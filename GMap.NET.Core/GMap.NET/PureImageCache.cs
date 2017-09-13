
namespace GMap.NET
{
   using System.IO;
   using System;

   /// <summary>
   /// pure abstraction for image cache
   /// </summary>
   public interface PureImageCache
   {
      /// <summary>
      /// puts image to db
      /// </summary>
      /// <param name="tile"></param>
      /// <param name="type"></param>
      /// <param name="pos"></param>
      /// <param name="zoom"></param>
      /// <returns></returns>
      bool PutImageToCache(byte[] tile, int type, GPoint pos, int zoom);

      /// <summary>
      /// gets image from db
      /// </summary>
      /// <param name="type"></param>
      /// <param name="pos"></param>
      /// <param name="zoom"></param>
      /// <returns></returns>
      PureImage GetImageFromCache(int type, GPoint pos, int zoom);

      /// <summary>
      /// delete old tiles beyond a supplied date
      /// </summary>
      /// <param name="date">Tiles older than this will be deleted.</param>
      /// <param name="type">provider dbid or null to use all providers</param>
      /// <returns>The number of deleted tiles.</returns>
      int DeleteOlderThan(DateTime date, int ? type);
   }
}
