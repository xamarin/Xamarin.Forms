using System;
using Windows.System.Profile;
using Xamarin.Forms.Core;

namespace Xamarin.Forms.Platform.UWP
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class ExportRendererAttribute : BaseExportRendererAttribute
	{
		public ExportRendererAttribute(Type handler, Type target) : base(handler, target)
		{
		}

		public override int GetMajorVersion()
		{
			string deviceFamilyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
			ulong version = ulong.Parse(deviceFamilyVersion);
			ulong major = (version & 0xFFFF000000000000L) >> 48;
			return (int)major;
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