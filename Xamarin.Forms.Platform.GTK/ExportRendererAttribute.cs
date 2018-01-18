using System;
using Xamarin.Forms.Core;

namespace Xamarin.Forms
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class ExportRendererAttribute : BaseExportRendererAttribute
    {
        public ExportRendererAttribute(Type handler, Type target) : base(handler, target)
        {
        }

		public override int GetMajorVersion()
		{
			return Environment.OSVersion.Version.Major;
		}
	}
}