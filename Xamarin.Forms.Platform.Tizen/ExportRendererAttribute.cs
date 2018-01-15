using System;
using Xamarin.Forms.Core;
using DotnetUtil = Tizen.Common.DotnetUtil;

namespace Xamarin.Forms.Platform.Tizen
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class ExportRendererAttribute : BaseExportRendererAttribute
	{
		public ExportRendererAttribute(Type handler, Type target) : base(handler, target)
		{
			MajorVersion = DotnetUtil.TizenAPIVersion;
		}
	}
}