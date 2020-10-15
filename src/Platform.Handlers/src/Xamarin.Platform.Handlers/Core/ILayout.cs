using System.Collections.Generic;
using Xamarin.Platform.Layouts;

namespace Xamarin.Platform
{
	public interface ILayout : IView
	{
		IReadOnlyList<IView> Children { get; }
		ILayoutManager CreateLayoutManager(); 
	}
}