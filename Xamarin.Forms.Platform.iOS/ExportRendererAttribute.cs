using System;
using UIKit;

namespace Xamarin.Forms
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class ExportRendererAttribute : BaseExportRendererAttribute
	{
		public ExportRendererAttribute(Type handler, Type target, UIUserInterfaceIdiom idiom) : this(handler, target)
		{
			Idiomatic = true;
			Idiom = idiom;
		}

		public ExportRendererAttribute(Type handler, Type target) : base(handler, target)
		{
			MajorVersion = Convert.ToInt32(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);
		}

		internal UIUserInterfaceIdiom Idiom { get; }

		internal bool Idiomatic { get; }

		public override bool ShouldRegister()
		{
			if (!(!Idiomatic || Idiom == UIDevice.CurrentDevice.UserInterfaceIdiom))
				return false;

			return base.ShouldRegister();
		}
	}
}