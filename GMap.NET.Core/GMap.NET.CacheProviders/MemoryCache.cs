
namespace GMap.NET.CacheProviders
{
   using System.Diagnostics;
   using GMap.NET.Internals;
   using System;

   public class MemoryCache : IDisposable
   {
      readonly KiberTileCache TilesInMemory = new KiberTileCache();

      FastReaderWriterLock kiberCacheLock = new FastReaderWriterLock();

      /// <summary>
      /// the amount of tiles in MB to keep in memmory, default: 22MB, if each ~100Kb it's ~222 tiles
      /// </summary>
      public int Capacity
      {
         get
         {
            kiberCacheLock.AcquireReaderLock();
            try
            {
               return TilesInMemory.MemoryCacheCapacity;
            }
            finally
            {
               kiberCacheLock.ReleaseReaderLock();
            }
         }
         set
         {
            kiberCacheLock.AcquireWriterLock();
            try
            {
               TilesInMemory.MemoryCacheCapacity = value;
            }
            finally
            {
               kiberCacheLock.ReleaseWriterLock();
            }
         }
      }

      /// <summary>
      /// current memmory cache size in MB
      /// </summary>
      public double Size
      {
         get
         {
            kiberCacheLock.AcquireReaderLock();
            try
            {
               return TilesInMemory.MemoryCacheSize;
            }
            finally
            {
               kiberCacheLock.ReleaseReaderLock();
            }
         }
      }

      public void Clear()
      {
         kiberCacheLock.AcquireWriterLock();
         try
         {
            TilesInMemory.Clear();
         }
         finally
         {
            kiberCacheLock.ReleaseWriterLock();
         }
      }

      // ...

      internal byte[] GetTileFromMemoryCache(RawTile tile)
      {
         kiberCacheLock.AcquireReaderLock();
         try
         {
            byte[] ret = null;
            if(TilesInMemory.TryGetValue(tile, out ret))
            {
               return ret;
            }
         }
         finally
         {
            kiberCacheLock.ReleaseReaderLock();
         }
         return null;
      }

      internal void AddTileToMemoryCache(RawTile tile, byte[] data)
      {
         if(data != null)
         {
            kiberCacheLock.AcquireWriterLock();
            try
            {
               if(!TilesInMemory.ContainsKey(tile))
               {
                  TilesInMemory.Add(tile, data);
               }
            }
            finally
            {
               kiberCacheLock.ReleaseWriterLock();
            }
         }
#if DEBUG
         else
         {
            if(Debugger.IsAttached)
            {
               Debugger.Break();
            }
         }
#endif
      }

      internal void RemoveOverload()
      {
         kiberCacheLock.AcquireWriterLock();
         try
         {
            TilesInMemory.RemoveMemoryOverload();
         }
         finally
         {
            kiberCacheLock.ReleaseWriterLock();
         }
      }

      #region IDisposable Members

      ~MemoryCache()
      {
         Dispose(false);
      }

      void Dispose(bool disposing)
      {
         if(kiberCacheLock != null)
         {
            if(disposing)
            {
               Clear();
            }

            kiberCacheLock.Dispose();
            kiberCacheLock = null;
         }
      }

      public void Dispose()
      {
         this.Dispose(true);
         GC.SuppressFinalize(this);
      }

      #endregion
   }
}
