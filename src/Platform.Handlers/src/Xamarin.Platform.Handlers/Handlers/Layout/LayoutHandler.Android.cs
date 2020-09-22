using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Platform.Handlers
{
	public partial class LayoutHandler : AbstractViewHandler<ILayout, LayoutViewGroup>
	{
		protected override LayoutViewGroup CreateView()
		{
			var viewGroup = new LayoutViewGroup(Context)
			{
				CrossPlatformMeasure = VirtualView.Measure,
				CrossPlatformArrange = VirtualView.Arrange
			};

			return viewGroup;
		}

		public override void SetView(IView view)
		{
			base.SetView(view);

			TypedNativeView.CrossPlatformMeasure = VirtualView.Measure;
			TypedNativeView.CrossPlatformArrange = VirtualView.Arrange;

			foreach (var child in VirtualView.Children)
			{
				TypedNativeView.AddView(child.ToNative(Context));
			}
		}

		protected override void DisposeView(LayoutViewGroup nativeView)
		{
			nativeView.CrossPlatformArrange = null;
			nativeView.CrossPlatformMeasure = null;

			nativeView.RemoveAllViews();

			base.DisposeView(nativeView);
		}
	}
}
