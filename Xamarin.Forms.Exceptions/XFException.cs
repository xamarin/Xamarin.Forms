using System.Composition;
using System.Xml;

namespace Xamarin.Forms.Exceptions
{
	[Export(typeof(IXamlException))]
	public class XFException : IXamlException
	{
		public bool CodeIsCorrect(string errorCode)
			=> new FormatHelper() { ErrorCode = errorCode }.Prefix.ToUpperInvariant() == "XF";

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
				case 0001:
					format.UnformattedMessage = "Can't resolve type '{0}'.";
					break;
				case 0002:
					format.UnformattedMessage = "Can't resolve property '{0}' to '{1}'.";
					break;
				case 0003:
					format.UnformattedMessage = "Can not Add() elements to '{0}'.";
					break;
				case 0004:
					format.UnformattedMessage = "Can not set the content of '{0}' as it doesn't have a '{1}'.";
					break;
				case 0005:
					format.UnformattedMessage = "Can't resolve name '{0}' of {1}.";
					break;
				case 0006:
					format.UnformattedMessage = "Cannot assign property '{0}': Property does not exist, or is not assignable, or mismatching type between value and property.";
					break;
				case 0010:
					format.UnformattedMessage = "Binding: Property '{0}' not found on '{1}'.";
					break;
				case 0011:
					format.UnformattedMessage = "Binding: Indexer did not contain closing bracket.";
					break;
				case 0012:
					format.UnformattedMessage = "Binding: Indexer did not contain arguments.";
					break;
				case 0013:
					format.UnformattedMessage = "Binding: Unsupported indexer index type: {0}.";
					break;
				case 0014:
					format.UnformattedMessage = "Binding: {0} could not be parsed as an index for a {1}.";
					break;
				case 0015:
					format.UnformattedMessage = "The name of the bindable property {0} does not ends with \"Property\". This is the kind of convention the world is build upon, a bit like Planck's constant.";
					break;
				case 0016:
					format.UnformattedMessage = "Missing a public static '{0}' or a public instance property getter for the attached property '{0}'.";
					break;
				case 0021:
					format.UnformattedMessage = "x:DataType expects a string literal, an {{x:Type}} markup or {{x:Null}}.";
					break;
				case 0022:
					format.UnformattedMessage = "Undeclared xmlns prefix {0}.";
					break;
				case 0023:
					format.UnformattedMessage = "No property, bindable property, or event found for '{0}', or mismatching type between value and property.";
					break;
				case 0024:
					format.UnformattedMessage = "Expected {0} but found {1}.";
					break;
				case 0025:
					format.UnformattedMessage = "A resource with the key '{0}' is already present in the {1}.";
					break;
				case 0026:
					format.UnformattedMessage = "Resources in {0} require a x:Key attribute.";
					break;
				case 0030:
					format.UnformattedMessage = "Property not set.";
					break;
				case 0031:
					format.UnformattedMessage = "No resource found for '{0}'.";
					break;
				case 0032:
					format.UnformattedMessage = "TypeName isn't set.";
					break;
				case 0040:
					format.UnformattedMessage = "StyleSheet require either a Source or a content.";
					break;
				case 0041:
					format.UnformattedMessage = "Style property or Content is not a string literal.";
					break;
				case 0042:
					format.UnformattedMessage = "'{0}' is a duplicate.";
					break;
				case 0043:
					format.UnformattedMessage = "Multiple child elements in '{0}'.";
					break;
				case 0044:
					format.UnformattedMessage = "Type '{0}' is not a {1}.";
					break;
				case 0045:
					format.UnformattedMessage = "Type '{0}' not found in xmlns '{1}'.";
					break;
				case 0046:
					format.UnformattedMessage = "No method '{0}' with correct signature found on type '{1}'.";
					break;
				case 0050:
					format.UnformattedMessage = "'{0}' not found for '{1}'.";
					break;
				case 0051:
					format.UnformattedMessage = "Missing public default constructor for '{0}'.";
					break;
				case 0055:
					format.UnformattedMessage = "{0} requires a non-null value to be specified for at least one idiom or Default.";
					break;
				case 0056:
					format.UnformattedMessage = "{0} require {1}.";
					break;
				case 0057:
					format.UnformattedMessage = "{0} is invalid.";
					break;
				case 0058:
					format.UnformattedMessage = "{0} extension failed.";
					break;
				case 0059:
					format.UnformattedMessage = "{0} extension not closed.";
					break;
				case 0060:
					format.UnformattedMessage = "Syntax for {0} is {1}.";
					break;
				case 0061:
					format.UnformattedMessage = "x:Static: unable to find a public -- or accessible internal -- static field, static property, const or enum value named {0} in {1}.";
					break;
				case 0062:
					format.UnformattedMessage = "Can not find the object referenced by {0}.";
					break;
				case 0063:
					format.UnformattedMessage = "{0} only accepts Relative URIs.";
					break;
				case 0064:
					format.UnformattedMessage = "Resource '{0}' not found.";
					break;
				case 0065:
					format.UnformattedMessage = "No constructors found for {0} with matching x:Arguments";
					break;
				case 0070:
					format.UnformattedMessage = "Multi-valued enums are not valid on sbyte enum types";
					break;
			}
			return format.GetMessage(args);
		}
	}
}