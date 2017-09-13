
namespace GMap.NET.CacheProviders
{
#if SQLite

   using System.Collections.Generic;
   using System.Data.Common;
   using System.IO;
   using System.Text;
   using System;
   using System.Diagnostics;
   using System.Globalization;
   using GMap.NET.MapProviders;
   using System.Threading;

#if !MONO
   using System.Data.SQLite;
   using GMap.NET.Internals;
#else
   using SQLiteConnection = Mono.Data.Sqlite.SqliteConnection;
   using SQLiteTransaction = Mono.Data.Sqlite.SqliteTransaction;
   using SQLiteCommand = Mono.Data.Sqlite.SqliteCommand;
   using SQLiteDataReader = Mono.Data.Sqlite.SqliteDataReader;
   using SQLiteParameter = Mono.Data.Sqlite.SqliteParameter;
#endif

   /// <summary>
   /// ultra fast cache system for tiles
   /// </summary>
   public class SQLitePureImageCache : PureImageCache
   {
#if !PocketPC
#if !MONO
      static SQLitePureImageCache()
      {
         AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
      }

      static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
      {
         if(args.Name.StartsWith("System.Data.SQLite", StringComparison.OrdinalIgnoreCase))
         {
            string appDataDir = CacheLocator.GetApplicationDataFolderPath();
            if(string.IsNullOrEmpty(appDataDir))
            {
               return null;
            }    

            string dllDir = appDataDir + "DllCache" + Path.DirectorySeparatorChar;
            string dll = dllDir + "SQLite_v98_NET" + Environment.Version.Major + "_" + (IntPtr.Size == 8 ? "x64" : "x86") + Path.DirectorySeparatorChar + "System.Data.SQLite.DLL";
            if(!File.Exists(dll))
            {
               string dir = Path.GetDirectoryName(dll);
               if(!Directory.Exists(dir))
               {
                  Directory.CreateDirectory(dir);
               }

               Debug.WriteLine("Saving to DllCache: " + dll);

               if(Environment.Version.Major == 2)
               {
                  using(MemoryStream gzipDll = new MemoryStream((IntPtr.Size == 8 ? Properties.Resources.System_Data_SQLite_x64_NET2_dll : Properties.Resources.System_Data_SQLite_x86_NET2_dll)))
                  {
                     using(var gs = new System.IO.Compression.GZipStream(gzipDll, System.IO.Compression.CompressionMode.Decompress))
                     {
                        using(MemoryStream exctDll = new MemoryStream())
                        {
                           byte[] tmp = new byte[1024 * 256];
                           int r = 0;
                           while((r = gs.Read(tmp, 0, tmp.Length)) > 0)
                           {
                              exctDll.Write(tmp, 0, r);
                           }
                           File.WriteAllBytes(dll, exctDll.ToArray());
                        }
                     }
                  }
               }
               else if(Environment.Version.Major == 4)
               {
                  using(MemoryStream gzipDll = new MemoryStream((IntPtr.Size == 8 ? Properties.Resources.System_Data_SQLite_x64_NET4_dll : Properties.Resources.System_Data_SQLite_x86_NET4_dll)))
                  {
                     using(var gs = new System.IO.Compression.GZipStream(gzipDll, System.IO.Compression.CompressionMode.Decompress))
                     {
                        using(MemoryStream exctDll = new MemoryStream())
                        {
                           byte[] tmp = new byte[1024 * 256];
                           int r = 0;
                           while((r = gs.Read(tmp, 0, tmp.Length)) > 0)
                           {
                              exctDll.Write(tmp, 0, r);
                           }
                           File.WriteAllBytes(dll, exctDll.ToArray());
                        }
                     }
                  }
               }
            }

            Debug.WriteLine("Assembly.LoadFile: " + dll);

            return System.Reflection.Assembly.LoadFile(dll);
         }
         return null;
      }

      static int ping = 0;

      /// <summary>
      /// triggers dynamic sqlite loading
      /// </summary>
      public static void Ping()
      {
          if (++ping == 1)
          {
              Trace.WriteLine("SQLiteVersion: " + SQLiteConnection.SQLiteVersion + " | " + SQLiteConnection.SQLiteSourceId + " | " + SQLiteConnection.DefineConstants);
          }
      }
#endif
#endif

