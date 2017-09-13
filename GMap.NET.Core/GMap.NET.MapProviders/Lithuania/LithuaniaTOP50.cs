
namespace GMap.NET.MapProviders
{
    using System;
    using GMap.NET.Projections;

    public class LithuaniaTOP50 : GMapProvider
    {
        public static readonly LithuaniaTOP50 Instance;

        LithuaniaTOP50()
        {
            MaxZoom = 15;
        }

        static LithuaniaTOP50()
        {
            Instance = new LithuaniaTOP50();
        }

        #region GMapProvider Members

        Guid id = new Guid("2920B1AF-6D57-4895-9A21-D5837CBF1049");
        public override Guid Id
        {
            get
            {
                return id;
            }
        }

        public override string Name
        {
            get
            {
                return "LithuaniaTOP50";
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
                if (overlays == null)
                {
                    overlays = new GMapProvider[] { this };
                }
                return overlays;
            }
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            return null;
        }
        #endregion
    }
}