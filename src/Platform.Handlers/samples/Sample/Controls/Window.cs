using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Platform;
using Xamarin.Platform.Core;

namespace Sample.Controls
{
	public class Window : IWindow
	{
		public Window()
		{
		}

		public IPage Page { get; set; }

		public void OnStarting(IReadOnlyDictionary<string, string> restoredState)
		{
			throw new NotImplementedException();
		}

		public void OnStopping(IDictionary<string, string> savingState)
		{
			throw new NotImplementedException();
		}
	}
}
