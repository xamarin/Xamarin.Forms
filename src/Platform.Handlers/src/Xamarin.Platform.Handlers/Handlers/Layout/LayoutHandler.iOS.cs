using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Platform.Handlers
{
	public partial class LayoutHandler : AbstractViewHandler<ILayout, LayoutView>
	{
		protected override LayoutView CreateView()
		{
			var view = new LayoutView
			{
				CrossPlatformMeasure = VirtualView.Measure,
				CrossPlatformArrange = VirtualView.Arrange,
			};

			return view;
		}

		public override void SetView(IView view)
		{
			base.SetView(view);

			TypedNativeView.CrossPlatformMeasure = VirtualView.Measure;
			TypedNativeView.CrossPlatformArrange = VirtualView.Arrange;

			foreach (var child in VirtualView.Children)
			{
				TypedNativeView.AddSubview(child.ToNative());
			}
		}

		protected override void DisposeView(LayoutView nativeView)
		{
			nativeView.CrossPlatformArrange = null;
			nativeView.CrossPlatformMeasure = null;

			foreach (var subview in nativeView.Subviews)
			{
				subview.RemoveFromSuperview();
			}

			base.DisposeView(nativeView);
		}
	}
}
