using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Xamarin.Platform
{
	public interface IPicker : IView
	{
		string Title { get; }
		Color TextColor { get; }
		Color TitleColor { get; }
		IList<string> Items { get; }
		IList ItemsSource { get; }
		int SelectedIndex { get; set; }
		object? SelectedItem { get; set; }
	}
}