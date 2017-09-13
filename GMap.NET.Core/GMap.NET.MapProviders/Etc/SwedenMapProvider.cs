
namespace GMap.NET.MapProviders
{
   using System;
   using GMap.NET.Projections;

   public abstract class SwedenMapProviderBase : GMapProvider
   {
      public SwedenMapProviderBase()
      {
         RefererUrl = "https://kso.etjanster.lantmateriet.se/?lang=en";
         Copyright = string.Format("©{0} Lantmäteriet", DateTime.Today.Year);
         MaxZoom = 11;
         //Area = new RectLatLng(58.0794870805093, 20.3286067123543, 7.90883164336887, 2.506129113082);
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
            return SWEREF99_TMProjection.Instance;
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
   }

    /// <summary>
    /// SwedenMap provider, https://kso.etjanster.lantmateriet.se/?lang=en#
    /// </summary>
    public class SwedenMapProvider : SwedenMapProviderBase
    {
      public static readonly SwedenMapProvider Instance;

      SwedenMapProvider()
      {
      }

      static SwedenMapProvider()
      {
         Instance = new SwedenMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("40890A96-6E82-4FA7-90A3-73D66B974F63");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "SwedenMap";
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
            // https://kso.etjanster.lantmateriet.se/karta/topowebb/v1/wmts?SERVICE=WMTS&REQUEST=GetTile&VERSION=1.0.0&LAYER=topowebb&STYLE=default&TILEMATRIXSET=3006&TILEMATRIX=2&TILEROW=6&TILECOL=7&FORMAT=image%2Fpng

            return string.Format(UrlFormat, zoom, pos.Y, pos.X);
      }

