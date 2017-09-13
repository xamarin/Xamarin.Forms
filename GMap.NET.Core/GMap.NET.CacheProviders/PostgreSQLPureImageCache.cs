
namespace GMap.NET.CacheProviders
{
#if PostgreSQL
   using System;
   using System.Diagnostics;
   using System.IO;
   using Npgsql;
   using NpgsqlTypes;
   using GMap.NET.MapProviders;

   /// <summary>
   /// image cache for postgresql server
   /// </summary>
   public class PostgreSQLPureImageCache : PureImageCache, IDisposable
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

      NpgsqlCommand cmdInsert;
      NpgsqlCommand cmdFetch;
      NpgsqlConnection cnGet;
      NpgsqlConnection cnSet;

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
   #region prepare postgresql & cache table

               try
               {
                  // different connections so the multi-thread inserts and selects don't collide on open readers.
                  this.cnGet = new NpgsqlConnection(connectionString);
                  this.cnGet.Open();
                  this.cnSet = new NpgsqlConnection(connectionString);
                  this.cnSet.Open();

                  bool tableexists = false;
                  using(NpgsqlCommand cmd = new NpgsqlCommand())
                  {
                     cmd.CommandText = "SELECT COUNT(*) FROM information_schema.tables WHERE table_name='GMapNETcache'";
                     cmd.Connection = cnGet;
                     object cnt = cmd.ExecuteScalar();
                     tableexists = ((long)cnt == 1);
                  }

                  if(!tableexists)
                  {
                     using(NpgsqlCommand cmd = new NpgsqlCommand())
                     {
                        cmd.Connection = cnGet;

                        // create tile-cache table
                        cmd.CommandText = "CREATE TABLE \"GMapNETcache\" ( \n"
                            + " \"Type\" integer NOT NULL, \n"
                            + " \"Zoom\" integer NOT NULL, \n"
                            + " \"X\"    integer NOT NULL, \n"
                            + " \"Y\"    integer NOT NULL, \n"
                            + " \"Tile\" bytea   NOT NULL, \n"
                            + " CONSTRAINT \"PK_GMapNETcache\" PRIMARY KEY ( \"Type\", \"Zoom\", \"X\", \"Y\" ) )";
                        cmd.ExecuteNonQuery();

                        // allows out-of-line storage but not compression of tile data
                        // see http://www.postgresql.org/docs/9.0/static/storage-toast.html
                        cmd.CommandText = "ALTER TABLE \"GMapNETcache\" \n"
                            + " ALTER COLUMN \"Tile\" SET STORAGE EXTERNAL";
                        cmd.ExecuteNonQuery();

                        // select pk index for cluster operations
                        cmd.CommandText = "ALTER TABLE \"GMapNETcache\" \n"
                            + " CLUSTER ON \"PK_GMapNETcache\"";
                        cmd.ExecuteNonQuery();
                     }
                  }

                  this.cmdFetch = new NpgsqlCommand("SELECT \"Tile\" FROM \"GMapNETcache\" WHERE \"X\"=@x AND \"Y\"=@y AND \"Zoom\"=@zoom AND \"Type\"=@type", cnGet);
                  this.cmdFetch.Parameters.Add("@x", NpgsqlDbType.Integer);
                  this.cmdFetch.Parameters.Add("@y", NpgsqlDbType.Integer);
                  this.cmdFetch.Parameters.Add("@zoom", NpgsqlDbType.Integer);
                  this.cmdFetch.Parameters.Add("@type", NpgsqlDbType.Integer);
                  this.cmdFetch.Prepare();

                  this.cmdInsert = new NpgsqlCommand("INSERT INTO \"GMapNETcache\" ( \"X\", \"Y\", \"Zoom\", \"Type\", \"Tile\" ) VALUES ( @x, @y, @zoom, @type, @tile )", cnSet);
                  this.cmdInsert.Parameters.Add("@x", NpgsqlDbType.Integer);
                  this.cmdInsert.Parameters.Add("@y", NpgsqlDbType.Integer);
                  this.cmdInsert.Parameters.Add("@zoom", NpgsqlDbType.Integer);
                  this.cmdInsert.Parameters.Add("@type", NpgsqlDbType.Integer);
                  this.cmdInsert.Parameters.Add("@tile", NpgsqlDbType.Bytea);
                  this.cmdInsert.Prepare();

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

         return ret;
      }

      public PureImage GetImageFromCache(int type, GPoint pos, int zoom)
      {
         PureImage ret = null;

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
