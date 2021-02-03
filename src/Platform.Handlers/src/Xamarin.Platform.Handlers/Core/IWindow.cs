using System;
using System.Collections.Generic;

namespace Xamarin.Platform
{
	public interface IWindow
	{
		public IPage Page { get; set; }
		
		public void OnStopping(IDictionary<string, string> savingState);

		public void OnStarting(IReadOnlyDictionary<string, string> restoredState);
	}
}
