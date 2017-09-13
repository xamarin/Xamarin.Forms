
namespace GMap.NET.CacheProviders
{
#if MySQL
   using System;
   using System.Data;
   using System.Diagnostics;
   using System.IO;
   using GMap.NET;
   using MySql.Data.MySqlClient;
   using GMap.NET.MapProviders;

   /// <summary>
   /// image cache for mysql server
   /// </summary>
   public class MySQLPureImageCache : PureImageCache, IDisposable
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

      MySqlCommand cmdInsert;
      MySqlCommand cmdFetch;
      MySqlConnection cnGet;
      MySqlConnection cnSet;

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
            if(!initialized)
            {
               #region prepare mssql & cache table
               try
               {
                  // different connections so the multi-thread inserts and selects don't collide on open readers.
                  this.cnGet = new MySqlConnection(connectionString);
                  this.cnGet.Open();
                  this.cnSet = new MySqlConnection(connectionString);
                  this.cnSet.Open();

                  {
                     using(MySqlCommand cmd = new MySqlCommand(
                        @" CREATE TABLE IF NOT EXISTS `gmapnetcache` (
                             `Type` int(10) NOT NULL,
                             `Zoom` int(10) NOT NULL,
                             `X` int(10) NOT NULL,
                             `Y` int(10) NOT NULL,
                             `Tile` longblob NOT NULL,
                             PRIMARY KEY (`Type`,`Zoom`,`X`,`Y`)
                           ) ENGINE=InnoDB DEFAULT CHARSET=utf8;", cnGet))
                     {
                        cmd.ExecuteNonQuery();
                     }
                  }

                  this.cmdFetch = new MySqlCommand("SELECT Tile FROM `gmapnetcache` WHERE Type=@type AND Zoom=@zoom AND X=@x AND Y=@y", cnGet);
                  this.cmdFetch.Parameters.Add("@type", MySqlDbType.Int32);
                  this.cmdFetch.Parameters.Add("@zoom", MySqlDbType.Int32);
                  this.cmdFetch.Parameters.Add("@x", MySqlDbType.Int32);
                  this.cmdFetch.Parameters.Add("@y", MySqlDbType.Int32);
                  this.cmdFetch.Prepare();

                  this.cmdInsert = new MySqlCommand("INSERT INTO `gmapnetcache` ( Type, Zoom, X, Y, Tile ) VALUES ( @type, @zoom, @x, @y, @tile )", cnSet);
                  this.cmdInsert.Parameters.Add("@type", MySqlDbType.Int32);
                  this.cmdInsert.Parameters.Add("@zoom", MySqlDbType.Int32);
                  this.cmdInsert.Parameters.Add("@x", MySqlDbType.Int32);
                  this.cmdInsert.Parameters.Add("@y", MySqlDbType.Int32);
                  this.cmdInsert.Parameters.Add("@tile", MySqlDbType.Blob); //, calcmaximgsize);
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
            return initialized;
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
                     cnSet.Ping();

                     if(cnSet.State != ConnectionState.Open)
                     {
                        cnSet.Open();
                     }

                     cmdInsert.Parameters["@type"].Value = type;
                     cmdInsert.Parameters["@zoom"].Value = zoom;
                     cmdInsert.Parameters["@x"].Value = pos.X;
                     cmdInsert.Parameters["@y"].Value = pos.Y;
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
                     cnGet.Ping();

                     if(cnGet.State != ConnectionState.Open)
                     {
                        cnGet.Open();
                     }

                     cmdFetch.Parameters["@type"].Value = type;
                     cmdFetch.Parameters["@zoom"].Value = zoom;
                     cmdFetch.Parameters["@x"].Value = pos.X;
                     cmdFetch.Parameters["@y"].Value = pos.Y;
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
