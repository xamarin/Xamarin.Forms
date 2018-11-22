﻿using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Xamarin.Forms.Internals;
using WImageSource = Windows.UI.Xaml.Media.ImageSource;
using UwpScrollBarVisibility = Windows.UI.Xaml.Controls.ScrollBarVisibility;

namespace Xamarin.Forms.Platform.UWP
{
	internal static class Extensions
	{
		public static ConfiguredTaskAwaitable<T> DontSync<T>(this IAsyncOperation<T> self)
		{
			return self.AsTask().ConfigureAwait(false);
		}

		public static ConfiguredTaskAwaitable DontSync(this IAsyncAction self)
		{
			return self.AsTask().ConfigureAwait(false);
		}

		public static void SetBinding(this FrameworkElement self, DependencyProperty property, string path)
		{
			self.SetBinding(property, new Windows.UI.Xaml.Data.Binding { Path = new PropertyPath(path) });
		}

		public static void SetBinding(this FrameworkElement self, DependencyProperty property, string path, Windows.UI.Xaml.Data.IValueConverter converter)
		{
			self.SetBinding(property, new Windows.UI.Xaml.Data.Binding { Path = new PropertyPath(path), Converter = converter });
		}

		public static async Task<WImageSource> ToWindowsImageSource(this ImageSource source)
		{
			IImageSourceHandler handler;
			if (source != null && (handler = Registrar.Registered.GetHandlerForObject<IImageSourceHandler>(source)) != null)
			{
				try
				{
					return await handler.LoadImageAsync(source);
				}
				catch (OperationCanceledException)
				{
					return null;
				}

			}
			else
			{
				return null;
			}
		}

		internal static InputScopeNameValue GetKeyboardButtonType(this ReturnType returnType)
		{
			switch (returnType)
			{
				case ReturnType.Default:
				case ReturnType.Done:
				case ReturnType.Go:
				case ReturnType.Next:
				case ReturnType.Send:
					return InputScopeNameValue.Default;
				case ReturnType.Search:
					return InputScopeNameValue.Search;
				default:
					throw new System.NotImplementedException($"ReturnType {returnType} not supported");
			}
		}

		internal static InputScope ToInputScope(this ReturnType returnType)
		{
			var scopeName = new InputScopeName()
			{
				NameValue = GetKeyboardButtonType(returnType)
			};

			var inputScope = new InputScope
			{
				Names = { scopeName }
			};

			return inputScope;
		}

		internal static UwpScrollBarVisibility ToUwpScrollBarVisibility(this ScrollBarVisibility visibility)
		{
			switch (visibility)
			{
				case ScrollBarVisibility.Always:
					return UwpScrollBarVisibility.Visible;
				case ScrollBarVisibility.Default:
					return UwpScrollBarVisibility.Auto;
				case ScrollBarVisibility.Never:
					return UwpScrollBarVisibility.Hidden;
				default:
					return UwpScrollBarVisibility.Auto;
			}
		}

		public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
		{
			if (value.CompareTo(min) < 0)
				return min;
			if (value.CompareTo(max) > 0)
				return max;
			return value;
		}
	}
}