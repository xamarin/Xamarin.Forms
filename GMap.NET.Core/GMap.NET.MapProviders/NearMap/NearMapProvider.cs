
namespace GMap.NET.MapProviders
{
   using System;
   using GMap.NET.Projections;
   using System.Net;
   using System.Text;

   /// <summary>
   /// http://en.wikipedia.org/wiki/NearMap
   /// nearmap originally allowed personal use of images for free for non-enterprise users.
   /// However this free access ended in December 2012, when the company modified its business model to user-pay
   /// </summary>
   public abstract class NearMapProviderBase : GMapProvider
   {
      public NearMapProviderBase()
      {
         // credentials doesn't work ;/
         //Credential = new NetworkCredential("greatmaps", "greatmaps");
         
         //try ForceBasicHttpAuthentication(...);
      }     

      #region GMapProvider Members
      public override Guid Id
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public override string Name
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public override PureProjection Projection
      {
         get
         {
            return MercatorProjection.Instance;
         }
      }

      GMapProvider[] overlays;
      public override GMapProvider[] Overlays
      {
         get
         {
            if(overlays == null)
            {
               overlays = new GMapProvider[] { this };
            }
            return overlays;
         }
      }

      public override PureImage GetTileImage(GPoint pos, int zoom)
      {
         throw new NotImplementedException();
      }
      #endregion

      public new static int GetServerNum(GPoint pos, int max)
      {
         // var hostNum=((opts.nodes!==0)?((tileCoords.x&2)%opts.nodes):0)+opts.nodeStart;
         return (int)(pos.X & 2) % max;
      }

      static readonly string SecureStr = "Vk52edzNRYKbGjF8Ur0WhmQlZs4wgipDETyL1oOMXIAvqtxJBuf7H36acCnS9P";

      public string GetSafeString(GPoint pos)
      {
         #region -- source --
         /*
         TileLayer.prototype.differenceEngine=function(s,a)
         {
             var offset=0,result="",alen=a.length,v,p;
             for(var i=0; i<alen; i++)
             {
                 v=parseInt(a.charAt(i),10);
                 if(!isNaN(v))
                 {
                     offset+=v;
                     p=s.charAt(offset%s.length);
                     result+=p
                 }             
             }
             return result
         };    
       
         TileLayer.prototype.getSafeString=function(x,y,nmd)
         {
              var arg=x.toString()+y.toString()+((3*x)+y).toString();
              if(nmd)
              {
                 arg+=nmd
              }
              return this.differenceEngine(TileLayer._substring,arg)
         };  
        */
         #endregion

         var arg = pos.X.ToString() + pos.Y.ToString() + ((3 * pos.X) + pos.Y).ToString();

         string ret = "&s=";
         int offset = 0;
         for(int i = 0; i < arg.Length; i++)
         {
            offset += int.Parse(arg[i].ToString());
            ret += SecureStr[offset % SecureStr.Length];
         }

         return ret;
      }
   }

   /// <summary>
   /// NearMap provider - http://www.nearmap.com/
   /// </summary>
   public class NearMapProvider : NearMapProviderBase
   {
      public static readonly NearMapProvider Instance;

      NearMapProvider()
      {
         RefererUrl = "http://www.nearmap.com/";
      }

      static NearMapProvider()
      {
         Instance = new NearMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("E33803DF-22CB-4FFA-B8E3-15383ED9969D");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "NearMap";
      public override string Name
      {
         get
         {
            return name;
         }
      }

      public override PureImage GetTileImage(GPoint pos, int zoom)
      {
         string url = MakeTileImageUrl(pos, zoom, LanguageStr);

         return GetTileImageUsingHttp(url);
      }

      #endregion

      string MakeTileImageUrl(GPoint pos, int zoom, string language)
      {
         // http://web1.nearmap.com/maps/hl=en&x=18681&y=10415&z=15&nml=Map_&nmg=1&s=kY8lZssipLIJ7c5
         // http://web1.nearmap.com/kh/v=nm&hl=en&x=20&y=8&z=5&nml=Map_&s=55KUZ
        
         //http://maps.au.nearmap.com/api/0/authentication/checkticket?nmf=json&return200=true&callback=jQuery110206126813754172529_1431499242400&_=1431499242401
         //jQuery110206126813754172529_1431499242400({"IncidentId":null,"AccountContext":{"AccountId":"account2574457","Username":"demo"},"Result":{"Ticket":{"Ticket":"1637726D061CB8B8A28BC98064443C96FB07008C16531B2F5100F98B9EBFE69C4083C88C920D3BF4C0768A27ADE9ECADF324A380DBF80C3C0982DC83374FE8EBF0F70868735351FC7","Expires":"\/Date(1432103400000)\/","CookieName":"nearmap_web3_app"},"Status":"Ok","Username":"demo","AccountId":"account2574457"}})
         
         // http://web1.au.nearmap.com/maps/hl=en&x=1855837&y=1265913&z=21&nml=V&httpauth=false&version=2
         // http://web2.au.nearmap.com/maps/hl=en&x=231977&y=158238&z=18&nml=Dem&httpauth=false&version=2
         
         return string.Format(UrlFormat, GetServerNum(pos, 3), pos.X, pos.Y, zoom, GetSafeString(pos));
      }

      static readonly string UrlFormat = "http://web{0}.nearmap.com/kh/v=nm&hl=en&x={1}&y={2}&z={3}&nml=Map_{4}";
   }
}