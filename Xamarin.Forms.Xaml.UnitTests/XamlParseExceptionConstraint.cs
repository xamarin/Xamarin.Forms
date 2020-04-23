using System;
using NUnit.Framework.Constraints;
using Xamarin.Forms.Exceptions;

namespace Xamarin.Forms.Xaml.UnitTests
{
	public class XamlParseExceptionConstraint : ExceptionTypeConstraint
	{
		bool haslineinfo;
		int linenumber;
		int lineposition;
		XFException.Ecode? code;

		XamlParseExceptionConstraint(bool haslineinfo) : base(typeof(XFException))
		{
			this.haslineinfo = haslineinfo;
		}

		public override string DisplayName => "xamlparse";

		public XamlParseExceptionConstraint() : this(false)
		{
		}

		public XamlParseExceptionConstraint(int linenumber, int lineposition, XFException.Ecode? code = null) : this(true)
		{
			this.linenumber = linenumber;
			this.lineposition = lineposition;
			this.code = code;
		}

		protected override bool Matches(object actual)
		{
			if (!base.Matches(actual))
				return false;
			var xmlInfo = ((XamlParseException)actual).XmlInfo;
			if (!haslineinfo)
				return true;
			if (xmlInfo == null || !xmlInfo.HasLineInfo())
				return false;
			if (code != null)
				if (code != (XFException.Ecode)((XamlParseException)actual).Code)
					return false;
			return xmlInfo.LineNumber == linenumber && xmlInfo.LinePosition == lineposition;
		}

		//public override void WriteActualValueTo (MessageWriter writer)
		//{
		//	var ex = actual as XamlParseException;
		//	writer.WriteActualValue ((actual == null) ? null : actual.GetType ());
		//	if (ex != null) {
		//		if (ex.XmlInfo != null && ex.XmlInfo.HasLineInfo ())
		//			writer.Write (" line {0}, position {1}", ex.XmlInfo.LineNumber, ex.XmlInfo.LinePosition);
		//		else 
		//			writer.Write (" no line info");
		//		writer.WriteLine (" ({0})", ex.Message);
		//		writer.Write (ex.StackTrace);
		//	}
		//}
	}
}