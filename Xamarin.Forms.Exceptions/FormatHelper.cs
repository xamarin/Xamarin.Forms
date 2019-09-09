using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace Xamarin.Forms.Exceptions
{
	class FormatHelper
	{
		readonly Regex _curlyBracketRegex = new Regex(@"\{(?<num>\d+?).*?\}");

		readonly Regex _numberRegex = new Regex(@"^(?<prefix>[^\d]+)(?<num>\d+)$");

		public int NumericCode
		{
			get
			{
				var match = _numberRegex.Match(ErrorCode);
				if (match.Success && int.TryParse(_numberRegex.Match(ErrorCode).Groups["num"].Value, out int numericCode))
					return numericCode;
				return -1;
			}
		}

		public string Prefix
		{
			get
			{
				var match = _numberRegex.Match(ErrorCode);
				return match.Success ? _numberRegex.Match(ErrorCode).Groups["prefix"].Value : null;
			}
		}

		public string PositionString { get; set; } = "Position";

		public string UnformattedMessage { get; set; } = "Undefined exception";

		public string ErrorCode { get; set; }

		public IXmlLineInfo XmlLineInfo { get; set; }

		public string GetMessage(params string[] args) => AddPosition(OptionalFormat(UnformattedMessage, args));

		string OptionalFormat(string formatString, params string[] args)
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

		string AddPosition(string message)
		{
			if (XmlLineInfo == null || !XmlLineInfo.HasLineInfo())
				return message;
			return string.Format("{0} {1}:{2}. {3}", PositionString, XmlLineInfo.LineNumber, XmlLineInfo.LinePosition, message);
		}
	}
}
