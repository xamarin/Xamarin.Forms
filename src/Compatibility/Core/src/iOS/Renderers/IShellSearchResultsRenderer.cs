using System;
using UIKit;

namespace Microsoft.Maui.Controls.Compatibility.Platform.iOS
{
	public interface IShellSearchResultsRenderer : IDisposable
	{
		UIViewController ViewController { get; }

		SearchHandler SearchHandler { get; set; }

		event EventHandler<object> ItemSelected;
	}
}