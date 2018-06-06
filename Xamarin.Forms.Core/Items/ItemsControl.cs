using System.ComponentModel;

namespace Xamarin.Forms
{
	// TODO hartez 2018/05/31 11:49:49 Move the stuff in ItemsControl.cs to the appropriate files
	public interface IItemsLayout : INotifyPropertyChanged {}

	public enum ItemsLayoutOrientation
	{
		Vertical,
		Horizontal
	}

	public abstract class ItemsLayout : BindableObject, IItemsLayout
	{
		public ItemsLayoutOrientation Orientation { get; }

		protected ItemsLayout(ItemsLayoutOrientation orientation)
		{
			Orientation = orientation;
		}
	}

	public class ListItemsLayout : ItemsLayout
	{
		public ListItemsLayout(ItemsLayoutOrientation orientation) : base(orientation)
		{
		}

		public static readonly BindableProperty SnapPointsAlignmentProperty;
		public SnapPointsAlignment SnapPointsAlignment { get; set; }

		public static readonly BindableProperty SnapPointsTypeProperty;
		public SnapPointsType SnapPointsType { get; set; }

		// TODO hartez 2018/05/31 15:56:23 Should these just be called Vertical and Horizontal (without List)?	
		public static readonly IItemsLayout VerticalList = new ListItemsLayout(ItemsLayoutOrientation.Vertical); 
		public static readonly IItemsLayout HorizontalList = new ListItemsLayout(ItemsLayoutOrientation.Horizontal); 
	}

	public class GridItemsLayout : ItemsLayout
	{
		public static readonly BindableProperty SpanProperty =
			BindableProperty.Create(nameof(Span), typeof(int), typeof(GridItemsLayout), 1);

		public int Span
		{
			get => (int)GetValue(SpanProperty);
			set => SetValue(SpanProperty, value);
		}

		public GridItemsLayout([Parameter("Span")] int span, [Parameter("Orientation")] ItemsLayoutOrientation orientation) :
			base(orientation)
		{
			Span = span;
		}
	}

	public enum SnapPointsAlignment
	{
		Start,
		Center,
		End
	}

	public enum SnapPointsType
	{
		None,
		Optional,
		Mandatory,
		OptionalSingle,
		MandatorySingle,
	}
}
