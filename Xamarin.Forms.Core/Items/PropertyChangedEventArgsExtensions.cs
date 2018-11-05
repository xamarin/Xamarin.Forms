using System.ComponentModel;

namespace Xamarin.Forms
{
	// TODO hartez 2018/06/23 13:42:22 Trying this out for a nicer read in OnElementPropertyChanged, not sure if I like it yet	
	public static class PropertyChangedEventArgsExtensions
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool Is(this PropertyChangedEventArgs args, BindableProperty property)
		{
			CollectionView.VerifyCollectionViewFlagEnabled();
			return args.PropertyName == property.PropertyName;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsOneOf(this PropertyChangedEventArgs args, params BindableProperty[] properties)
		{
			CollectionView.VerifyCollectionViewFlagEnabled();
			foreach (BindableProperty property in properties)
			{
				if (args.PropertyName == property.PropertyName)
				{
					return true;
				}
			}

			return false;
		}
	}
}