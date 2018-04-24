using System;
using System.Diagnostics.Contracts;
using System.Linq;

using UIKit;

using Xamarin.Forms.Platform.iOS;

namespace Xamarin.Forms
{
	public class ViewControllerSegueTarget : SegueTarget
	{
		internal static void Init()
		{
			SegueTarget.CreateFromObjectHandler = obj => new ViewControllerSegueTarget(obj);
		}

		public UIViewController ViewController {
			get {
				if (IsTemplate)
					throw new InvalidOperationException("Target is a template");
				return (UIViewController)TryCreateValue(typeof(UIViewController));
			}
		}

		protected ViewControllerSegueTarget(object obj) : base(obj)
		{
		}

		public ViewControllerSegueTarget(UIViewController viewController)
			: base (viewController)
		{
			if (viewController == null)
				throw new ArgumentNullException();
		}

		public override object TryCreateValue(Type targetType)
		{
			var result = base.TryCreateValue(targetType);
			if (result == null)
			{
				if (targetType.IsAssignableFrom(typeof(Page)))
				{
					var vc = (UIViewController)TryCreateValue(typeof(UIViewController));
					result = GetPage(vc);
					if (result == null)
						Reject(vc);
				}
				else if (targetType.IsAssignableFrom(typeof(UIViewController)))
				{
					var page = (Page)base.TryCreateValue(typeof(Page));
					if (page == null)
						return null;

					var renderer = Platform.iOS.Platform.GetRenderer(page);
					if (renderer == null)
					{
						renderer = Platform.iOS.Platform.CreateRenderer(page);
						Platform.iOS.Platform.SetRenderer(page, renderer);
					}
					return renderer.ViewController;
				}
			}
			return result;
		}

		internal static Page GetPage(UIViewController vc)
		{
			if (vc == null)
				return null;
			return ((vc as IVisualElementRenderer)?.Element as Page) ?? GetPage(vc.ChildViewControllers.FirstOrDefault());
		}
	}
}
