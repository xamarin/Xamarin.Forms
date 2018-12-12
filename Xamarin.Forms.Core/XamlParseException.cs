using System;
using System.Xml;

namespace Xamarin.Forms.Xaml
{
#if NETSTANDARD2_0
	[Serializable]
#endif
    public class XamlParseException : Exception
    {
        public XamlParseException()
        {
        }

        public XamlParseException(string message)
           : base(message)
        {
        }

        public XamlParseException(string message, Exception innerException)
           : base(message, innerException)
        {
        }

#if NETSTANDARD2_0
		protected XamlParseException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}
#endif

		public XamlParseException(string message, IXmlLineInfo xmlInfo, Exception innerException = null)
			: this(FormatMessage(message, xmlInfo), innerException)
		{
			_unformattedMessage = message;
			XmlInfo = xmlInfo;
		}

        public IXmlLineInfo XmlInfo { get; private set; }
        internal string UnformattedMessage => _unformattedMessage ?? Message;

        static string FormatMessage(string message, IXmlLineInfo xmlinfo)
        {
            if (xmlinfo == null || !xmlinfo.HasLineInfo())
                return message;
            return string.Format("Position {0}:{1}. {2}", xmlinfo.LineNumber, xmlinfo.LinePosition, message);
        }

        static IXmlLineInfo GetLineInfo(IServiceProvider serviceProvider)
            => (serviceProvider.GetService(typeof(IXmlLineInfoProvider)) is IXmlLineInfoProvider lineInfoProvider) ? lineInfoProvider.XmlLineInfo : new XmlLineInfo();
    }
}