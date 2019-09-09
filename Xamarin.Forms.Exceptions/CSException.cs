using System.Composition;
using System.Xml;

namespace Xamarin.Forms.Exceptions
{
	[Export(typeof(IXamlException))]
	public class CSException : IXamlException
	{
		public bool CodeIsCorrect(string errorCode)
			=> new FormatHelper() { ErrorCode = errorCode }.Prefix.ToUpperInvariant() == "CS";

		public string GetMessage(string errorCode, IXmlLineInfo xmlinfo, params string[] args)
		{
			var format = new FormatHelper
			{
				PositionString = "Position",
				XmlLineInfo = xmlinfo,
				ErrorCode = errorCode
			};
			// TODO: localization
			switch (format.NumericCode)
			{
				case 0039:
					format.UnformattedMessage = "Cannot convert type '{0}' to '{1}' via a reference conversion, boxing conversion, unboxing conversion, wrapping conversion, or null type conversion";
					break;
				case 0102:
					format.UnformattedMessage = "The type '{0}' already contains a definition for '{1}'";
					break;
				case 0618:
					format.UnformattedMessage = "'{0}' is obsolete: '{1}'";
					break;
				case 1061:
					format.UnformattedMessage = "'{0}' does not contain a definition for '{1}' and no extension method '{2}' accepting a first argument of type '{0}' could be found (are you missing a using directive or an assembly reference?).";
					break;
			}

			return format.GetMessage(args);
		}
	}
}
