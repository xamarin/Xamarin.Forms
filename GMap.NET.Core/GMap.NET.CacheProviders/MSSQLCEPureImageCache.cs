
namespace GMap.NET.CacheProviders
{
#if !SQLite
   using System;
   using System.Data;
   using System.Diagnostics;
   using System.IO;
   using SqlCommand = System.Data.SqlServerCe.SqlCeCommand;
   using SqlConnection = System.Data.SqlServerCe.SqlCeConnection;
    using GMap.NET.MapProviders;

   /// <summary>
   /// image cache for ms sql server
   /// </summary>
   public class MsSQLCePureImageCache : PureImageCache, IDisposable
   {
      string cache;
      string gtileCache;

      /// <summary>
      /// local cache location
      /// </summary>
      public string CacheLocation
      {
         get
         {
            return cache;
         }
         set
         {
            cache = value; 
            gtileCache = Path.Combine(cache, "TileDBv3") + Path.DirectorySeparatorChar + GMapProvider.LanguageStr + Path.DirectorySeparatorChar;
         }
      }

      SqlCommand cmdInsert;
      SqlCommand cmdFetch;
      SqlConnection cnGet;
      SqlConnection cnSet;

      /// <summary>
      /// is cache initialized
      /// </summary>
      public volatile bool Initialized = false;

      /// <summary>
      /// inits connection to server
      /// </summary>
      /// <returns></returns>
      public bool Initialize()
      {
         if(!Initialized)
         {
   #region prepare mssql & cache table
            try
            {
               // precrete dir
               if(!Directory.Exists(gtileCache))
               {
                  Directory.CreateDirectory(gtileCache);
               }

               string connectionString = string.Format("Data Source={0}Data.sdf", gtileCache);

               if(!File.Exists(gtileCache + "Data.sdf"))
               {
                  using(System.Data.SqlServerCe.SqlCeEngine engine = new System.Data.SqlServerCe.SqlCeEngine(connectionString))
                  {
                     engine.CreateDatabase();
                  }

                  try
                  {
                     using(SqlConnection c = new SqlConnection(connectionString))
                     {
                        c.Open();

                        using(SqlCommand cmd = new SqlCommand(
                           "CREATE TABLE [GMapNETcache] ( \n"
                  + "   [Type] [int]   NOT NULL, \n"
                  + "   [Zoom] [int]   NOT NULL, \n"
                  + "   [X]    [int]   NOT NULL, \n"
                  + "   [Y]    [int]   NOT NULL, \n"
                  + "   [Tile] [image] NOT NULL, \n"
                  + "   CONSTRAINT [PK_GMapNETcache] PRIMARY KEY (Type, Zoom, X, Y) \n"
                  + ")", c))
                        {
                           cmd.ExecuteNonQuery();
                        }
                     }
                  }
                  catch(Exception ex)
                  {
                     try
                     {
                        File.Delete(gtileCache + "Data.sdf");
                     }
                     catch
                     {
                     }

                     throw ex;
                  }
               }

               // different connections so the multi-thread inserts and selects don't collide on open readers.
               this.cnGet = new SqlConnection(connectionString);
               this.cnGet.Open();
               this.cnSet = new SqlConnection(connectionString);
               this.cnSet.Open();

               this.cmdFetch = new SqlCommand("SELECT [Tile] FROM [GMapNETcache] WITH (NOLOCK) WHERE [X]=@x AND [Y]=@y AND [Zoom]=@zoom AND [Type]=@type", cnGet);
               this.cmdFetch.Parameters.Add("@x", System.Data.SqlDbType.Int);
               this.cmdFetch.Parameters.Add("@y", System.Data.SqlDbType.Int);
               this.cmdFetch.Parameters.Add("@zoom", System.Data.SqlDbType.Int);
               this.cmdFetch.Parameters.Add("@type", System.Data.SqlDbType.Int);
               this.cmdFetch.Prepare();

               this.cmdInsert = new SqlCommand("INSERT INTO [GMapNETcache] ( [X], [Y], [Zoom], [Type], [Tile] ) VALUES ( @x, @y, @zoom, @type, @tile )", cnSet);
               this.cmdInsert.Parameters.Add("@x", System.Data.SqlDbType.Int);
               this.cmdInsert.Parameters.Add("@y", System.Data.SqlDbType.Int);
               this.cmdInsert.Parameters.Add("@zoom", System.Data.SqlDbType.Int);
               this.cmdInsert.Parameters.Add("@type", System.Data.SqlDbType.Int);
               this.cmdInsert.Parameters.Add("@tile", System.Data.SqlDbType.Image); //, calcmaximgsize);
               //can't prepare insert because of the IMAGE field having a variable size.  Could set it to some 'maximum' size?

               Initialized = true;
            }
            catch(Exception ex)
            {
               Initialized = false;
               Debug.WriteLine(ex.Message);
            }
   #endregion
         }
         return Initialized;
      }

   #region IDisposable Members
      public void Dispose()
      {
         lock(cmdInsert)
         {
            if(cmdInsert != null)
            {
               cmdInsert.Dispose();
               cmdInsert = null;
            }

            if(cnSet != null)
            {
               cnSet.Dispose();
               cnSet = null;
            }
         }

         lock(cmdFetch)
         {
            if(cmdFetch != null)
            {
               cmdFetch.Dispose();
               cmdFetch = null;
            }

            if(cnGet != null)
            {
               cnGet.Dispose();
               cnGet = null;
            }
         }
         Initialized = false;
      }
   #endregion

   #region PureImageCache Members
      public bool PutImageToCache(byte[] tile, int type, GPoint pos, int zoom)
      {
         bool ret = true;
         {
            if(Initialize())
            {
               try
               {
                  lock(cmdInsert)
                  {
                     cmdInsert.Parameters["@x"].Value = pos.X;
                     cmdInsert.Parameters["@y"].Value = pos.Y;
                     cmdInsert.Parameters["@zoom"].Value = zoom;
                     cmdInsert.Parameters["@type"].Value = type;
                     cmdInsert.Parameters["@tile"].Value = tile;
                     cmdInsert.ExecuteNonQuery();
                  }
               }
               catch(Exception ex)
               {
                  Debug.WriteLine(ex.ToString());
                  ret = false;
                  Dispose();
               }
            }
         }
         return ret;
      }

      public PureImage GetImageFromCache(int type, GPoint pos, int zoom)
      {
         PureImage ret = null;
         {
            if(Initialize())
            {
               try
               {
                  object odata = null;
                  lock(cmdFetch)
                  {
                     cmdFetch.Parameters["@x"].Value = pos.X;
                     cmdFetch.Parameters["@y"].Value = pos.Y;
                     cmdFetch.Parameters["@zoom"].Value = zoom;
                     cmdFetch.Parameters["@type"].Value = type;
                     odata = cmdFetch.ExecuteScalar();
                  }

                  if(odata != null && odata != DBNull.Value)
                  {
                     byte[] tile = (byte[])odata;
                     if(tile != null && tile.Length > 0)
                     {
                        if(GMapProvider.TileImageProxy != null)
                        {
                           ret = GMapProvider.TileImageProxy.FromArray(tile);
                        }
                     }
                     tile = null;
                  }
               }
               catch(Exception ex)
               {
                  Debug.WriteLine(ex.ToString());
                  ret = null;
                  Dispose();
               }
            }
         }
         return ret;
      }

      int PureImageCache.DeleteOlderThan(DateTime date, int ? type)
      {
         throw new NotImplementedException();
      }
   #endregion
   }
#endif
}
