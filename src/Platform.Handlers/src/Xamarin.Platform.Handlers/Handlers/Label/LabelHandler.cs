namespace Xamarin.Platform.Handlers
{
	public partial class LabelHandler
	{
		public static PropertyMapper<ILabel, LabelHandler> LabelMapper = new PropertyMapper<ILabel, LabelHandler>(ViewHandler.ViewMapper)
		{
			[nameof(ILabel.Color)] = MapColor,
			[nameof(ILabel.Text)] = MapText,
		};

		public static void MapColor(LabelHandler handler, ILabel Label)
		{
		}

		public static void MapText(LabelHandler handler, ILabel label)
		{
			handler.TypedNativeView?.UpdateText(label);
		}
#if MONOANDROID
		protected override NativeView CreateNativeView() => new NativeView(this.Context);
#else
		protected override NativeView CreateNativeView() => new NativeView();
#endif

		public LabelHandler() : base(LabelMapper)
		{

		}

		public LabelHandler(PropertyMapper mapper) : base(mapper ?? LabelMapper)
		{

		}
	}
}