using System.Collections.ObjectModel;

namespace Xamarin.Forms
{
	[TypeConverter(typeof(PointCollectionConverter))]
    public sealed class PointCollection : ObservableCollection<Point>
    {

    }
}