using System;
using NUnit.Framework.Constraints;
using System.Text;

namespace Xamarin.Forms.Xaml.UnitTests
{	
	public class XamlParseExceptionConstraint : ExceptionTypeConstraint
	{
		bool haslineinfo;
		int linenumber;
		int lineposition;
		Func<string, bool> messagePredicate;

		XamlParseExceptionConstraint (bool haslineinfo) : base (typeof (XamlParseException))
		{
			this.haslineinfo = haslineinfo;
		}

		public XamlParseExceptionConstraint () : this (false)
		{
		}

		public XamlParseExceptionConstraint (int linenumber, int lineposition, Func<string, bool> messagePredicate = null) : this (true)
		{
			this.linenumber = linenumber;
			this.lineposition = lineposition;
			this.messagePredicate = messagePredicate;
		}

		public override string DisplayName {
			get { return "XamlParse"; }
		}

		public override string Description {
			get {
				var sb = new StringBuilder(base.Description);
				if (haslineinfo)
					sb.Append($" Position {linenumber}:{lineposition}");
				return sb.ToString();
			}
		}
		public override ConstraintResult ApplyTo<TActual>(TActual actual)
		{
			var ex = actual as XamlParseException;
			if (actual != null && ex == null) {
				throw new ArgumentException("Actual value must be an XamlParseException", nameof(actual));
			}
			this.actualType = ((actual == null) ? null : actual.GetType());
			return new XamlParseExceptionConstraintResult(this, actual, this.actualType, this.Matches(actual));
		}

		protected override bool Matches (object actual)
		{
			var ex = actual as XamlParseException;
			if (!base.Matches (actual))
				return false;
			if (!haslineinfo)
				return true;
			var xmlInfo = ex.XmlInfo;
			if (xmlInfo == null || !xmlInfo.HasLineInfo ())
				return false;
			if (messagePredicate != null && !messagePredicate (((XamlParseException)actual).UnformattedMessage))
				return false;
			return xmlInfo.LineNumber == linenumber && xmlInfo.LinePosition == lineposition;
		}

		class XamlParseExceptionConstraintResult : ConstraintResult
		{
			readonly object _caughtException;

			public XamlParseExceptionConstraintResult(ExceptionTypeConstraint constraint, object caughtException, Type type, bool matches) : base(constraint, type, matches)
			{
				_caughtException = caughtException;
			}

			public override void WriteActualValueTo(MessageWriter writer)
			{
				if (Status == ConstraintStatus.Failure) {
					var ex = _caughtException as XamlParseException;
					if (ex == null) {
						base.WriteActualValueTo(writer);
						return;
					}
					writer.WriteActualValue(ex);
				}
			}
		}
	}
}