using System.Xml;

namespace Xamarin.Forms.Exceptions
{
	public interface IXamlException
	{
		bool CodeIsCorrect(string errorCode);
		string GetMessage(string errorCode, IXmlLineInfo xmlinfo, params string[] args);
	}
}
