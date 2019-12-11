using System;
using System.Xml;

namespace Xamarin.Forms.Exceptions
{
#if NETSTANDARD2_0
	[Serializable]
#endif
	public class XFException : XamlParseException
	{
		public enum Ecode
		{
			ResolveType = 0001,
			ResolveProperty = 0002,
			AddElementsTo = 0003,
			SetContent = 0004,
			ResolveName = 0005,
			AssignProperty = 0006,
			BindingPropertyNotFound = 0010,
			BindingClosingBracket = 0011,
			BindingArguments = 0012,
			BindingUnsupportedType = 0013,
			BindingParse = 0014,
			BindingEndsProperty = 0015,
			PublicStaticProperty = 0016,
			DataTypeStringLiteral = 0021,
			UndeclaredPrefix = 0022,
			TypeMismatch = 0023,
			Unexpected = 0024,
			ResourceAlreadyPresent = 0025,
			ResourceRequireKey = 0026,
			PropertyNotSet = 0030,
			NoResourceFoundFor = 0031,
			TypeName = 0032,
			StyleSheet = 0040,
			StyleOrContent = 0041,
			Duplicate = 0042,
			MultipleChild = 0043,
			BadType = 0044,
			TypeNotFound = 0045,
			MethodNotFound = 0046,
			SomethingNotFound = 0050,
			MissingConstructor = 0051,
			NonNullValue = 0055,
			Requires = 0056,
			Invalid = 0057,
			ExtensionFailed = 0058,
			ExtensionNotClosed = 0059,
			Syntax = 0060,
			Xstatic = 0061,
			ObjectNotFound = 0062,
			RelativeUriOnly = 0063,
			ResourceNotFound = 0064,
			ConstructorsNotFound = 0065,
			MultiEnumToSbyte = 0070,
		}

		public override string Prefix => "XF";

		public XFException(Ecode code, params string[] args)
			: base((int)code, args)
		{
		}

		public XFException(Ecode code, IXmlLineInfo xmlInfo, params string[] args)
			: base((int)code, xmlInfo, args)
		{
		}

		public XFException(Ecode code, IXmlLineInfo xmlInfo, Exception innerException, params string[] args)
			: base((int)code, xmlInfo, innerException, args)
		{
		}

