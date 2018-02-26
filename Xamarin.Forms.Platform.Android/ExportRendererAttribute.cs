using Android.OS;
using System;
using Xamarin.Forms.Core;

namespace Xamarin.Forms
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
					_majorVersion = (int)Build.VERSION.SdkInt;

				return _majorVersion;
			}
		}

		public ExportRendererAttribute(Type handler, Type target) : base(handler, target)
		{
		}
	}
}