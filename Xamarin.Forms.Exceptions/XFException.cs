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
			var unformattedMessage = string.Empty;
			switch ((Ecode)Code)
			{
				case Ecode.ResolveType:
					unformattedMessage = Resources.ResolveType;
					break;
				case Ecode.ResolveProperty:
					unformattedMessage = Resources.ResolveProperty;
					break;
				case Ecode.AddElementsTo:
					unformattedMessage = Resources.AddElementsTo;
					break;
				case Ecode.SetContent:
					unformattedMessage = Resources.SetContent;
					break;
				case Ecode.ResolveName:
					unformattedMessage = Resources.ResolveName;
					break;
				case Ecode.AssignProperty:
					unformattedMessage = Resources.AssignProperty;
					break;
				case Ecode.BindingPropertyNotFound:
					unformattedMessage = Resources.BindingPropertyNotFound;
					break;
				case Ecode.BindingClosingBracket:
					unformattedMessage = Resources.BindingClosingBracket;
					break;
				case Ecode.BindingArguments:
					unformattedMessage = Resources.BindingArguments;
					break;
				case Ecode.BindingUnsupportedType:
					unformattedMessage = Resources.BindingUnsupportedType;
					break;
				case Ecode.BindingParse:
					unformattedMessage = Resources.BindingParse;
					break;
				case Ecode.BindingEndsProperty:
					unformattedMessage = Resources.BindingEndsProperty;
					break;
				case Ecode.PublicStaticProperty:
					unformattedMessage = Resources.PublicStaticProperty;
					break;
				case Ecode.DataTypeStringLiteral:
					unformattedMessage = Resources.DataTypeStringLiteral;
					break;
				case Ecode.UndeclaredPrefix:
					unformattedMessage = Resources.UndeclaredPrefix;
					break;
				case Ecode.TypeMismatch:
					unformattedMessage = Resources.TypeMismatch;
					break;
				case Ecode.Unexpected:
					unformattedMessage = Resources.Unexpected;
					break;
				case Ecode.ResourceAlreadyPresent:
					unformattedMessage = Resources.ResourceAlreadyPresent;
					break;
				case Ecode.ResourceRequireKey:
					unformattedMessage = Resources.ResourceRequireKey;
					break;
				case Ecode.PropertyNotSet:
					unformattedMessage = Resources.PropertyNotSet;
					break;
				case Ecode.NoResourceFoundFor:
					unformattedMessage = Resources.NoResourceFoundFor;
					break;
				case Ecode.TypeName:
					unformattedMessage = Resources.TypeName;
					break;
				case Ecode.StyleSheet:
					unformattedMessage = Resources.StyleSheet;
					break;
				case Ecode.StyleOrContent:
					unformattedMessage = Resources.StyleOrContent;
					break;
				case Ecode.Duplicate:
					unformattedMessage = Resources.Duplicate;
					break;
				case Ecode.MultipleChild:
					unformattedMessage = Resources.MultipleChild;
					break;
				case Ecode.BadType:
					unformattedMessage = Resources.BadType;
					break;
				case Ecode.TypeNotFound:
					unformattedMessage = Resources.TypeNotFound;
					break;
				case Ecode.MethodNotFound:
					unformattedMessage = Resources.MethodNotFound;
					break;
				case Ecode.SomethingNotFound:
					unformattedMessage = Resources.SomethingNotFound;
					break;
				case Ecode.MissingConstructor:
					unformattedMessage = Resources.MissingConstructor;
					break;
				case Ecode.NonNullValue:
					unformattedMessage = Resources.NonNullValue;
					break;
				case Ecode.Requires:
					unformattedMessage = Resources.Requires;
					break;
				case Ecode.Invalid:
					unformattedMessage = Resources.Invalid;
					break;
				case Ecode.ExtensionFailed:
					unformattedMessage = Resources.ExtensionFailed;
					break;
				case Ecode.ExtensionNotClosed:
					unformattedMessage = Resources.ExtensionNotClosed;
					break;
				case Ecode.Syntax:
					unformattedMessage = Resources.Syntax;
					break;
				case Ecode.Xstatic:
					unformattedMessage = Resources.Xstatic;
					break;
				case Ecode.ObjectNotFound:
					unformattedMessage = Resources.ObjectNotFound;
					break;
				case Ecode.RelativeUriOnly:
					unformattedMessage = Resources.RelativeUriOnly;
					break;
				case Ecode.ResourceNotFound:
					unformattedMessage = Resources.ResourceNotFound;
					break;
				case Ecode.ConstructorsNotFound:
					unformattedMessage = Resources.ConstructorsNotFound;
					break;
				case Ecode.MultiEnumToSbyte:
					unformattedMessage = Resources.MultiEnumToSbyte;
					break;
			}
			return unformattedMessage;
		}
	}
}