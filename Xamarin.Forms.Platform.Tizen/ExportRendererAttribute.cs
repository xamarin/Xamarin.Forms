using System;
using Xamarin.Forms.Core;
using Information = Tizen.System.Information;

namespace Xamarin.Forms.Platform.Tizen
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class ExportRendererAttribute : BaseExportRendererAttribute
	{
		public ExportRendererAttribute(Type handler, Type target) : base(handler, target)
		{
			if(Information.TryGetValue("tizen.org/feature/platform.version", out string OSVersion))
				MajorVersion = Convert.ToInt32(OSVersion.Split('.')[0]);
		}
	}
}