namespace GMap.NET.GTK.Images
{
    using System.Drawing;
    using System.IO;
    using System.Drawing.Imaging;
    using System;
    using System.Diagnostics;
    using MapProviders;
    using Internals;

    public class GMapImageProxy : PureImageProxy
    {
        GMapImageProxy()
        {

        }

        public static void Enable()
        {
            GMapProvider.TileImageProxy = Instance;
        }

        public static readonly GMapImageProxy Instance = new GMapImageProxy();

        internal ColorMatrix ColorMatrix;

        static readonly bool Win7OrLater = Stuff.IsRunningOnWin7orLater();

        public override PureImage FromStream(Stream stream)
        {
            GMapImage ret = null;
            try
            {
                Image m = Image.FromStream(stream, true, Win7OrLater ? false : true);

                if (m != null)
                {
                    ret = new GMapImage();
                    ret.Img = ColorMatrix != null ? ApplyColorMatrix(m, ColorMatrix) : m;
                }

            }
            catch (Exception ex)
            {
                ret = null;
            }

            return ret;
        }

        public override bool Save(Stream stream, PureImage image)
        {
            GMapImage ret = image as GMapImage;
            bool ok = true;

            if (ret.Img != null)
            {
                // Png
                try
                {
                    ret.Img.Save(stream, ImageFormat.Png);
                }
                catch
                {
                    // Jpeg
                    try
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        ret.Img.Save(stream, ImageFormat.Jpeg);
                    }
                    catch
                    {
                        ok = false;
                    }
                }
            }
            else
            {
                ok = false;
            }

            return ok;
        }

        private Bitmap ApplyColorMatrix(Image original, ColorMatrix matrix)
        {
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            using (original) 
            {
                using (Graphics g = Graphics.FromImage(newBitmap))
                {
                    using (ImageAttributes attributes = new ImageAttributes())
                    {
                        attributes.SetColorMatrix(matrix);
                        g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
                    }
                }
            }

            return newBitmap;
        }
    }
}