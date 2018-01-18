using System;
using Xamarin.Forms.Core;
using DotnetUtil = Tizen.Common.DotnetUtil;

namespace Xamarin.Forms.Platform.Tizen
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class ExportRendererAttribute : BaseExportRendererAttribute
	{
		protected override int? MajorVersion
		{
			get
			{
				if (_majorVersion == null)
					_majorVersion = DotnetUtil.TizenAPIVersion;

				return _majorVersion;
			}
		}

		public ExportRendererAttribute(Type handler, Type target) : base(handler, target)
		{
		}
	}
}