      string cache;
      string gtileCache;
      string dir;
      string db;
      bool Created = false;

      public string GtileCache
      {
         get
         {
            return gtileCache;
         }
      }

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

            gtileCache = Path.Combine(cache, "TileDBv5") + Path.DirectorySeparatorChar;

            dir = gtileCache + GMapProvider.LanguageStr + Path.DirectorySeparatorChar;

            // precreate dir
            if(!Directory.Exists(dir))
            {
               Directory.CreateDirectory(dir);
            }

#if !MONO
            SQLiteConnection.ClearAllPools();              
#endif
            // make empty db
            {
               db = dir + "Data.gmdb";

               if(!File.Exists(db))
               {
                  Created = CreateEmptyDB(db);
               }
               else
               {
                  Created = AlterDBAddTimeColumn(db);
               }

               CheckPreAllocation();

               //var connBuilder = new SQLiteConnectionStringBuilder();
               //connBuilder.DataSource = "c:\filePath.db";
               //connBuilder.Version = 3;
               //connBuilder.PageSize = 4096;
               //connBuilder.JournalMode = SQLiteJournalModeEnum.Wal;
               //connBuilder.Pooling = true;
               //var x = connBuilder.ToString();
#if !MONO                                      
               ConnectionString = string.Format("Data Source=\"{0}\";Page Size=32768;Pooling=True", db); //;Journal Mode=Wal
#else
               ConnectionString = string.Format("Version=3,URI=file://{0},FailIfMissing=True,Page Size=32768,Pooling=True", db);
#endif
            }

            // clear old attachments
            AttachedCaches.Clear();
            RebuildFinnalSelect();

