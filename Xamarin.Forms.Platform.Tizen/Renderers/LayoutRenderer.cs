using System;

namespace Xamarin.Forms.Platform.Tizen
{
	/// <summary>
	/// Renderer of a Layout.
	/// </summary>
	public class LayoutRenderer : ViewRenderer<Layout, Native.Canvas>
	{
		bool _isLayoutUpdatedRegistered = false;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public LayoutRenderer()
		{
		}

		public void RegisterOnLayoutUpdated()
		{
			if (!_isLayoutUpdatedRegistered)
			{
				Control.LayoutUpdated += OnLayoutUpdated;
				_isLayoutUpdatedRegistered = true;
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Layout> e)
		{
			if (null == Control)
			{
				var canvas = new Native.Canvas(Forms.Context.MainWindow);
				SetNativeControl(canvas);
			}

			base.OnElementChanged(e);
		}

		protected override void Dispose(bool disposing)
		{
			if (_isLayoutUpdatedRegistered)
			{
				Control.LayoutUpdated -= OnLayoutUpdated;
			}

			base.Dispose(disposing);
		}

		void OnLayoutUpdated(object sender, Native.LayoutEventArgs e)
		{
			DoLayout(e);
		}
	}
}
