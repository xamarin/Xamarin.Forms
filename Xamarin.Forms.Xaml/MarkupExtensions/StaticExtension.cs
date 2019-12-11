using System;
using System.Linq;
using System.Reflection;
using System.Xml;
using Xamarin.Forms.Exceptions;

namespace Xamarin.Forms.Xaml
{
	[ContentProperty(nameof(Member))]
	[ProvideCompiled("Xamarin.Forms.Build.Tasks.StaticExtension")]
	public class StaticExtension : IMarkupExtension
	{
		public string Member { get; set; }

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
				throw new ArgumentNullException(nameof(serviceProvider));
			if (!(serviceProvider.GetService(typeof(IXamlTypeResolver)) is IXamlTypeResolver typeResolver))
				throw new ArgumentException("No IXamlTypeResolver in IServiceProvider");
			if (string.IsNullOrEmpty(Member) || !Member.Contains("."))
				throw new XFException(XFException.Ecode.Syntax, serviceProvider.GetLineInfo(), "x:Static", "[Member=][prefix:]typeName.staticMemberName");

			var dotIdx = Member.LastIndexOf('.');
			var typename = Member.Substring(0, dotIdx);
			var membername = Member.Substring(dotIdx + 1);

			var type = typeResolver.Resolve(typename, serviceProvider);

			var pinfo = type.GetRuntimeProperties().FirstOrDefault(pi => pi.Name == membername && pi.GetMethod.IsStatic);
			if (pinfo != null)
				return pinfo.GetMethod.Invoke(null, new object[] { });

			var finfo = type.GetRuntimeFields().FirstOrDefault(fi => fi.Name == membername && fi.IsStatic);
			if (finfo != null)
				return finfo.GetValue(null);

			throw new XFException(XFException.Ecode.Xstatic, serviceProvider.GetLineInfo(), membername, typename);
		}
	}
}