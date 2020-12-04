using System;

#if __MOBILE__
using UIKit;
using NativeView = UIKit.UIView;

namespace Xamarin.Forms.Platform.iOS
#else
using NativeView = AppKit.NSView;

namespace Xamarin.Forms.Platform.MacOS
#endif
{
	internal class TabIndexManager : IDisposable
	{
		VisualElement _element;
		bool _disposed;
		IVisualElementRenderer _renderer;

#if __MOBILE__
		public UIKeyCommand[] TabCommands = {
			UIKeyCommand.Create ((Foundation.NSString)"\t", 0, new ObjCRuntime.Selector ("tabForward:")),
			UIKeyCommand.Create ((Foundation.NSString)"\t", UIKeyModifierFlags.Shift, new ObjCRuntime.Selector ("tabBackward:"))
		};
#endif

		public TabIndexManager(IVisualElementRenderer renderer)
		{
			_renderer = renderer;
			_renderer.ElementChanged += RendererElementChanged;

			RendererElementChanged(this, new VisualElementChangedEventArgs(null, _renderer.Element));
		}

		public void UpdateParentPageAccessibilityElements()
		{
#if __MOBILE__
			var parentRenderer = _renderer.NativeView.Superview;
			while (parentRenderer != null)
			{
				if (parentRenderer is PageContainer container)
				{
					container.ClearAccessibilityElements();
					break;
				}

				parentRenderer = parentRenderer.Superview;
			}
#endif
		}

		public NativeView FocusSearch(bool forwardDirection)
		{
			int maxAttempts = 0;
			var tabIndexes = _element?.GetTabIndexesOnParentPage(out maxAttempts);
			if (tabIndexes == null)
				return null;

			int tabIndex = _element.TabIndex;
			int attempt = 0;

			do
			{
				_element = _element.FindNextElement(forwardDirection, tabIndexes, ref tabIndex) as VisualElement;
				if (_element == null)
					break;

#if __MACOS__
				var renderer = Platform.GetRenderer(_element);
				var control = (renderer as ITabStop)?.TabStop;
				if (control != null && control.AcceptsFirstResponder())
					return control;
#endif
				_element.Focus();
			} while (!(_element.IsFocused || ++attempt >= maxAttempts));
			return null;
		}

		public void UpdateTabStopAndIndex()
		{
			UpdateTabStop();
			UpdateTabIndex();
		}

		protected int TabIndex { get; set; } = 0;

		protected bool TabStop { get; set; } = true;
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (!disposing)
				return;

			if (_renderer != null)
				_renderer.ElementChanged -= RendererElementChanged;

			if (_element != null)
			{
				_element.FocusChangeRequested -= ViewOnFocusChangeRequested;
				_element.PropertyChanged -= ElementPropertyChanged;
			}

			_element = null;
			_renderer = null;
		}

		void UpdateTabStop()
		{
			if (_element == null)
				return;

			TabStop = _element.IsTabStop;
			UpdateParentPageAccessibilityElements();
		}

		void UpdateTabIndex()
		{
			if (_element == null)
				return;

			TabIndex = _element.TabIndex;
			UpdateParentPageAccessibilityElements();
		}


		void RendererElementChanged(object sender, VisualElementChangedEventArgs e)
		{
			if (_element != null)
			{
				_element.FocusChangeRequested -= ViewOnFocusChangeRequested;
				_element.PropertyChanged += ElementPropertyChanged;
			}

			if (e.NewElement != null)
			{
				_element = e.NewElement;
				_element.FocusChangeRequested += ViewOnFocusChangeRequested;
				_element.PropertyChanged += ElementPropertyChanged;
			}

			UpdateTabStopAndIndex();
		}

		void ElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == VisualElement.IsTabStopProperty.PropertyName)
				UpdateTabStop();
			else if (e.PropertyName == VisualElement.TabIndexProperty.PropertyName)
				UpdateTabIndex();
		}

		void ViewOnFocusChangeRequested(object sender, VisualElement.FocusRequestArgs focusRequestArgs)
		{
			focusRequestArgs.Result = focusRequestArgs.Focus ? _renderer.NativeView.BecomeFirstResponder() : _renderer.NativeView.ResignFirstResponder();
		}
	}
}
