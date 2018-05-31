namespace Xamarin.Forms
{
	// TODO hartez 2018/05/31 11:49:49 Move the stuff in ItemsControl.cs to the appropriate files
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

		// TODO hartez 2018/05/31 15:56:23 Should these just be called Vertical and Horizontal (without List)?	
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
