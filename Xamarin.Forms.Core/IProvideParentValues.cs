using System.Collections.Generic;

namespace Xamarin.Forms.Xaml
{
	public interface IProvideParentValues : IProvideValueTarget
	{
		IEnumerable<object> ParentObjects { get; }
	}
}
