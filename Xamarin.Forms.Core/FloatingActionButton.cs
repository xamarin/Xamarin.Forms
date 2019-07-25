using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_FloatingActionButtonRenderer))]
	public class FloatingActionButton : View, IButtonController, IElementConfiguration<FloatingActionButton>, IImageElement
	{
		public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(FloatingActionButton), null);

		public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(FloatingActionButton), null);

		public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(FloatingActionButton), null);

		public static readonly BindableProperty SizeProperty = BindableProperty.Create(nameof(Size), typeof(FloatingActionButtonSize), typeof(FloatingActionButton), FloatingActionButtonSize.Normal);

		
		public ImageSource ImageSource
		{
			get { return (ImageSource)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public object CommandParameter
		{
			get { return GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		public FloatingActionButtonSize Size
		{
			get { return (FloatingActionButtonSize)GetValue(SizeProperty); }
			set { SetValue(SizeProperty, value); }
		}

		Aspect IImageElement.Aspect => Aspect.AspectFit;

		ImageSource IImageElement.Source => ImageSource;

		bool IImageElement.IsOpaque => false;

		public event EventHandler Clicked;
		public event EventHandler Pressed;
		public event EventHandler Released;

		readonly Lazy<PlatformConfigurationRegistry<FloatingActionButton>> _platformConfigurationRegistry;
		public FloatingActionButton()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<FloatingActionButton>>(() => new PlatformConfigurationRegistry<FloatingActionButton>(this));
		}

		public IPlatformElementConfiguration<T, FloatingActionButton> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendClicked()
		{
			if (IsEnabled == true)
			{
				Command?.Execute(CommandParameter);
				Clicked?.Invoke(this, EventArgs.Empty);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendPressed()
		{
			if (IsEnabled == true)
			{
				Pressed?.Invoke(this, EventArgs.Empty);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendReleased()
		{
			if (IsEnabled == true)
			{
				Released?.Invoke(this, EventArgs.Empty);
			}
		}

		void IImageElement.RaiseImageSourcePropertyChanged() => OnPropertyChanged(ImageSourceProperty.PropertyName);

		void IImageElement.OnImageSourcesSourceChanged(object sender, EventArgs e) =>
			ImageElement.ImageSourcesSourceChanged(this, e);
	}

	public enum FloatingActionButtonSize
	{
		Normal,
		Mini
	}
}
