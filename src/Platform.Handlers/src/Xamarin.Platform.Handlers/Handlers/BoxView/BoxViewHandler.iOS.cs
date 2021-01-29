using Xamarin.Forms;

namespace Xamarin.Platform.Handlers
{
	public partial class BoxViewHandler : AbstractViewHandler<IBox, NativeBoxView>
	{
		protected override NativeBoxView CreateNativeView() => new NativeBoxView();

		public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			if (TypedNativeView != null)
				TypedNativeView.Size = new Size(widthConstraint, heightConstraint);

			return base.GetDesiredSize(widthConstraint, heightConstraint);
		}
	}
}