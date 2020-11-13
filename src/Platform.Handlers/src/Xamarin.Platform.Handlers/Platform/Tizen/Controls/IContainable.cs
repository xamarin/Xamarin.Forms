using System.Collections.Generic;

namespace Xamarin.Platform.Tizen
{
	public interface IContainable<T>
	{
		IList<T> Children { get; }
	}
}
