
using System;

namespace Xamarin.Forms
{
	[Obsolete("This interface is obsolete as of version 3.6. Please use IFastElementConfiguration instead.")]
	public interface IElementConfiguration<out TElement> where TElement : Element
	{
		IPlatformElementConfiguration<T, TElement> On<T>() where T : IConfigPlatform;
	}
}
