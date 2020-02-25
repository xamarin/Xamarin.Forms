using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal class PageContainer : UIView, IUIAccessibilityContainer
	{
		readonly IAccessibilityElementsController _parent;
		List<NSObject> _accessibilityElements = null;
		bool _disposed;

		public PageContainer(IAccessibilityElementsController parent)
		{
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			_parent = parent;
		}

		public PageContainer()
		{
			IsAccessibilityElement = false;
		}

		List<NSObject> DefaultOrder => this.Descendants().Select(i => i as NSObject).ToList();

		List<NSObject> AccessibilityElements
		{
			get
			{
				// lazy-loading this list so that the expensive call to GetAccessibilityElements only happens when VoiceOver is on.
				if (_accessibilityElements == null)
				{
					_accessibilityElements = _parent.GetAccessibilityElements() ?? DefaultOrder;
				}
				return _accessibilityElements;
			}
		}

		public void ClearAccessibilityElements()
		{
			_accessibilityElements = null;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				ClearAccessibilityElements();
				_disposed = true;
			}
			base.Dispose(disposing);
		}

		[Export("accessibilityElementCount")]
		nint AccessibilityElementCount()
		{
			if (AccessibilityElements == null)
				return 0;

			// Note: this will only be called when VoiceOver is enabled
			return AccessibilityElements.Count;
		}

		[Export("accessibilityElementAtIndex:")]
		NSObject GetAccessibilityElementAt(nint index)
		{
			if (AccessibilityElements == null)
				return NSNull.Null;

			// Note: this will only be called when VoiceOver is enabled
			return AccessibilityElements[(int)index];
		}

		[Export("indexOfAccessibilityElement:")]
		int GetIndexOfAccessibilityElement(NSObject element)
		{
			if (AccessibilityElements == null)
				return int.MaxValue;

			// Note: this will only be called when VoiceOver is enabled
			return AccessibilityElements.IndexOf(element);
		}
	}
}