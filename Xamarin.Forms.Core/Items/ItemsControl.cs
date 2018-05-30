namespace Xamarin.Forms
{
	public interface IItemsLayout {}

	public enum ItemsLayoutOrientation
	{
		Vertical,
		Horizontal
	}

	public abstract class ItemsLayout : IItemsLayout
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

		public static readonly IItemsLayout VerticalList = new ListItemsLayout(ItemsLayoutOrientation.Vertical); 
		public static readonly IItemsLayout HorizontalList = new ListItemsLayout(ItemsLayoutOrientation.Horizontal); 
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
