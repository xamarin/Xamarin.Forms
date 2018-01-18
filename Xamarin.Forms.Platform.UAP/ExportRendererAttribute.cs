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
			ulong minor = (version & 0x0000FFFF00000000L) >> 32;
			ulong build = (version & 0x00000000FFFF0000L) >> 16;
			ulong revision = version & 0x000000000000FFFFL;
			return Convert.ToInt32($"{major}.{minor}.{build}.{revision}".Split('.')[0]);
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