            // attach all databases from main cache location
#if !PocketPC
            var dbs = Directory.GetFiles(dir, "*.gmdb", SearchOption.AllDirectories);
#else
            var dbs = Directory.GetFiles(dir, "*.gmdb");
#endif
            foreach(var d in dbs)
            {
               if(d != db)
               {
                  Attach(d);
               }
            }
         }
      }

      /// <summary>
      /// pre-allocate 32MB free space 'ahead' if needed,
      /// decreases fragmentation
      /// </summary>
      void CheckPreAllocation()
      {
         {
            byte[] pageSizeBytes = new byte[2];
            byte[] freePagesBytes = new byte[4];

            lock(this)
            {
               using(var dbf = File.Open(db, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
               {
                  dbf.Seek(16, SeekOrigin.Begin);

#if (!PocketPC && !MONO)
                  dbf.Lock(16, 2);
                  dbf.Read(pageSizeBytes, 0, 2);
                  dbf.Unlock(16, 2);

                  dbf.Seek(36, SeekOrigin.Begin);

                  dbf.Lock(36, 4);
                  dbf.Read(freePagesBytes, 0, 4);
                  dbf.Unlock(36, 4);
#else
                  dbf.Read(pageSizeBytes, 0, 2);
                  dbf.Seek(36, SeekOrigin.Begin);
                  dbf.Read(freePagesBytes, 0, 4);
#endif

                  dbf.Close();
               }
            }

            if(BitConverter.IsLittleEndian)
            {
               Array.Reverse(pageSizeBytes);
               Array.Reverse(freePagesBytes);
            }
            UInt16 pageSize = BitConverter.ToUInt16(pageSizeBytes, 0);
            UInt32 freePages = BitConverter.ToUInt32(freePagesBytes, 0);

            var freeMB = (pageSize * freePages) / (1024.0 * 1024.0);

#if !PocketPC
            int addSizeMB = 32;
            int waitUntilMB = 4;
#else
            int addSizeMB = 4; // reduce due to test in emulator
            int waitUntilMB = 2;
#endif

            Debug.WriteLine("FreePageSpace in cache: " + freeMB + "MB | " + freePages + " pages");

            if(freeMB <= waitUntilMB)
            {
               PreAllocateDB(db, addSizeMB);
            }
         }
      }

      #region -- import / export --
      public static bool CreateEmptyDB(string file)
      {
         bool ret = true;

         try
         {
            string dir = Path.GetDirectoryName(file);
            if(!Directory.Exists(dir))
            {
               Directory.CreateDirectory(dir);
            }

            using(SQLiteConnection cn = new SQLiteConnection())
            {
#if !MONO
               cn.ConnectionString = string.Format("Data Source=\"{0}\";FailIfMissing=False;Page Size=32768", file);
#else
               cn.ConnectionString = string.Format("Version=3,URI=file://{0},FailIfMissing=False,Page Size=32768", file);
#endif
               cn.Open();
               {
                  using(DbTransaction tr = cn.BeginTransaction())
                  {
                     try
                     {
                        using(DbCommand cmd = cn.CreateCommand())
                        {
                           cmd.Transaction = tr;
#if !PocketPC
                           cmd.CommandText = Properties.Resources.CreateTileDb;
#else
                           cmd.CommandText = GMap.NET.WindowsMobile.Properties.Resources.CreateTileDb;
#endif
                           cmd.ExecuteNonQuery();
                        }
                        tr.Commit();
                     }
                     catch(Exception exx)
                     {
#if MONO
                        Console.WriteLine("CreateEmptyDB: " + exx.ToString());
#endif
                        Debug.WriteLine("CreateEmptyDB: " + exx.ToString());

                        tr.Rollback();
                        ret = false;
                     }
                  }
                  cn.Close();
               }
            }
         }
         catch(Exception ex)
         {
#if MONO
            Console.WriteLine("CreateEmptyDB: " + ex.ToString());
#endif
            Debug.WriteLine("CreateEmptyDB: " + ex.ToString());
            ret = false;
         }
         return ret;
      }

      public static bool PreAllocateDB(string file, int addSizeInMBytes)
      {
         bool ret = true;

         try
         {
            Debug.WriteLine("PreAllocateDB: " + file + ", +" + addSizeInMBytes + "MB");

            using(SQLiteConnection cn = new SQLiteConnection())
            {
#if !MONO
               cn.ConnectionString = string.Format("Data Source=\"{0}\";FailIfMissing=False;Page Size=32768", file);
#else
               cn.ConnectionString = string.Format("Version=3,URI=file://{0},FailIfMissing=False,Page Size=32768", file);
#endif
               cn.Open();
               {
                  using(DbTransaction tr = cn.BeginTransaction())
                  {
                     try
                     {
                        using(DbCommand cmd = cn.CreateCommand())
                        {
                           cmd.Transaction = tr;
                           cmd.CommandText = string.Format("create table large (a); insert into large values (zeroblob({0})); drop table large;", addSizeInMBytes * 1024 * 1024);
                           cmd.ExecuteNonQuery();
                        }
                        tr.Commit();
                     }
                     catch(Exception exx)
                     {
#if MONO
                        Console.WriteLine("PreAllocateDB: " + exx.ToString());
#endif
                        Debug.WriteLine("PreAllocateDB: " + exx.ToString());

                        tr.Rollback();
                        ret = false;
                     }
                  }
                  cn.Close();
               }
            }
         }
         catch(Exception ex)
         {
#if MONO
            Console.WriteLine("PreAllocateDB: " + ex.ToString());
#endif
            Debug.WriteLine("PreAllocateDB: " + ex.ToString());
            ret = false;
         }
         return ret;
      }

      private static bool AlterDBAddTimeColumn(string file)
      {
         bool ret = true;

         try
         {
            if(File.Exists(file))
            {
               using(SQLiteConnection cn = new SQLiteConnection())
               {
#if !MONO
                  cn.ConnectionString = string.Format("Data Source=\"{0}\";FailIfMissing=False;Page Size=32768;Pooling=True", file);
#else
                  cn.ConnectionString = string.Format("Version=3,URI=file://{0},FailIfMissing=False,Page Size=32768,Pooling=True", file);
#endif
                  cn.Open();
                  {
                     using(DbTransaction tr = cn.BeginTransaction())
                     {
                        bool? NoCacheTimeColumn = null;

                        try
                        {
                           using(DbCommand cmd = new SQLiteCommand("SELECT CacheTime FROM Tiles", cn))
                           {
                              cmd.Transaction = tr;

                              using(DbDataReader rd = cmd.ExecuteReader())
                              {
                                 rd.Close();
                              }
                              NoCacheTimeColumn = false;
                           }
                        }
                        catch(Exception ex)
                        {
                           if(ex.Message.Contains("no such column: CacheTime"))
                           {
                              NoCacheTimeColumn = true;
                           }
                           else
                           {
                              throw ex;
                           }
                        }

                        try
                        {
                           if(NoCacheTimeColumn.HasValue && NoCacheTimeColumn.Value)
                           {
                              using(DbCommand cmd = cn.CreateCommand())
                              {
                                 cmd.Transaction = tr;

                                 cmd.CommandText = "ALTER TABLE Tiles ADD CacheTime DATETIME";

                                 cmd.ExecuteNonQuery();
                              }
                              tr.Commit();
                              NoCacheTimeColumn = false;
                           }
                        }
                        catch(Exception exx)
                        {
#if MONO
                           Console.WriteLine("AlterDBAddTimeColumn: " + exx.ToString());
#endif
                           Debug.WriteLine("AlterDBAddTimeColumn: " + exx.ToString());

                           tr.Rollback();
                           ret = false;
                        }
                     }
                     cn.Close();
                  }
               }
            }
            else
            {
               ret = false;
            }
         }
         catch(Exception ex)
         {
#if MONO
            Console.WriteLine("AlterDBAddTimeColumn: " + ex.ToString());
#endif
            Debug.WriteLine("AlterDBAddTimeColumn: " + ex.ToString());
            ret = false;
         }
         return ret;
      }

      public static bool VacuumDb(string file)
      {
         bool ret = true;

         try
         {
            using(SQLiteConnection cn = new SQLiteConnection())
            {
#if !MONO
               cn.ConnectionString = string.Format("Data Source=\"{0}\";FailIfMissing=True;Page Size=32768", file);
#else
               cn.ConnectionString = string.Format("Version=3,URI=file://{0},FailIfMissing=True,Page Size=32768", file);
#endif
               cn.Open();
               {
                  using(DbCommand cmd = cn.CreateCommand())
                  {
                     cmd.CommandText = "vacuum;";
                     cmd.ExecuteNonQuery();
                  }
                  cn.Close();
               }
            }
         }
         catch(Exception ex)
         {
            Debug.WriteLine("VacuumDb: " + ex.ToString());
            ret = false;
         }
         return ret;
      }

      public static bool ExportMapDataToDB(string sourceFile, string destFile)
      {
         bool ret = true;

         try
         {
            if(!File.Exists(destFile))
            {
               ret = CreateEmptyDB(destFile);
            }

            if(ret)
            {
               using(SQLiteConnection cn1 = new SQLiteConnection())
               {
#if !MONO
                  cn1.ConnectionString = string.Format("Data Source=\"{0}\";Page Size=32768", sourceFile);
#else
                  cn1.ConnectionString = string.Format("Version=3,URI=file://{0},FailIfMissing=True,Page Size=32768", sourceFile);
#endif

                  cn1.Open();
                  if(cn1.State == System.Data.ConnectionState.Open)
                  {
                     using(SQLiteConnection cn2 = new SQLiteConnection())
                     {
#if !MONO
                        cn2.ConnectionString = string.Format("Data Source=\"{0}\";Page Size=32768", destFile);
#else
                        cn2.ConnectionString = string.Format("Version=3,URI=file://{0},FailIfMissing=True,Page Size=32768", destFile);
#endif
                        cn2.Open();
                        if(cn2.State == System.Data.ConnectionState.Open)
                        {
                           using(SQLiteCommand cmd = new SQLiteCommand(string.Format("ATTACH DATABASE \"{0}\" AS Source", sourceFile), cn2))
                           {
                              cmd.ExecuteNonQuery();
                           }

                           using(SQLiteTransaction tr = cn2.BeginTransaction())
                           {
                              try
                              {
                                 List<long> add = new List<long>();
                                 using(SQLiteCommand cmd = new SQLiteCommand("SELECT id, X, Y, Zoom, Type FROM Tiles;", cn1))
                                 {
                                    using(SQLiteDataReader rd = cmd.ExecuteReader())
                                    {
                                       while(rd.Read())
                                       {
                                          long id = rd.GetInt64(0);
                                          using(SQLiteCommand cmd2 = new SQLiteCommand(string.Format("SELECT id FROM Tiles WHERE X={0} AND Y={1} AND Zoom={2} AND Type={3};", rd.GetInt32(1), rd.GetInt32(2), rd.GetInt32(3), rd.GetInt32(4)), cn2))
                                          {
                                             using(SQLiteDataReader rd2 = cmd2.ExecuteReader())
                                             {
                                                if(!rd2.Read())
                                                {
                                                   add.Add(id);
                                                }
                                             }
                                          }
                                       }
                                    }
                                 }

                                 foreach(long id in add)
                                 {
                                    using(SQLiteCommand cmd = new SQLiteCommand(string.Format("INSERT INTO Tiles(X, Y, Zoom, Type, CacheTime) SELECT X, Y, Zoom, Type, CacheTime FROM Source.Tiles WHERE id={0}; INSERT INTO TilesData(id, Tile) Values((SELECT last_insert_rowid()), (SELECT Tile FROM Source.TilesData WHERE id={0}));", id), cn2))
                                    {
                                       cmd.Transaction = tr;
                                       cmd.ExecuteNonQuery();
                                    }
                                 }
                                 add.Clear();

                                 tr.Commit();
                              }
                              catch(Exception exx)
                              {
                                 Debug.WriteLine("ExportMapDataToDB: " + exx.ToString());
                                 tr.Rollback();
                                 ret = false;
                              }
                           }

                           using(SQLiteCommand cmd = new SQLiteCommand("DETACH DATABASE Source;", cn2))
                           {
                              cmd.ExecuteNonQuery();
                           }
                        }
                     }
                  }
               }
            }
         }
         catch(Exception ex)
         {
            Debug.WriteLine("ExportMapDataToDB: " + ex.ToString());
            ret = false;
         }
         return ret;
      }
      #endregion

      static readonly string singleSqlSelect = "SELECT Tile FROM main.TilesData WHERE id = (SELECT id FROM main.Tiles WHERE X={0} AND Y={1} AND Zoom={2} AND Type={3})";
      static readonly string singleSqlInsert = "INSERT INTO main.Tiles(X, Y, Zoom, Type, CacheTime) VALUES(@p1, @p2, @p3, @p4, @p5)";
      static readonly string singleSqlInsertLast = "INSERT INTO main.TilesData(id, Tile) VALUES((SELECT last_insert_rowid()), @p1)";

      string ConnectionString;

      readonly List<string> AttachedCaches = new List<string>();
      string finnalSqlSelect = singleSqlSelect;
      string attachSqlQuery = string.Empty;
      string detachSqlQuery = string.Empty;

      void RebuildFinnalSelect()
      {
         finnalSqlSelect = null;
         finnalSqlSelect = singleSqlSelect;

         attachSqlQuery = null;
         attachSqlQuery = string.Empty;

         detachSqlQuery = null;
         detachSqlQuery = string.Empty;

         int i = 1;
         foreach(var c in AttachedCaches)
         {
            finnalSqlSelect += string.Format("\nUNION SELECT Tile FROM db{0}.TilesData WHERE id = (SELECT id FROM db{0}.Tiles WHERE X={{0}} AND Y={{1}} AND Zoom={{2}} AND Type={{3}})", i);
            attachSqlQuery += string.Format("\nATTACH '{0}' as db{1};", c, i);
            detachSqlQuery += string.Format("\nDETACH DATABASE db{0};", i);

            i++;
         }
      }

      public void Attach(string db)
      {
         if(!AttachedCaches.Contains(db))
         {
            AttachedCaches.Add(db);
            RebuildFinnalSelect();
         }
      }

      public void Detach(string db)
      {
         if(AttachedCaches.Contains(db))
         {
            AttachedCaches.Remove(db);
            RebuildFinnalSelect();
         }
      }

      #region PureImageCache Members

      int preAllocationPing = 0;

      bool PureImageCache.PutImageToCache(byte[] tile, int type, GPoint pos, int zoom)
      {
         bool ret = true;
         if(Created)
         {
            try
            {
               using(SQLiteConnection cn = new SQLiteConnection())
               {
                  cn.ConnectionString = ConnectionString;
                  cn.Open();
                  {
                     using(DbTransaction tr = cn.BeginTransaction())
                     {
                        try
                        {
                           using(DbCommand cmd = cn.CreateCommand())
                           {
                              cmd.Transaction = tr;
                              cmd.CommandText = singleSqlInsert;

                              cmd.Parameters.Add(new SQLiteParameter("@p1", pos.X));
                              cmd.Parameters.Add(new SQLiteParameter("@p2", pos.Y));
                              cmd.Parameters.Add(new SQLiteParameter("@p3", zoom));
                              cmd.Parameters.Add(new SQLiteParameter("@p4", type));
                              cmd.Parameters.Add(new SQLiteParameter("@p5", DateTime.Now));

                              cmd.ExecuteNonQuery();
                           }

                           using(DbCommand cmd = cn.CreateCommand())
                           {
                              cmd.Transaction = tr;

                              cmd.CommandText = singleSqlInsertLast;
                              cmd.Parameters.Add(new SQLiteParameter("@p1", tile));

                              cmd.ExecuteNonQuery();
                           }
                           tr.Commit();
                        }
                        catch(Exception ex)
                        {
#if MONO
                           Console.WriteLine("PutImageToCache: " + ex.ToString());
#endif
                           Debug.WriteLine("PutImageToCache: " + ex.ToString());

                           tr.Rollback();
                           ret = false;
                        }
                     }
                  }
                  cn.Close();
               }

               if(Interlocked.Increment(ref preAllocationPing) % 22 == 0)
               {
                  CheckPreAllocation();
               }
            }
            catch(Exception ex)
            {
#if MONO
               Console.WriteLine("PutImageToCache: " + ex.ToString());
#endif
               Debug.WriteLine("PutImageToCache: " + ex.ToString());
               ret = false;
            }
         }
         return ret;
      }

      PureImage PureImageCache.GetImageFromCache(int type, GPoint pos, int zoom)
      {
         PureImage ret = null;
         try
         {
            using(SQLiteConnection cn = new SQLiteConnection())
            {
               cn.ConnectionString = ConnectionString;
               cn.Open();
               {
                  if(!string.IsNullOrEmpty(attachSqlQuery))
                  {
                     using(DbCommand com = cn.CreateCommand())
                     {
                        com.CommandText = attachSqlQuery;
                        int x = com.ExecuteNonQuery();
                        //Debug.WriteLine("Attach: " + x);                         
                     }
                  }

                  using(DbCommand com = cn.CreateCommand())
                  {
                     com.CommandText = string.Format(finnalSqlSelect, pos.X, pos.Y, zoom, type);

                     using(DbDataReader rd = com.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                     {
                        if(rd.Read())
                        {
                           long length = rd.GetBytes(0, 0, null, 0, 0);
                           byte[] tile = new byte[length];
                           rd.GetBytes(0, 0, tile, 0, tile.Length);
                           {
                              if(GMapProvider.TileImageProxy != null)
                              {
                                 ret = GMapProvider.TileImageProxy.FromArray(tile);
                              }
                           }
                           tile = null;
                        }
                        rd.Close();
                     }
                  }

                  if(!string.IsNullOrEmpty(detachSqlQuery))
                  {
                     using(DbCommand com = cn.CreateCommand())
                     {
                        com.CommandText = detachSqlQuery;
                        int x = com.ExecuteNonQuery();
                        //Debug.WriteLine("Detach: " + x);
                     }
                  }
               }
               cn.Close();
            }
         }
         catch(Exception ex)
         {
#if MONO
            Console.WriteLine("GetImageFromCache: " + ex.ToString());
#endif
            Debug.WriteLine("GetImageFromCache: " + ex.ToString());
            ret = null;
         }

         return ret;
      }

      int PureImageCache.DeleteOlderThan(DateTime date, int? type)
      {
         int affectedRows = 0;

         try
         {
            using(SQLiteConnection cn = new SQLiteConnection())
            {
               cn.ConnectionString = ConnectionString;
               cn.Open();
               {
                  using(DbCommand com = cn.CreateCommand())
                  {
                     com.CommandText = string.Format("DELETE FROM Tiles WHERE CacheTime is not NULL and CacheTime < datetime('{0}')", date.ToString("s"));
                     if(type.HasValue)
                     {
                        com.CommandText += " and Type = " + type;
                     }
                     affectedRows = com.ExecuteNonQuery();
                  }
               }
            }
         }
         catch(Exception ex)
         {
#if MONO
            Console.WriteLine("DeleteOlderThan: " + ex);
#endif
            Debug.WriteLine("DeleteOlderThan: " + ex);
         }

         return affectedRows;
      }

      #endregion
   }
#endif
}
