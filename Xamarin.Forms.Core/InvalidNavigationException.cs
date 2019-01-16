using System;
using System.Runtime.Serialization;

namespace Xamarin.Forms
{
#if NETSTANDARD2_0
	[Serializable]
#endif
	public class InvalidNavigationException : Exception
	{
		public InvalidNavigationException()
		{
		}

		public InvalidNavigationException(string message) 
			: base(message)
		{
		}

		public InvalidNavigationException(string message, Exception innerException) 
			: base(message, innerException)
		{
		}

		protected InvalidNavigationException(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}
	}
}