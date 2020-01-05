using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms
{
	public class ModalBehavior : BindableObject
	{
		static ModalBehavior _defaultModalBehavior;

		public static readonly BindableProperty ModalProperty =
			BindableProperty.Create(nameof(Modal), typeof(bool), typeof(ModalBehavior), true);

		public static readonly BindableProperty AnimatedProperty =
			BindableProperty.Create(nameof(Animated), typeof(bool), typeof(ModalBehavior), false);

		public bool Modal
		{
			get => (bool)GetValue(ModalProperty);
			set => SetValue(ModalProperty, value);
		}

		public bool Animated
		{
			get => (bool)GetValue(AnimatedProperty);
			set => SetValue(AnimatedProperty, value);
		}

		internal static ModalBehavior Default
		{
			get
			{
				if (_defaultModalBehavior == null)
					_defaultModalBehavior = new ModalBehavior() { Modal = false };

				return _defaultModalBehavior;
			}
		}
	}
}
