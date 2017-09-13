using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.IO;
using GMap.NET.MapProviders;

namespace GMap.NET.Internals
{
    internal class TileHttpHost
    {
        volatile bool listen = false;
        TcpListener server;
        int port;

        readonly byte[] responseHeaderBytes;

        public TileHttpHost()
        {
            string response = "HTTP/1.0 200 OK\r\nContent-Type: image\r\nConnection: close\r\n\r\n";
            responseHeaderBytes = Encoding.ASCII.GetBytes(response);
        }

        public void Stop()
        {
            if (listen)
            {
                listen = false;
                if (server != null)
                {
                    server.Stop();
                }
            }
        }

        public void Start(int port)
        {
            if (server == null)
            {
                this.port = port;
                server = new TcpListener(IPAddress.Any, port);
            }
            else
            {
                if (this.port != port)
                {
                    Stop();
                    this.port = port;
                    server = null;
                    server = new TcpListener(IPAddress.Any, port);
                }
                else
                {
                    if (listen)
                    {
                        return;
                    }
                }
            }

            server.Start();
            listen = true;

            Thread t = new Thread(() =>
            {
                while (listen)
                {
                    try
                    {   
                        if (!server.Pending())
                        {
                            Thread.Sleep(111);
                        }
                        else
                        {
                            ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessRequest), server.AcceptTcpClient());
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("TileHttpHost: " + ex);
                    }
                }
            });

            t.Name = "TileHost";
            t.IsBackground = true;
            t.Start();
        }

        void ProcessRequest(object p)
        {
          try
          {
            using(TcpClient c = p as TcpClient)
            {
                using(var s = c.GetStream())
                {
                    using(StreamReader r = new StreamReader(s, Encoding.UTF8))
                    {              
                        var request = r.ReadLine();                          

                        if(!string.IsNullOrEmpty(request) && request.StartsWith("GET"))
                        {
                            //Debug.WriteLine("TileHttpHost: " + request);

                            // http://localhost:88/88888/5/15/11
                            // GET /8888888888/5/15/11 HTTP/1.1

                            var rq = request.Split(' ');
                            if(rq.Length >= 2)
                            {
                                var ids = rq[1].Split(new char[]{'/'}, StringSplitOptions.RemoveEmptyEntries);
                                if(ids.Length == 4)
                                {
                                    int dbId = int.Parse(ids[0]);
                                    int zoom = int.Parse(ids[1]);
                                    int x = int.Parse(ids[2]);
                                    int y = int.Parse(ids[3]);

                                    var pr = GMapProviders.TryGetProvider(dbId);
                                    if(pr != null)
                                    {
                                        Exception ex;
                                        var img = GMaps.Instance.GetImageFrom(pr, new GPoint(x, y), zoom, out ex);
                                        if (img != null)
                                        {
                                            using (img)
                                            {
                                                s.Write(responseHeaderBytes, 0, responseHeaderBytes.Length);
                                                img.Data.WriteTo(s);
                                            }
                                        }
                                    }
                                } 
                            }         
                        }
                    }
                }
                c.Close();
            }
          }
          catch (Exception ex)
          {
              Debug.WriteLine("TileHttpHost, ProcessRequest: " + ex);
          }
        }
    }
}
