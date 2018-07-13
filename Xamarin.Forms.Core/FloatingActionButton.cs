using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_FloatingActionButtonRenderer))]
	public class FloatingActionButton : View, IButtonController
	{
		public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create("ImageSource", typeof(ImageSource), typeof(FloatingActionButton), null);

		public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(FloatingActionButton), null);

		public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(FloatingActionButton), null);

		public static readonly BindableProperty SizeProperty = BindableProperty.Create("Size", typeof(FloatingActionButtonSize), typeof(FloatingActionButton), FloatingActionButtonSize.Normal);

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

		public event EventHandler Clicked;
		public event EventHandler Pressed;
		public event EventHandler Released;

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
	}

	public enum FloatingActionButtonSize
	{
	    Normal,
	    Mini
	}
}
