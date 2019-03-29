using Foundation;
using System;
using System.Collections.Generic;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class AccessibleUIViewController : UIViewController, IAccessibilityElementsController
	{
		bool _disposed;

		VisualElement _element;

		public PageContainer Container { get; private set; }
		IAccessibilityElementsController AccessibilityElementsController => this;

		public List<NSObject> GetAccessibilityElements()
		{
			if (Container == null || _element == null)
				return null;

			var children = _element.Descendants();
			SortedDictionary<int, List<VisualElement>> tabIndexes = null;
			List<NSObject> views = new List<NSObject>();
			foreach (var child in children)
			{
				if (!(child is VisualElement ve))
					continue;

				tabIndexes = ve.GetSortedTabIndexesOnParentPage(out _);
				break;
			}

			if (tabIndexes == null)
				return null;

			foreach (var idx in tabIndexes?.Keys)
			{
				var tabGroup = tabIndexes[idx];
				foreach (var child in tabGroup)
				{
					if (child is Layout ||
						!(
							child is VisualElement ve && ve.IsTabStop
							&& AutomationProperties.GetIsInAccessibleTree(ve) != false // accessible == true
							&& ve.GetRenderer().NativeView is ITabStop tabStop)
						 )
						continue;

					var thisControl = tabStop.TabStop;

					if (thisControl == null)
						continue;

					if (views.Contains(thisControl))
						break; // we've looped to the beginning

					views.Add(thisControl);
				}
			}

			return views;
		}

		void IAccessibilityElementsController.ResetAccessibilityElements()
		{
			Container?.ClearAccessibilityElements();
		}

		public virtual void SetElement(VisualElement element)
		{
			_element = element;

			ResetContainer();
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			AccessibilityElementsController?.ResetAccessibilityElements();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				Container?.Dispose();
				Container = null;

				_disposed = true;
			}

			base.Dispose(disposing);
		}

		void ResetContainer()
		{
			Container?.RemoveFromSuperview();

			Container = new PageContainer(this);
			View?.AddSubview(Container);
		}
	}
}