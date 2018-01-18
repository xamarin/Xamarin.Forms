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

		public override int GetMajorVersion()
		{
#if __MOBILE__
			return Convert.ToInt32(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);
#else
			return Convert.ToInt32(NSProcessInfo.ProcessInfo.OperatingSystemVersionString.Split('.')[0]);
#endif
		}
	}
}