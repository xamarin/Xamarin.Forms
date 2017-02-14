using ElmSharp;

namespace Xamarin.Forms.Platform.Tizen
{
	/// <summary>
	/// Base class for view renderers.
	/// </summary>
	public abstract class ViewRenderer<TView, TNativeView> : VisualElementRenderer<TView>
		where TView : View
		where TNativeView : Widget
	{
		GestureDetector _gestureDetector;

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected ViewRenderer()
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<TView> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				_gestureDetector.Clear();
				_gestureDetector = null;
			}

			if (e.NewElement != null)
			{
				_gestureDetector = new GestureDetector(this);
			}
		}

		/// <summary>
		/// Native control associated with this renderer.
		/// </summary>
		public TNativeView Control
		{
			get
			{
				return (TNativeView)NativeView;
			}
		}

		protected override void ApplyTransformation()
		{
			base.ApplyTransformation();
			_gestureDetector?.UpdateHitBox();
		}
	}
}