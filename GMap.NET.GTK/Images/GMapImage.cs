namespace GMap.NET.GTK.Images
{
    public class GMapImage : PureImage
    {
        public System.Drawing.Image Img;

        public override void Dispose()
        {
            if (Img != null)
            {
                Img.Dispose();
                Img = null;
            }

            if (Data != null)
            {
                Data.Dispose();
                Data = null;
            }
        }
    }
}