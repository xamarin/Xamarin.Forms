namespace Xamarin.Platform.Handlers
{
	public partial class BoxViewHandler : AbstractViewHandler<IBox, NativeBoxView>
	{
		protected override NativeBoxView CreateNativeView() => new NativeBoxView(Context);
	}
}