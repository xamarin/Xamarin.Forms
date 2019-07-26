using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Xamarin.Forms.Internals;
using WImage = Windows.UI.Xaml.Controls.Image;

namespace Xamarin.Forms.Platform.UWP
{
	public class FloatingActionButtonRenderer : ViewRenderer<FloatingActionButton, FormsButton>
	{
		const float SmallSize = 44;
		const float NormalSize = 56;

		protected override void OnElementChanged(ElementChangedEventArgs<FloatingActionButton> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var button = new FormsButton();

					button.Click += OnButtonClick;
					button.AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
					button.Loaded += ButtonOnLoaded;

					SetNativeControl(button);
				}
				else
				{
					WireUpFormsVsm();
				}

				UpdateContent();

				//TODO: We may want to revisit this strategy later. If a user wants to reset any of these to the default, the UI won't update.
				if (Element.IsSet(VisualElement.BackgroundColorProperty) && Element.BackgroundColor != (Color)VisualElement.BackgroundColorProperty.DefaultValue)
					UpdateBackground();

				UpdateSize();
			}
		}

		void ButtonOnLoaded(object o, RoutedEventArgs routedEventArgs)
		{
			WireUpFormsVsm();
		}

		void WireUpFormsVsm()
		{
			if (Element.UseFormsVsm())
			{
				InterceptVisualStateManager.Hook(Control.GetFirstDescendant<Windows.UI.Xaml.Controls.Grid>(), Control, Element);
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.Is(VisualElement.BackgroundColorProperty))
			{
				UpdateBackground();
			}
			else if (e.Is(FloatingActionButton.SizeProperty))
			{
				UpdateSize();
				UpdateContent();
			}
			else if (e.Is(FloatingActionButton.ImageSourceProperty))
			{
				UpdateContent();
			}
		}

		protected override void UpdateBackgroundColor()
		{
			// Button is a special case; we don't want to set the Control's background
			// because it goes outside the bounds of the Border/ContentPresenter, 
			// which is where we might change the BorderRadius to create a rounded shape.
			return;
		}

		protected override bool PreventGestureBubbling { get; set; } = true;

		void OnButtonClick(object sender, RoutedEventArgs e)
		{
			((IButtonController)Element)?.SendReleased();
			((IButtonController)Element)?.SendClicked();
		}

		void OnPointerPressed(object sender, RoutedEventArgs e)
		{
			((IButtonController)Element)?.SendPressed();
		}

		void UpdateBackground()
		{
			Control.BackgroundColor = Element.BackgroundColor != Color.Default ? Element.BackgroundColor.ToBrush() : (Brush)Windows.UI.Xaml.Application.Current.Resources["ButtonBackgroundThemeBrush"];
		}

		void UpdateSize()
		{
			FloatingActionButtonSize elemSize = Element.Size;
			var size = elemSize == FloatingActionButtonSize.Mini ? SmallSize : NormalSize;

			Control.Width = Control.Height = size;
			Control.BorderRadius = (int)(size / 2d);
		}

		async void UpdateContent()
		{
			var elementImage = await Element.ImageSource.ToWindowsImageSourceAsync();
			
			var maxSize = Element.Size == FloatingActionButtonSize.Mini ? SmallSize * 0.5f : NormalSize * 0.5f;
			var size = elementImage.GetImageSourceSize();
			var image = new WImage
			{
				Source = elementImage,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				Stretch = Stretch.Uniform,
				Width = Math.Min(size.Width, maxSize),
				Height = Math.Min(size.Height, maxSize),
			};

			// BitmapImage is a special case that has an event when the image is loaded
			// when this happens, we want to resize the button
			if (elementImage is BitmapImage bmp)
			{
				bmp.ImageOpened += (sender, args) =>
				{
					var actualSize = bmp.GetImageSourceSize();
					image.Width = Math.Min(actualSize.Width, maxSize);
					image.Height = Math.Min(actualSize.Height, maxSize);
					Element?.InvalidateMeasureNonVirtual(InvalidationTrigger.RendererReady);
				};
			}

			Control.Content = image;
			Element?.InvalidateMeasureNonVirtual(InvalidationTrigger.RendererReady);
		}
	}
}
