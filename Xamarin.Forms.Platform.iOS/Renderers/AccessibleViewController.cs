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
			IDictionary<int, List<VisualElement>> tabIndexes = null;
			int childrenWithTabStopsLessOne = 0;
			VisualElement firstTabStop = null;
			List<NSObject> views = new List<NSObject>();
			foreach (var child in children)
			{
				if (!(child is VisualElement ve && Platform.GetRenderer(ve).NativeView is ITabStop tabStop))
				{
					continue;
				}
				if (tabIndexes == null)
				{
					tabIndexes = ve.GetTabIndexesOnParentPage(out childrenWithTabStopsLessOne);
					firstTabStop = GetFirstTabStopVisualElement(tabIndexes);
					break;
				}
			}

			if (firstTabStop == null || tabIndexes == null)
				return null;

			VisualElement nextVisualElement = firstTabStop;
			UIView nextControl = null;
			do
			{
				nextControl = (Platform.GetRenderer(nextVisualElement).NativeView as ITabStop)?.TabStop;

				if (views.Contains(nextControl))
					break; // we've looped to the beginning

				if (nextControl != null)
					views.Add(nextControl);

				nextVisualElement = GetNextTabStopVisualElement(nextVisualElement, forwardDirection: true,
																tabIndexes: tabIndexes,
																maxAttempts: childrenWithTabStopsLessOne);
			} while (nextVisualElement != null && nextVisualElement != firstTabStop);

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

		static VisualElement GetFirstTabStopVisualElement(IDictionary<int, List<VisualElement>> tabIndexes)
		{
			if (tabIndexes == null)
				return null;

			return TabIndexExtensions.GetFirstElementByTabIndex(tabIndexes);
		}

		static VisualElement GetNextTabStopVisualElement(VisualElement ve, bool forwardDirection, IDictionary<int, List<VisualElement>> tabIndexes, int maxAttempts)
		{
			if (maxAttempts <= 0 || tabIndexes == null || ve == null)
				return null;

			int tabIndex = ve.TabIndex;

			VisualElement nextElement = ve;
			UIView nextControl = null;
			int attempt = 0;
			do
			{
				nextElement = nextElement.FindNextElement(forwardDirection, tabIndexes, ref tabIndex);

				if (AutomationProperties.GetIsInAccessibleTree(nextElement) != false)
				{
					var renderer = Platform.GetRenderer(nextElement);
					nextControl = (renderer as ITabStop)?.TabStop;
				}

			} while (++attempt < maxAttempts && nextControl == null);
			return nextElement;
		}
	}
}