using System;
using EColor = ElmSharp.Color;

namespace Xamarin.Forms.Platform.Tizen
{
	/// <summary>
	/// Renderer of ContentPage.
	/// </summary>
	public class ContentPageRenderer : VisualElementRenderer<ContentPage>
	{
		/// <summary>
		/// Native control which holds the contents.
		/// </summary>
		Native.ContentPage _page;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ContentPageRenderer()
		{
			RegisterPropertyHandler(Page.BackgroundImageProperty, UpdateBackgroundImage);
			RegisterPropertyHandler(Page.TitleProperty, UpdateTitle);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<ContentPage> e)
		{
			if (null == _page)
			{
				_page = new Native.ContentPage(Forms.Context.MainWindow);
				_page.LayoutUpdated += new EventHandler<Native.LayoutEventArgs>(OnLayoutUpdated);
				SetNativeControl(_page);
			}

			base.OnElementChanged(e);
		}

		protected override void UpdateBackgroundColor()
		{
			// base.UpdateBackgroundColor() is not called on purpose, we don't want the regular background setting
			if (Element.BackgroundColor.IsDefault || Element.BackgroundColor.A == 0)
				_page.Color = EColor.Transparent;
			else
				_page.Color = Element.BackgroundColor.ToNative();
		}

		protected override void UpdateLayout()
		{
			// empty on purpose
		}

		void UpdateBackgroundImage()
		{
			if (string.IsNullOrWhiteSpace(Element.BackgroundImage))
				_page.File = null;
			else
				_page.File = ResourcePath.GetPath(Element.BackgroundImage);
		}

		void UpdateTitle()
		{
			_page.Title = Element.Title;
		}

		void OnLayoutUpdated(object sender, Native.LayoutEventArgs e)
		{
			DoLayout(e);
		}
	}
}
