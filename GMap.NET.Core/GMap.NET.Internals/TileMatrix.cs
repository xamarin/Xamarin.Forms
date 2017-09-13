
namespace GMap.NET.Internals
{
   using System.Collections.Generic;
   using System.Threading;
   using System.Diagnostics;
   using System;

   /// <summary>
   /// matrix for tiles
   /// </summary>
   public class TileMatrix : IDisposable
   {
      List<Dictionary<GPoint, Tile>> Levels = new List<Dictionary<GPoint, Tile>>(33);
      FastReaderWriterLock Lock = new FastReaderWriterLock();

      public TileMatrix()
      {
         for(int i = 0; i < Levels.Capacity; i++)
         {
             Levels.Add(new Dictionary<GPoint, Tile>(55, new GPointComparer()));
         }
      }

      public void ClearAllLevels()
      {
         Lock.AcquireWriterLock();
         try
         {
            foreach(var matrix in Levels)
            {
               foreach(var t in matrix)
               {
                  t.Value.Dispose();
               }
               matrix.Clear();
            }
         }
         finally
         {
            Lock.ReleaseWriterLock();
         }
      }

      public void ClearLevel(int zoom)
      {
         Lock.AcquireWriterLock();
         try
         {
            if(zoom < Levels.Count)
            {
               var l = Levels[zoom];

               foreach(var t in l)
               {
                  t.Value.Dispose();
               }

               l.Clear();
            }
         }
         finally
         {
            Lock.ReleaseWriterLock();
         }
      }

      List<KeyValuePair<GPoint, Tile>> tmp = new List<KeyValuePair<GPoint, Tile>>(44);

      public void ClearLevelAndPointsNotIn(int zoom, List<DrawTile> list)
      {
         Lock.AcquireWriterLock();
         try
         {
            if(zoom < Levels.Count)
            {
               var l = Levels[zoom];

               tmp.Clear();

               foreach(var t in l)
               {
                  if(!list.Exists(p => p.PosXY == t.Key))
                  {
                     tmp.Add(t);
                  }
               }

               foreach(var r in tmp)
               {
                  l.Remove(r.Key);
                  r.Value.Dispose();
               }

               tmp.Clear();
            }
         }
         finally
         {
            Lock.ReleaseWriterLock();
         }
      }

      public void ClearLevelsBelove(int zoom)
      {
         Lock.AcquireWriterLock();
         try
         {
            if(zoom - 1 < Levels.Count)
            {
               for(int i = zoom - 1; i >= 0; i--)
               {
                  var l = Levels[i];

                  foreach(var t in l)
                  {
                     t.Value.Dispose();
                  }

                  l.Clear();
               }
            }
         }
         finally
         {
            Lock.ReleaseWriterLock();
         }
      }

      public void ClearLevelsAbove(int zoom)
      {
         Lock.AcquireWriterLock();
         try
         {
            if(zoom + 1 < Levels.Count)
            {
               for(int i = zoom + 1; i < Levels.Count; i++)
               {
                  var l = Levels[i];

                  foreach(var t in l)
                  {
                     t.Value.Dispose();
                  }

                  l.Clear();
               }
            }
         }
         finally
         {
            Lock.ReleaseWriterLock();
         }
      }

      public void EnterReadLock()
      {
         Lock.AcquireReaderLock();
      }

      public void LeaveReadLock()
      {
         Lock.ReleaseReaderLock();
      }

      public Tile GetTileWithNoLock(int zoom, GPoint p)
      {
         Tile ret = Tile.Empty;

         //if(zoom < Levels.Count)
         {
            Levels[zoom].TryGetValue(p, out ret);
         }

         return ret;
      }

      public Tile GetTileWithReadLock(int zoom, GPoint p)
      {
         Tile ret = Tile.Empty;

         Lock.AcquireReaderLock();
         try
         {
            ret = GetTileWithNoLock(zoom, p);
         }
         finally
         {
            Lock.ReleaseReaderLock();
         }

         return ret;
      }

      public void SetTile(Tile t)
      {
         Lock.AcquireWriterLock();
         try
         {
            if(t.Zoom < Levels.Count)
            {
               Levels[t.Zoom][t.Pos] = t;
            }
         }
         finally
         {
            Lock.ReleaseWriterLock();
         }
      }

      #region IDisposable Members

      ~TileMatrix()
      {
         Dispose(false);
      }

      void Dispose(bool disposing)
      {
         if(Lock != null)
         {
            if(disposing)
            {
               ClearAllLevels();
            }

            Levels.Clear();
            Levels = null;

            tmp.Clear();
            tmp = null;

            Lock.Dispose();
            Lock = null;
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
