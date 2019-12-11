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
			TypeAlreadyContais = 0102,
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
			var unformatedMessage = string.Empty;
			// TODO: localization
			switch ((Ecode)Code)
			{
				case Ecode.Convert:
					unformatedMessage = "Cannot convert type '{0}' to '{1}' via a reference conversion, boxing conversion, unboxing conversion, wrapping conversion, or null type conversion";
					break;
				case Ecode.TypeAlreadyContais:
					unformatedMessage = "The type '{0}' already contains a definition for '{1}'";
					break;
				case Ecode.Obsolete:
					unformatedMessage = "'{0}' is obsolete: '{1}'";
					break;
				case Ecode.Definition:
					unformatedMessage = "'{0}' does not contain a definition for '{1}' and no extension method '{2}' accepting a first argument of type '{0}' could be found (are you missing a using directive or an assembly reference?).";
					break;
			}
			return unformatedMessage;
		}
	}
}
