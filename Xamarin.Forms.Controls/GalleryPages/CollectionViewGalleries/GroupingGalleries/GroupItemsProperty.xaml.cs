using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.GroupingGalleries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GroupItemsProperty : ContentPage
	{
		public GroupItemsProperty()
		{
			InitializeComponent();

			CollectionView.ItemsSource = new Cities();
		}
	}

	[Preserve(AllMembers = true)]
	internal class PropertySelection : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var on = (bool)value;

			if (on)
			{
				return "Teams";
			}

			return "Heroes";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}