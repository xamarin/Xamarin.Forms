using System;

namespace Xamarin.Forms
{
    public class PathGeometryConverter : TypeConverter
    {
        public override object ConvertFromInvariantString(string value)
        {
            PathGeometry pathGeometry = new PathGeometry();

            PathFigureCollectionConverter.ParseStringToPathFigureCollection(pathGeometry.Figures, value);

            return pathGeometry;
        }
    }
}