using System;

namespace Xamarin.Forms
{
#if NETSTANDARD2_0
	[Serializable]
#endif
	public class InvalidNavigationException : Exception
	{
		public InvalidNavigationException(string message) : base(message)
		{
		}
	}
}