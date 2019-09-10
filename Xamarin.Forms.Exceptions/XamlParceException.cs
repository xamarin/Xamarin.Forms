using System;
using System.Collections.Generic;
using System.Xml;
using System.Composition;
using System.Composition.Hosting;

namespace Xamarin.Forms.Exceptions
{
#if NETSTANDARD2_0
	[Serializable]
#endif
	public class XamlParseException : Exception
	{
		[ImportMany]
		static IEnumerable<IXamlException> exceptions { get; set; }

		protected static CompositionContext CreateContainer(params Type[] types)
		{
			return new ContainerConfiguration()
				.WithParts(types)
				.CreateContainer();
		}

		static string GetMessage(string errorCode, IXmlLineInfo xmlinfo, params string[] args)
		{
			if (exceptions == null)
			{
				var c = CreateContainer(typeof(CSException), typeof(XFException));
				exceptions = c.GetExports<IXamlException>();
			}
			foreach (var ex in exceptions)
			{
				if (ex.CodeIsCorrect(errorCode))
				{
					return ex.GetMessage(errorCode, xmlinfo, args);
				}
			}
			return string.Empty;
		}

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

		public string ErrorCode { get; }

		public string ErrorSubcategory { get; }

		public string HelpKeyword { get; }

#if NETSTANDARD2_0
		protected XamlParseException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			ErrorCode = info.GetString("errorCode");
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

		internal XamlParseException(string message, IXmlLineInfo xmlInfo = null, Exception innerException = null, string errorCode = null)
			: base(message, innerException)
		{
			XmlInfo = xmlInfo;
			ErrorCode = errorCode;
		}

		public XamlParseException(string errorCode, IXmlLineInfo xmlInfo, params string[] args)
			: this(errorCode, xmlInfo, null, args)
		{
		}

		public XamlParseException(string errorCode, params string[] args)
			: this(errorCode, xmlInfo: null, args)
		{
		}

		public XamlParseException(string errorCode, IXmlLineInfo xmlInfo, Exception innerException, params string[] args)
			: base(GetMessage(errorCode, xmlInfo, args), innerException)
		{
			XmlInfo = xmlInfo;
			ErrorCode = errorCode.ToUpperInvariant();
		}

		public IXmlLineInfo XmlInfo { get; }
	}
}
