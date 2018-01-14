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
		public ExportRendererAttribute(Type handler, Type target, UIUserInterfaceIdiom idiom) : this(handler, target)
		{
			Idiomatic = true;
			Idiom = idiom;
		}
		internal UIUserInterfaceIdiom Idiom { get; }
#endif

		public ExportRendererAttribute(Type handler, Type target) : base(handler, target)
		{
			Idiomatic = false;
			MajorVersion = Convert.ToInt32(GetOSVersion().Split('.')[0]);
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

		string GetOSVersion()
		{
#if __MOBILE__
			return UIDevice.CurrentDevice.SystemVersion;
#else
			return NSProcessInfo.ProcessInfo.OperatingSystemVersionString;
#endif
		}
	}
}