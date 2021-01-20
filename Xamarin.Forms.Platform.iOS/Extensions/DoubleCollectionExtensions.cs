#if __MOBILE__
namespace Xamarin.Forms.Platform.iOS
#else
namespace Xamarin.Forms.Platform.MacOS
#endif
{
    public static class DoubleCollectionExtensions
    {
        public static double[] ToArray(this DoubleCollection doubleCollection)
        {
            if (doubleCollection == null || doubleCollection.Count == 0)
                return new double[0];
            else
            {
                double[] array;

                if (doubleCollection.Count % 2 == 0)
                {
                    array = new double[doubleCollection.Count];
                    doubleCollection.CopyTo(array, 0);
                }
                else
                {
                    array = new double[2 * doubleCollection.Count];
                    doubleCollection.CopyTo(array, 0);
                    doubleCollection.CopyTo(array, doubleCollection.Count);
                }

                return array;
            }
        }
    }
}