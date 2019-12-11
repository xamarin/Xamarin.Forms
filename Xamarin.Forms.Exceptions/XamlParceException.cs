using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace Xamarin.Forms.Exceptions
{
#if NETSTANDARD2_0
	[Serializable]
#endif
	public abstract class XamlParseException : Exception
	{
		readonly Regex _curlyBracketRegex = new Regex(@"\{(?<num>\d+?).*?\}");
		string _message;
		readonly string _undefinedMessage = "Undefined exception";
		static readonly string _positionMessage = "Position";

		string GetTypeString(bool isError) => isError ? "error" : "warning";

		public virtual string Prefix { get; }

		public bool Error { get; private set; }

		// http://blogs.msdn.com/b/msbuild/archive/2006/11/03/msbuild-visual-studio-aware-error-messages-and-message-formats.aspx
		public virtual string GetMessage()
		{
			throw new NotImplementedException();
		}

		public IXmlLineInfo XmlInfo { get; }

		public int Code { get; }

		public string ErrorCode => $"{Prefix}{Code:0000}";

		public virtual string ErrorSubcategory { get; }

		public virtual string HelpKeyword { get; }

#if NETSTANDARD2_0
		protected XamlParseException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			if (int.TryParse(info.GetString("errorCode"), out int code))
				Code = code;
			ErrorSubcategory = info.GetString("errorCode");
			HelpKeyword = info.GetString("helpKeyword");
		}

		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errorSubcategory", ErrorSubcategory);
			info.AddValue("errorCode", ErrorCode);
			info.AddValue("helpKeyword", HelpKeyword);
		}
#endif

		public XamlParseException(int code, params string[] args)
			: this(code, xmlInfo: null, args)
		{
		}

		protected XamlParseException(int code, IXmlLineInfo xmlInfo, params string[] args)
			: this(code, xmlInfo, innerException: null, args)
		{
		}

		public XamlParseException(int code, IXmlLineInfo xmlInfo, Exception innerException, params string[] args)
			: this(code, true, xmlInfo, innerException, args)
		{
		}

		public XamlParseException(int code, bool error, IXmlLineInfo xmlInfo, Exception innerException, params string[] args)
			: base(code.ToString("0000"), innerException)
		{
			Code = code;
			XmlInfo = xmlInfo;
			Error = error;
			_message = Format(GetMessage(), args);
		}

		// http://blogs.msdn.com/b/msbuild/archive/2006/11/03/msbuild-visual-studio-aware-error-messages-and-message-formats.aspx
		string Format(string unformatedMessage, params object[] args)
		{
			var message = string.IsNullOrEmpty(unformatedMessage)
				? _undefinedMessage
				: $"{GetTypeString(Error)} {Prefix}{Code:0000}: {SmartFormat(unformatedMessage, args)}";

			return AddPosition(XmlInfo, message);
		}

		string SmartFormat(string formatString, params object[] args)
		{
			var matches = _curlyBracketRegex.Matches(formatString);
			if (matches.Count == 0)
				return formatString;

			var requiredArgs = matches.Cast<Match>().Max(m => int.Parse(m.Groups["num"].Value) + 1);
			if (requiredArgs > args.Length)
			{
				var result = new string[requiredArgs];
				args.CopyTo(result, 0);
				for (int i = args.Length; i < requiredArgs; i++)
					result[i] = string.Empty;
				args = result;
			}
			return string.Format(formatString, args);
		}

		string AddPosition(IXmlLineInfo XmlLineInfo, string message)
		{
			if (XmlLineInfo == null || !XmlLineInfo.HasLineInfo())
				return message;
			return $"{_positionMessage} ({XmlLineInfo.LineNumber},{XmlLineInfo.LinePosition}): {message}";
		}

		public override string Message => _message;

		public override string ToString() => _message;
	}
}
