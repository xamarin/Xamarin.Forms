using System;

namespace Xamarin.Forms
{
#if NETSTANDARD2_0
	[Serializable]
#endif
	public class UnsolvableConstraintsException : Exception
	{
		public UnsolvableConstraintsException(string message) : base(message)
		{
		}
	}
}