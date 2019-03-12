using System;
using System.ComponentModel;
using Android.Content;
using Android.Support.V4.Content;
using AColor = Android.Graphics.Color;
using AColorRes = Android.Resource.Color;
using Android.Views;

namespace Xamarin.Forms.Platform.Android
{
	public class PageRenderer : VisualElementRenderer<Page>
	{
		public PageRenderer(Context context) : base(context)
		{
		}

		[Obsolete("This constructor is obsolete as of version 2.5. Please use PageRenderer(Context) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PageRenderer()
		{
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			base.OnTouchEvent(e);

			return true;
		}

		IPageController PageController => Element as IPageController;

		double _previousHeight;

		protected override void Dispose(bool disposing)
		{
			PageController?.SendDisappearing();
			base.Dispose(disposing);
		}

		protected override void OnAttachedToWindow()
		{
			base.OnAttachedToWindow();
			var pageContainer = Parent as PageContainer;
			if (pageContainer != null && (pageContainer.IsInFragment || pageContainer.Visibility == ViewStates.Gone))
				return;
			PageController.SendAppearing();
		}

		protected override void OnDetachedFromWindow()
		{
			base.OnDetachedFromWindow();
			var pageContainer = Parent as PageContainer;
			if (pageContainer != null && pageContainer.IsInFragment)
				return;
			PageController.SendDisappearing();
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
		{
			Page view = e.NewElement;
			base.OnElementChanged(e);

			if (Id == NoId)
			{
				Id = Platform.GenerateViewId();
			}

			UpdateBackground(false);

			Clickable = true;
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == Page.BackgroundImageProperty.PropertyName)
				UpdateBackground(true);
			else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
				UpdateBackground(false);
			else if (e.PropertyName == VisualElement.HeightProperty.PropertyName)
				UpdateHeight();
		}

		void UpdateHeight()
		{
			// Handle size changes because of the soft keyboard (there's probably a more elegant solution to this)

			// This is only necessary if:
			// - we're navigating back from a page where the soft keyboard was open when the user hit the Navigation Bar 'back' button
			// - the Application's content height has changed because WindowSoftInputModeAdjust was set to Resize
			// - the height has increased (in other words, the last layout was with the keyboard open, and now it's closed)
			var newHeight = Element.Height;

			if (_previousHeight > 0 && newHeight > _previousHeight)
			{
				var nav = Element.Navigation;

				// This update check will fire for all the pages on the stack, but we only need to request a layout for the top one
				if (nav?.NavigationStack != null && nav.NavigationStack.Count > 0 && Element == nav.NavigationStack[nav.NavigationStack.Count - 1])
				{
					// The Forms layout stuff is already correct, we just need to force Android to catch up
					RequestLayout();
				}
			}

			// Cache the height for next time
			_previousHeight = newHeight;
		}

		void UpdateBackground(bool setBkndColorEvenWhenItsDefault)
		{
			Page page = Element;

			string bkgndImage = page.BackgroundImage;
			if (!string.IsNullOrEmpty(bkgndImage))
				this.SetBackground(Context.GetDrawable(bkgndImage));
			else
			{
				Color bkgndColor = page.BackgroundColor;
				bool isDefaultBkgndColor = bkgndColor.IsDefault;
				if (page.Parent is BaseShellItem && isDefaultBkgndColor)
				{
					var color = Forms.IsMarshmallowOrNewer ?
						Context.Resources.GetColor(AColorRes.BackgroundLight, Context.Theme) :
						new AColor(ContextCompat.GetColor(Context, global::Android.Resource.Color.BackgroundLight));
					SetBackgroundColor(color);
				}
				else if (!isDefaultBkgndColor || setBkndColorEvenWhenItsDefault)
					SetBackgroundColor(bkgndColor.ToAndroid());
			}
		}
	}
}