		public override string GetMessage()
		{
			var unformatedMessage = string.Empty;
			// TODO: localization
			switch ((Ecode)Code)
			{
				case Ecode.ResolveType:
					unformatedMessage = "Can't resolve type '{0}'.";
					break;
				case Ecode.ResolveProperty:
					unformatedMessage = "Can't resolve property '{0}' to '{1}'.";
					break;
				case Ecode.AddElementsTo:
					unformatedMessage = "Can not Add() elements to '{0}'.";
					break;
				case Ecode.SetContent:
					unformatedMessage = "Can not set the content of '{0}' as it doesn't have a '{1}'.";
					break;
				case Ecode.ResolveName:
					unformatedMessage = "Can't resolve name '{0}' of {1}.";
					break;
				case Ecode.AssignProperty:
					unformatedMessage = "Cannot assign property '{0}': Property does not exist, or is not assignable, or mismatching type between value and property.";
					break;
				case Ecode.BindingPropertyNotFound:
					unformatedMessage = "Binding: Property '{0}' not found on '{1}'.";
					break;
				case Ecode.BindingClosingBracket:
					unformatedMessage = "Binding: Indexer did not contain closing bracket.";
					break;
				case Ecode.BindingArguments:
					unformatedMessage = "Binding: Indexer did not contain arguments.";
					break;
				case Ecode.BindingUnsupportedType:
					unformatedMessage = "Binding: Unsupported indexer index type: {0}.";
					break;
				case Ecode.BindingParse:
					unformatedMessage = "Binding: {0} could not be parsed as an index for a {1}.";
					break;
				case Ecode.BindingEndsProperty:
					unformatedMessage = "The name of the bindable property {0} does not ends with \"Property\". This is the kind of convention the world is build upon, a bit like Planck's constant.";
					break;
				case Ecode.PublicStaticProperty:
					unformatedMessage = "Missing a public static '{0}' or a public instance property getter for the attached property '{0}'.";
					break;
				case Ecode.DataTypeStringLiteral:
					unformatedMessage = "x:DataType expects a string literal, an {{x:Type}} markup or {{x:Null}}.";
					break;
				case Ecode.UndeclaredPrefix:
					unformatedMessage = "Undeclared xmlns prefix {0}.";
					break;
				case Ecode.TypeMismatch:
					unformatedMessage = "No property, bindable property, or event found for '{0}', or mismatching type between value and property.";
					break;
				case Ecode.Unexpected:
					unformatedMessage = "Expected {0} but found {1}.";
					break;
				case Ecode.ResourceAlreadyPresent:
					unformatedMessage = "A resource with the key '{0}' is already present in the {1}.";
					break;
				case Ecode.ResourceRequireKey:
					unformatedMessage = "Resources in {0} require a x:Key attribute.";
					break;
				case Ecode.PropertyNotSet:
					unformatedMessage = "Property not set.";
					break;
				case Ecode.NoResourceFoundFor:
					unformatedMessage = "No resource found for '{0}'.";
					break;
				case Ecode.TypeName:
					unformatedMessage = "TypeName isn't set.";
					break;
				case Ecode.StyleSheet:
					unformatedMessage = "StyleSheet require either a Source or a content.";
					break;
				case Ecode.StyleOrContent:
					unformatedMessage = "Style property or Content is not a string literal.";
					break;
				case Ecode.Duplicate:
					unformatedMessage = "'{0}' is a duplicate.";
					break;
				case Ecode.MultipleChild:
					unformatedMessage = "Multiple child elements in '{0}'.";
					break;
				case Ecode.BadType:
					unformatedMessage = "Type '{0}' is not a {1}.";
					break;
				case Ecode.TypeNotFound:
					unformatedMessage = "Type '{0}' not found in xmlns '{1}'.";
					break;
				case Ecode.MethodNotFound:
					unformatedMessage = "No method '{0}' with correct signature found on type '{1}'.";
					break;
				case Ecode.SomethingNotFound:
					unformatedMessage = "'{0}' not found for '{1}'.";
					break;
				case Ecode.MissingConstructor:
					unformatedMessage = "Missing public default constructor for '{0}'.";
					break;
				case Ecode.NonNullValue:
					unformatedMessage = "{0} requires a non-null value to be specified for at least one idiom or Default.";
					break;
				case Ecode.Requires:
					unformatedMessage = "{0} require {1}.";
					break;
				case Ecode.Invalid:
					unformatedMessage = "{0} is invalid.";
					break;
				case Ecode.ExtensionFailed:
					unformatedMessage = "{0} extension failed.";
					break;
				case Ecode.ExtensionNotClosed:
					unformatedMessage = "{0} extension not closed.";
					break;
				case Ecode.Syntax:
					unformatedMessage = "Syntax for {0} is {1}.";
					break;
				case Ecode.Xstatic:
					unformatedMessage = "x:Static: unable to find a public -- or accessible internal -- static field, static property, const or enum value named {0} in {1}.";
					break;
				case Ecode.ObjectNotFound:
					unformatedMessage = "Can not find the object referenced by {0}.";
					break;
				case Ecode.RelativeUriOnly:
					unformatedMessage = "{0} only accepts Relative URIs.";
					break;
				case Ecode.ResourceNotFound:
					unformatedMessage = "Resource '{0}' not found.";
					break;
				case Ecode.ConstructorsNotFound:
					unformatedMessage = "No constructors found for {0} with matching x:Arguments";
					break;
				case Ecode.MultiEnumToSbyte:
					unformatedMessage = "Multi-valued enums are not valid on sbyte enum types";
					break;
			}
			return unformatedMessage;
		}
	}
}