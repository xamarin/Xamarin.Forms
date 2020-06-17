﻿using System;
using System.ComponentModel;

namespace Xamarin.Forms
{
	public class SwipeItem : MenuItem, ISwipeItem
	{
		public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(SwipeItem), Color.Default);

		public static readonly BindableProperty IsVisibleProperty = BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(SwipeItem), true);

		public Color BackgroundColor
		{
			get { return (Color)GetValue(BackgroundColorProperty); }
			set { SetValue(BackgroundColorProperty, value); }
		}

		public bool IsVisible
		{
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}

		public event EventHandler<EventArgs> Invoked;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void OnInvoked()
		{
			if (Command != null && Command.CanExecute(CommandParameter))
				Command.Execute(CommandParameter);

			Invoked?.Invoke(this, EventArgs.Empty);
		}
	}
}