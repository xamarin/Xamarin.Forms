
namespace GMap.NET.CacheProviders
{
   using System;
   using System.Data.SqlClient;
   using System.Diagnostics;
   using System.IO;
   using GMap.NET.MapProviders;

   /// <summary>
   /// image cache for ms sql server
   /// optimized by mmurfinsimmons@gmail.com
   /// </summary>
   public class MsSQLPureImageCache : PureImageCache, IDisposable
   {
      string connectionString = string.Empty;
      public string ConnectionString
      {
         get
         {
            return connectionString;
         }
         set
         {
            if(connectionString != value)
            {
               connectionString = value;

               if(Initialized)
               {
                  Dispose();
                  Initialize();
               }
            }
         }
      }

      SqlCommand cmdInsert;
      SqlCommand cmdFetch;
      SqlConnection cnGet;
      SqlConnection cnSet;

      bool initialized = false;

      /// <summary>
      /// is cache initialized
      /// </summary>
      public bool Initialized
      {
         get
         {
            lock(this)
            {
               return initialized;
            }
         }
         private set
         {
            lock(this)
            {
               initialized = value;
            }
         }
      }

      /// <summary>
      /// inits connection to server
      /// </summary>
      /// <returns></returns>
      public bool Initialize()
      {
         lock(this)
         {
            if(!Initialized)
            {
               #region prepare mssql & cache table
               try
               {
                  // different connections so the multi-thread inserts and selects don't collide on open readers.
                  this.cnGet = new SqlConnection(connectionString);
                  this.cnGet.Open();
                  this.cnSet = new SqlConnection(connectionString);
                  this.cnSet.Open();

                  bool tableexists = false;
                  using(SqlCommand cmd = new SqlCommand("select object_id('GMapNETcache')", cnGet))
                  {
                     object objid = cmd.ExecuteScalar();
                     tableexists = (objid != null && objid != DBNull.Value);
                  }
                  if(!tableexists)
                  {
                     using(SqlCommand cmd = new SqlCommand(
                        "CREATE TABLE [GMapNETcache] ( \n"
                  + "   [Type] [int]   NOT NULL, \n"
                  + "   [Zoom] [int]   NOT NULL, \n"
                  + "   [X]    [int]   NOT NULL, \n"
                  + "   [Y]    [int]   NOT NULL, \n"
                  + "   [Tile] [image] NOT NULL, \n"
                  + "   CONSTRAINT [PK_GMapNETcache] PRIMARY KEY CLUSTERED (Type, Zoom, X, Y) \n"
                  + ")", cnGet))
                     {
                        cmd.ExecuteNonQuery();
                     }
                  }

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
                  this.initialized = false;
                  Debug.WriteLine(ex.Message);
               }
               #endregion
            }
            return Initialized;
         }
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

      /// <summary>
      /// NotImplemented
      /// </summary>
      /// <param name="date"></param>
      /// <param name="type"></param>
      /// <returns></returns>
      int PureImageCache.DeleteOlderThan(DateTime date, int ? type)
      {
         throw new NotImplementedException();
      }
      #endregion
   }
}
