using CoreGraphics;

namespace Xamarin.Forms.Platform.iOS
{
    public static class PointCollectionExtensions
    {
        public static CGPoint[] ToCGPoints(this PointCollection pointCollection)
        {
            if (pointCollection == null || pointCollection.Count == 0)
            {
                return new CGPoint[0];
            }

            CGPoint[] points = new CGPoint[pointCollection.Count];
            Point[] array = new Point[pointCollection.Count];
            pointCollection.CopyTo(array, 0);

            for (int i = 0; i < array.Length; i++)
            {
                points[i] = new CGPoint(array[i].X, array[i].Y);
            }

            return points;
        }
    }
}