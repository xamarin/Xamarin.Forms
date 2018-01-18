using System;
using Xamarin.Forms.Core;

namespace Xamarin.Forms
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class ExportRendererAttribute : BaseExportRendererAttribute
    {
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
}