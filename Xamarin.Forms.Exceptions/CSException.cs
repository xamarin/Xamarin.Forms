using System;
using System.Xml;

namespace Xamarin.Forms.Exceptions
{
#if NETSTANDARD2_0
	[Serializable]
#endif
	public class CSException : XamlParseException
	{
		public enum Ecode
		{
			Convert = 0039,
			TypeAlreadyContains = 0102,
			Obsolete = 0618,
			Definition = 1061,
		}

		public override string Prefix => "CS";

		public CSException(Ecode code, params string[] args)
			: base((int)code, args)
		{
		}

		public CSException(Ecode code, IXmlLineInfo xmlInfo, params string[] args)
			: base((int)code, xmlInfo, args)
		{
		}

		public CSException(Ecode code, IXmlLineInfo xmlInfo, Exception innerException, params string[] args)
			: base((int)code, xmlInfo, innerException, args)
		{
		}

		public override string GetMessage()
		{
			var unformattedMessage = string.Empty;
			switch ((Ecode)Code)
			{
				case Ecode.Convert:
					unformattedMessage = Resources.Convert;
					break;
				case Ecode.TypeAlreadyContains:
					unformattedMessage = Resources.TypeAlreadyContains;
					break;
				case Ecode.Obsolete:
					unformattedMessage = Resources.Obsolete;
					break;
				case Ecode.Definition:
					unformattedMessage = Resources.Definition;
					break;
			}
			return unformattedMessage;
		}
	}
}
