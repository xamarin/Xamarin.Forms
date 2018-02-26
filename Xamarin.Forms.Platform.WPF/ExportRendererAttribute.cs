using System;
using Xamarin.Forms.Core;

namespace Xamarin.Forms.Platform.WPF
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class ExportRendererAttribute : BaseExportRendererAttribute
	{
		int? _majorVersion;

		protected override int? MajorVersion
		{
			get
			{
				if (_majorVersion == null)
					_majorVersion = Environment.OSVersion.Version.Major;

				return _majorVersion;
			}
		}

		public ExportRendererAttribute(Type handler, Type target) : base(handler, target)
		{
		}
	}

	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class ExportCellAttribute : HandlerAttribute
	{
		public ExportCellAttribute(Type handler, Type target) : base(handler, target)
		{
		}
	}

	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class ExportImageSourceHandlerAttribute : HandlerAttribute
	{
		public ExportImageSourceHandlerAttribute(Type handler, Type target) : base(handler, target)
		{
		}
	}
}