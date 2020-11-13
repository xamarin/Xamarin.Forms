using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Platform.Tizen
{
	public class FormattedString
	{

		readonly bool _just_string;

		readonly string _string;

		readonly ObservableCollection<Span>? _spans = null;

#pragma warning disable CS8603 // Possible null reference return.
		public IList<Span> Spans { get { return _spans; } }
#pragma warning restore CS8603 // Possible null reference return.

		public FormattedString()
		{
			_just_string = false;
			_spans = new ObservableCollection<Span>();
			_string = "";
		}

		public FormattedString(string str)
		{
			_just_string = true;
			_string = str;
		}

		public override string ToString()
		{
			if (_just_string)
			{
				return _string;
			}
			else
			{
				return string.Concat(from span in this.Spans select span.Text);
			}
		}

		internal string ToMarkupString()
		{
			if (_just_string)
			{
				return _string;
			}
			else
			{
				return string.Concat(from span in Spans select span.GetMarkupText());
			}
		}

		public static explicit operator string(FormattedString formatted)
		{
			return formatted.ToString();
		}

		public static implicit operator FormattedString(string text)
		{
			return new FormattedString(text);
		}

		public static implicit operator FormattedString(Span span)
		{
			return new FormattedString()
			{
				Spans = { span }
			};
		}
	}
}
