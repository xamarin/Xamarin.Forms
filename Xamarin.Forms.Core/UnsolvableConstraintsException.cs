using System;
using System.Runtime.Serialization;

namespace Xamarin.Forms
{
#if NETSTANDARD2_0
	[Serializable]
#endif
	public class UnsolvableConstraintsException : Exception
	{
		public UnsolvableConstraintsException()
		{
		}

		public UnsolvableConstraintsException(string message)
			: base(message)
		{
		}

		public UnsolvableConstraintsException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected UnsolvableConstraintsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}