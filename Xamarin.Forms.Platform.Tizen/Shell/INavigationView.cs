using System;
using System.Collections.Generic;
using ElmSharp;
using EColor = ElmSharp.Color;
using NImage = Xamarin.Forms.Platform.Tizen.Native.Image;

namespace Xamarin.Forms.Platform.Tizen
{
	public interface INavigationView
	{
		EvasObject Header { get; set; }

		EColor BackgroundColor { get; set; }

		NImage BackgroundImage { get; set; }

		void BuildMenu(List<List<Element>> flyout);

		event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
	}
}
