using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Foundation; 
using SizeF = CoreGraphics.CGSize; 

#if __MOBILE__
using UIKit;
using NativeButton = UIKit.UIButton;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Xamarin.Forms.Platform.iOS
#else
using AppKit;
using static Xamarin.Forms.Platform.MacOS.ButtonRenderer;
using NativeButton = Xamarin.Forms.Platform.MacOS.ButtonRenderer.FormsNSButton;
using Xamarin.Forms.Platform.MacOS;

namespace Xamarin.Forms.Platform.MacOS
#endif 
{ 

	public class ImageButtonRenderer : ViewRenderer<ImageButton, NativeButton>, IImageVisualElementRenderer
	{
		bool _isDisposed;

		// This looks like it should be a const under iOS Classic,
		// but that doesn't work under iOS 
		// ReSharper disable once BuiltInTypeReferenceStyle
		// Under iOS Classic Resharper wants to suggest this use the built-in type ref
		// but under iOS that suggestion won't work
		readonly nfloat _minimumButtonHeight = 44; // Apple docs

		public bool IsDisposed => throw new NotImplementedException();

		public ImageButtonRenderer() : base()
		{
			BorderElementManager.Init(this); 
			ButtonElementManager.Init(this); 
			ImageElementManager.Init(this);
		}

#if __MOBILE__
		public override SizeF SizeThatFits(SizeF size)
		{
			var result = base.SizeThatFits(size);

			if (result.Height < _minimumButtonHeight)
			{
				result.Height = _minimumButtonHeight;
			}

			return result;
		}
#endif

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			if (disposing && Control != null)
			{ 
				ButtonElementManager.Dispose(this);
				BorderElementManager.Dispose(this); 
				ImageElementManager.Dispose(this);
			}

			_isDisposed = true;
			base.Dispose(disposing);
		}

		protected async override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == ImageButton.SourceProperty.PropertyName)
				await ImageElementManager.SetImage(this, Element).ConfigureAwait(false);
			else if (e.PropertyName == ImageButton.PaddingProperty.PropertyName)
				UpdatePadding();
		}

		protected async override void OnElementChanged(ElementChangedEventArgs<ImageButton> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var control = CreateNativeControl();
					#if __MOBILE__
					control.ClipsToBounds = true;
#endif
					SetNativeControl(control);

					Debug.Assert(Control != null, "Control != null");
				}

				UpdatePadding();
				await UpdateImage().ConfigureAwait(false);
			}
		}

		void UpdatePadding(NativeButton button = null)
		{
			var uiElement = button ?? Control;
			if (uiElement == null)
				return;

#if __MOBILE__
			uiElement.ContentEdgeInsets = new UIEdgeInsets(
				(float)(Element.Padding.Top),
				(float)(Element.Padding.Left),
				(float)(Element.Padding.Bottom),
				(float)(Element.Padding.Right)
			);
#else
			(Control as FormsNSButton)?.UpdatePadding(Element.Padding);
#endif
		}

		async Task UpdateImage()
		{
			try
			{
				await ImageElementManager.SetImage(this, Element).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				Internals.Log.Warning(nameof(ImageRenderer), "Error loading image: {0}", ex);
			}
		}

		protected override NativeButton CreateNativeControl()
		{
#if __MOBILE__
			return new UIButton(UIButtonType.System);
#else
			return new FormsNSButton
			{
				BezelStyle = NSBezelStyle.ShadowlessSquare,
				Bordered = false
			};
#endif
		}

		protected override void SetAccessibilityLabel()
		{
			// If we have not specified an AccessibilityLabel and the AccessibiltyLabel is current bound to the Title,
			// exit this method so we don't set the AccessibilityLabel value and break the binding.
			// This may pose a problem for users who want to explicitly set the AccessibilityLabel to null, but this
			// will prevent us from inadvertently breaking UI Tests that are using Query.Marked to get the dynamic Title 
			// of the ImageButton. 

			var elemValue = (string)Element?.GetValue(AutomationProperties.NameProperty);
			if (string.IsNullOrWhiteSpace(elemValue) && Control?.AccessibilityLabel ==
#if __MOBILE__
				Control?.Title(UIControlState.Normal)
#else
				Control?.Title
#endif
				)
				return;

			base.SetAccessibilityLabel();
		}

		bool IImageVisualElementRenderer.IsDisposed => _isDisposed;

#if __MOBILE__ 
		void IImageVisualElementRenderer.SetImage(UIImage image)
		{
			Control.SetImage(image?.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
			Control.HorizontalAlignment = UIControlContentHorizontalAlignment.Fill;
			Control.VerticalAlignment = UIControlContentVerticalAlignment.Fill;
		}

		UIImageView IImageVisualElementRenderer.GetImage()
		{
			return Control?.ImageView;
		}
#else
		public void SetImage(NSImage image)
		{
			Control.Image = image;
		}

		public IImageView GetImage()
		{
			return Control;
		}
#endif
	}
}
