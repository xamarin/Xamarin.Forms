using System;
using Android.OS;

namespace Xamarin.Forms
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class ExportRendererAttribute : BaseExportRendererAttribute
	{
		public ExportRendererAttribute(Type handler, Type target) : base(handler, target)
		{
			MajorVersion = (int)Build.VERSION.SdkInt;
		}
	}
}