using System;

namespace Xamarin.Forms.Fluent
{
	public static class FluentExtensions
	{
		#region BindableObject section

		public static TView Fluent<TView>(this TView self,
			Action<TView> action) where TView : BindableObject
		{
			if (self == null)
				throw new ArgumentNullException(nameof(self));
			if (action == null)
				throw new ArgumentNullException(nameof(action));

			action.Invoke(self);

			return self;
		}

		public static TView Bind<TView>(this TView self,
			BindableProperty targetProperty,
			BindingBase bindingBase) where TView : BindableObject
		{
			if (self == null)
				throw new ArgumentNullException(nameof(self));
			if (targetProperty == null)
				throw new ArgumentNullException(nameof(targetProperty));

			self.SetBinding(targetProperty, bindingBase);

			return self;
		}

		public static TView Bind<TView>(this TView self,
			BindableProperty targetProperty,
			string path,
			BindingMode mode = BindingMode.Default,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null) where TView : BindableObject
			=> Bind(self, targetProperty, new Binding
			{
				Path = path,
				Mode = mode,
				Converter = converter,
				ConverterParameter = converterParameter,
				StringFormat = stringFormat,
				Source = source
			});

		#endregion

		#region View Section

		public static TView Tap<TView>(this TView self,
			string commandPath,
			object commandParameter = null,
			int numberOfTapsRequired = 1) where TView : View
		{
			if (self == null)
				throw new ArgumentNullException(nameof(self));
			if (commandPath == null)
				throw new ArgumentNullException(nameof(commandPath));

			self.GestureRecognizers.Add(new TapGestureRecognizer
			{
				NumberOfTapsRequired = numberOfTapsRequired,
				CommandParameter = commandParameter
			}.Bind(TapGestureRecognizer.CommandProperty, commandPath));

			return self;
		}

		public static TView Gesture<TView>(this TView self,
			TapGestureRecognizer gesture) where TView : View
		{
			if (self == null)
				throw new ArgumentNullException(nameof(self));
			if (gesture == null)
				throw new ArgumentNullException(nameof(gesture));

			self.GestureRecognizers.Add(gesture);
			return self;
		}

		#endregion

		#region Layout Section

		public static TView Children<TView>(this TView self, params View[] children) where TView : Layout<View>
		{
			if (self == null)
				throw new ArgumentNullException(nameof(self));
			if (children == null)
				throw new ArgumentNullException(nameof(children));

			foreach(var child in children)
			{
				if(child != null)
				{
					self.Children.Add(child);
				}
			}

			return self;
		}

		#endregion

		#region Button Section

		public static TView Command<TView>(this TView self,
			string commandPath) where TView : Button
		{
			if (self == null)
				throw new ArgumentNullException(nameof(self));
			if (commandPath == null)
				throw new ArgumentNullException(nameof(commandPath));

			return self.Bind(Button.CommandProperty, commandPath);
		}

		#endregion
	}
}
