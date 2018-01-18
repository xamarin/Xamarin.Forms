using System;
using Xamarin.Forms.Core;

#if __MOBILE__
using UIKit;
#else
using Foundation;
#endif

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
				{
#if __MOBILE__
					_majorVersion = Convert.ToInt32(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);
#else
					_majorVersion = Convert.ToInt32(NSProcessInfo.ProcessInfo.OperatingSystemVersionString.Split('.')[0]);
#endif
				}

				return _majorVersion;
			}
		}

#if __MOBILE__
		public ExportRendererAttribute(Type handler, Type target, UIUserInterfaceIdiom idiom) : base(handler, target)
		{
			Idiomatic = true;
			Idiom = idiom;
		}
		internal UIUserInterfaceIdiom Idiom { get; }
#endif

		public ExportRendererAttribute(Type handler, Type target) : base(handler, target)
		{
			Idiomatic = false;
		}

		internal bool Idiomatic { get; }

		public override bool ShouldRegister()
		{
			if (!base.ShouldRegister())
				return false;

#if __MOBILE__
			return !Idiomatic || Idiom == UIDevice.CurrentDevice.UserInterfaceIdiom;
#else
			return !Idiomatic;
#endif
		}
	}
}