      static readonly string UrlFormat = "http://kso.etjanster.lantmateriet.se/karta/topowebb/v1/wmts?SERVICE=WMTS&REQUEST=GetTile&VERSION=1.0.0&LAYER=topowebb&STYLE=default&TILEMATRIXSET=3006&TILEMATRIX={0}&TILEROW={1}&TILECOL={2}&FORMAT=image%2Fpng";
   }
}

/*
https://kso.etjanster.lantmateriet.se/?lang=en#

{
	"analytics" : {
		"account" : "UA-47513746-2"
	},
	"ortofotoServiceUrl" : "https://services-ver.lantmateriet.se/distribution/produkter/metabild/v1/ortofoto",
	"metadata" : {
		"password" : "wllZIV50DH2b",
		"username" : "lant0181"
	},
	"minFastighetUrl" : "https://etjanster.lantmateriet.se/nyaminfastighet/?mode=TEXT&amp;module=sercxi-minafastigheter-module",
	"enableKartvaljare" : false,
	"frwebburl" : "https://etjanster.lantmateriet.se/frwebb/protect/index.jsp?information=FRW019&query=",
	"previewServiceUrl" : "https://services-ver.lantmateriet.se/distribution/produkter/tumnagel/v1/",
	"minaFastighetsArendenUrl" : "https://etjanster.lantmateriet.se/minafastighetsarenden",
	"flygbildServiceUrl" : "https://services-ver.lantmateriet.se/distribution/produkter/metabild/v1/flygbild"
}

PROJCS["SWEREF99 TM",
    GEOGCS["SWEREF99",
        DATUM["SWEREF99",
            SPHEROID["GRS 1980",6378137,298.257222101,
                AUTHORITY["EPSG","7019"]],
            TOWGS84[0,0,0,0,0,0,0],
            AUTHORITY["EPSG","6619"]],
        PRIMEM["Greenwich",0,
            AUTHORITY["EPSG","8901"]],
        UNIT["degree",0.01745329251994328,
            AUTHORITY["EPSG","9122"]],
        AUTHORITY["EPSG","4619"]],
    UNIT["metre",1,
        AUTHORITY["EPSG","9001"]],
    PROJECTION["Transverse_Mercator"],
    PARAMETER["latitude_of_origin",0],
    PARAMETER["central_meridian",15],
    PARAMETER["scale_factor",0.9996],
    PARAMETER["false_easting",500000],
    PARAMETER["false_northing",0],
    AUTHORITY["EPSG","3006"],
    AXIS["y",EAST],
    AXIS["x",NORTH]]

{
	"defaultLayer" : "topowebbwmts",
	"extent" : {
		"left" : -1200000,
		"bottom" : 4700000,
		"right" : 2600000,
		"top" : 8500000
	},
	"projection" : "EPSG:3006",
	"units" : "m",
	"allOverlays" : true,
	"resolutions" : [4096.0, 2048.0, 1024.0, 512.0, 256.0, 128.0, 64.0, 32.0, 16.0, 8.0, 4.0, 2.0, 1.0, 0.5, 0.25, 0.15, 0.1, 0.05, 0.01],
	"initPosition" : {
		"n" : 6607899,
		"e" : 564931,
		"zoom" : 2
	},
	"layers" : [{
			"id" : "topowebbhydrografi",
			"protocol" : "WMS",
			"url" : ["https://kso.etjanster.lantmateriet.se/karta/topowebb-skikt/wms/v1.1"],
			"exceptions" : "application/vnd.ogc.se_xml",
			"format" : "image/png",
			"layers" : "hydrografi",
			"name" : "Hydrografi",
			"fullmap" : "false",
			"enabled" : "true",
			"sheetFamily" : "",
			"maxResolution" : 0.0,
			"minResolution" : 0.0,
			"visible" : "true",
			"tileSize" : 512,
			"maxScale" : 0,
			"minScale" : 0,
			"removeLayer" : "false",
			"styles" : []
		}, {
			"id" : "orto",
			"protocol" : "WMS",
			"url" : ["https://kso.etjanster.lantmateriet.se/karta/ortofoto/wms/v1"],
			"exceptions" : "application/vnd.ogc.se_xml",
			"format" : "image/png",
			"layers" : "orto",
			"name" : "Ortofoton 0.5 m/pixel",
			"fullmap" : "true",
			"enabled" : "true",
			"sheetFamily" : "",
			"maxResolution" : 0.0,
			"minResolution" : 0.0,
			"visible" : "true",
			"tileSize" : 512,
			"maxScale" : 0,
			"minScale" : 0,
			"removeLayer" : "false",
			"styles" : []
		}, {
			"id" : "topowebbwmts",
			"protocol" : "WMTS",
			"url" : ["https://kso.etjanster.lantmateriet.se/karta/topowebb/v1/wmts"],
			"exceptions" : "application/vnd.ogc.se_xml",
			"format" : "image/png",
			"layers" : "topowebb",
			"name" : "Topografisk webbkarta (cache)",
			"fullmap" : "true",
			"enabled" : "true",
			"sheetFamily" : "",
			"maxResolution" : 4096.0,
			"minResolution" : 0.5,
			"visible" : "false",
			"tileSize" : 512,
			"maxScale" : 0,
			"minScale" : 0,
			"removeLayer" : "false",
			"styles" : []
		}, {
			"id" : "topowebbwms",
			"protocol" : "WMS",
			"url" : ["https://kso.etjanster.lantmateriet.se/karta/topowebb/wms/v1"],
			"exceptions" : "application/vnd.ogc.se_xml",
			"format" : "image/png",
			"layers" : "topowebbkartan",
			"name" : "Topografisk webbkarta",
			"fullmap" : "true",
			"enabled" : "true",
			"sheetFamily" : "",
			"maxResolution" : 0.0,
			"minResolution" : 0.0,
			"visible" : "true",
			"tileSize" : 512,
			"maxScale" : 0,
			"minScale" : 0,
			"removeLayer" : "false",
			"styles" : []
		}, {
			"id" : "terrangskuggning",
			"protocol" : "WMS",
			"url" : ["https://kso.etjanster.lantmateriet.se/karta/hojdmodell/wms/v1"],
			"exceptions" : "application/vnd.ogc.se_xml",
			"format" : "image/png",
			"layers" : "terrangskuggning",
			"name" : "Terrängskuggning",
			"fullmap" : "false",
			"enabled" : "true",
			"sheetFamily" : "",
			"maxResolution" : 0.0,
			"minResolution" : 0.0,
			"visible" : "false",
			"tileSize" : 2048,
			"maxScale" : 0,
			"minScale" : 0,
			"removeLayer" : "false",
			"styles" : []
		}, {
			"id" : "terrangkartan",
			"protocol" : "WMS",
			"url" : ["https://kso.etjanster.lantmateriet.se/karta/allmannakartor/wms/v1"],
			"exceptions" : "application/vnd.ogc.se_xml",
			"format" : "image/png",
			"layers" : "terrangkartan",
			"name" : "Terrängkartan",
			"fullmap" : "true",
			"enabled" : "true",
			"sheetFamily" : "SE50",
			"maxResolution" : 0.0,
			"minResolution" : 0.0,
			"visible" : "false",
			"tileSize" : 512,
			"maxScale" : 0,
			"minScale" : 0,
			"removeLayer" : "false",
			"styles" : []
		}, {
			"id" : "fjallkartan",
			"protocol" : "WMS",
			"url" : ["https://kso.etjanster.lantmateriet.se/karta/allmannakartor/wms/v1"],
			"exceptions" : "application/vnd.ogc.se_xml",
			"format" : "image/png",
			"layers" : "fjallkartan",
			"name" : "Fjällkartan 1:100 000",
			"fullmap" : "false",
			"enabled" : "true",
			"sheetFamily" : "SE100FJ",
			"maxResolution" : 0.0,
			"minResolution" : 0.0,
			"visible" : "false",
			"tileSize" : 512,
			"maxScale" : 0,
			"minScale" : 0,
			"removeLayer" : "false",
			"styles" : []
		}, {
			"id" : "vagkartan",
			"protocol" : "WMS",
			"url" : ["https://kso.etjanster.lantmateriet.se/karta/allmannakartor/wms/v1"],
			"exceptions" : "application/vnd.ogc.se_xml",
			"format" : "image/png",
			"layers" : "vagkartan",
			"name" : "Vägkartan",
			"fullmap" : "true",
			"enabled" : "true",
			"sheetFamily" : "",
			"maxResolution" : 0.0,
			"minResolution" : 0.0,
			"visible" : "false",
			"tileSize" : 512,
			"maxScale" : 0,
			"minScale" : 0,
			"removeLayer" : "false",
			"styles" : []
		}, {
			"id" : "oversiktskartan",
			"protocol" : "WMS",
			"url" : ["https://kso.etjanster.lantmateriet.se/karta/geodata-intern/wms/v1"],
			"exceptions" : "application/vnd.ogc.se_xml",
			"format" : "image/png",
			"layers" : "oversiktskartan",
			"name" : "Översiktskartan",
			"fullmap" : "true",
			"enabled" : "true",
			"sheetFamily" : "SE250",
			"maxResolution" : 0.0,
			"minResolution" : 0.0,
			"visible" : "false",
			"tileSize" : 512,
			"maxScale" : 0,
			"minScale" : 0,
			"removeLayer" : "false",
			"styles" : []
		}, {
			"id" : "sverigekartan",
			"protocol" : "WMS",
			"url" : ["https://kso.etjanster.lantmateriet.se/karta/allmannakartor/wms/v1"],
			"exceptions" : "application/vnd.ogc.se_xml",
			"format" : "image/png",
			"layers" : "sverigekartan",
			"name" : "Sverigekartan",
			"fullmap" : "true",
			"enabled" : "true",
			"sheetFamily" : "",
			"maxResolution" : 0.0,
			"minResolution" : 0.0,
			"visible" : "false",
			"tileSize" : 512,
			"maxScale" : 0,
			"minScale" : 0,
			"removeLayer" : "false",
			"styles" : []
		}
	],
	"profiles" : "[]",
	"touchProfiles" : "[]",
	"noAuthProfiles" : "[{'value':'default_background_noauth','text':'Topografisk','optgroup':'Standard'},{'value':'default_orto_noauth','text':'Ortofoto','optgroup':'Standard'},{'value':'default_terrangkartan_noauth','text':'Terrängkartan','optgroup':'Standard'},{'value':'default_fjallkartan_noauth','text':'Fjällkartan','optgroup':'Standard'},{'value':'default_vagkartan_noauth','text':'Vägkartan','optgroup':'Standard'},{'value':'default_sverigekartan_noauth','text':'Sverigekartan','optgroup':'Standard'},{'value':'default_terrangskuggning_noauth','text':'Terrängskuggning','optgroup':'Standard'}]",
	"profileDescriptions" : {
		"default_background" : "{'layers':[{'id':'topowebbwmts','index':0,'group':'','enabled':true,'visible':true,'style':'default','opacity':1.0}],'version':4}",
		"default_terrangkartan_noauth" : "{'layers':[{'id':'terrangkartan','index':0,'group':'','enabled':true,'visible':true,'style':'default','opacity':1.0}],'version':4}",
		"default_vagkartan_noauth" : "{'layers':[{'id':'vagkartan','index':0,'group':'','enabled':true,'visible':true,'style':'default','opacity':1.0}],'version':4}",
		"default_orto_noauth" : "{'layers':[{'id':'orto025','index':0,'group':'','enabled':true,'visible':true,'style':'default','opacity':1.0},{'id':'orto','index':1,'group':'','enabled':true,'visible':true,'style':'default','opacity':1.0}],'version':4}",
		"default_fjallkartan_noauth" : "{'layers':[{'id':'fjallkartan','index':0,'group':'','enabled':true,'visible':true,'style':'default','opacity':1.0}],'version':4}",
		"default_sverigekartan_noauth" : "{'layers':[{'id':'sverigekartan','index':0,'group':'','enabled':true,'visible':true,'style':'default','opacity':1.0}],'version':4}",
		"default_terrangskuggning_noauth" : "{'layers':[{'id':'topowebbhydrografi','index':0,'group':'','enabled':true,'visible':true,'style':'default','opacity':1.0},{'id':'terrangskuggning','index':1,'group':'','enabled':true,'visible':true,'style':'default','opacity':1.0}],'version':4}",
		"default_background_noauth" : "{'layers':[{'id':'topowebbwmts','index':0,'group':'','enabled':true,'visible':true,'style':'default','opacity':1.0}],'version':4}"
	}
}
*/
