namespace Xamarin.Platform.Handlers
{
	public partial class BoxViewHandler
	{
		public static PropertyMapper<IBox, BoxViewHandler> BoxViewMapper = new PropertyMapper<IBox, BoxViewHandler>(ViewHandler.ViewMapper)
		{
			[nameof(IBox.Color)] = MapColor,
			[nameof(IBox.CornerRadius)] = MapCornerRadius
		};

		public static void MapColor(BoxViewHandler handler, IBox boxView)
		{
			ViewHandler.CheckParameters(handler, boxView);
			handler.TypedNativeView?.UpdateColor(boxView);
		}

		public static void MapCornerRadius(BoxViewHandler handler, IBox boxView)
		{
			ViewHandler.CheckParameters(handler, boxView);
			handler.TypedNativeView?.UpdateCornerRadius(boxView);
		}

		public BoxViewHandler() : base(BoxViewMapper)
		{

		}

		public BoxViewHandler(PropertyMapper mapper) : base(mapper ?? BoxViewMapper)
		{

		}
	}
}