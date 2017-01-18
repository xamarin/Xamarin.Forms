using System;
using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{
	public class VisualElementPackager : IDisposable
	{
		VisualElement _element;

		bool _isDisposed;

		public VisualElementPackager(IVisualElementRenderer renderer)
		{
			if (renderer == null)
				throw new ArgumentNullException(nameof(renderer));

			Renderer = renderer;
			renderer.ElementChanged += OnRendererElementChanged;
			SetElement(null, renderer.Element);
		}

		protected IVisualElementRenderer Renderer { get; set; }

		IElementController ElementController => Renderer.Element;

		public void Dispose()
		{
			Dispose(true);
		}

		public void Load()
		{
			foreach (Element element in ElementController.LogicalChildren)
			{
				var child = element as VisualElement;
				if (child != null)
					OnChildAdded(child);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			if (disposing)
			{
				SetElement(_element, null);
				if (Renderer != null)
				{
					Renderer.ElementChanged -= OnRendererElementChanged;
					Renderer = null;
				}
			}

			_isDisposed = true;
		}

		protected virtual void OnChildAdded(VisualElement view)
		{
			if (_isDisposed)
				return;

			IVisualElementRenderer viewRenderer = Platform.CreateRenderer(view);
			Platform.SetRenderer(view, viewRenderer);

			NSView uiview = Renderer.NativeView;
			uiview.AddSubview(viewRenderer.NativeView);

			if (Renderer.ViewController != null && viewRenderer.ViewController != null)
				Renderer.ViewController.AddChildViewController(viewRenderer.ViewController);

			EnsureChildrenOrder();
		}

		protected virtual void OnChildRemoved(VisualElement view)
		{
			IVisualElementRenderer viewRenderer = Platform.GetRenderer(view);
			if (viewRenderer?.NativeView == null)
				return;

			viewRenderer.NativeView.RemoveFromSuperview();

			if (Renderer.ViewController != null)
				viewRenderer.ViewController?.RemoveFromParentViewController();
		}

		void EnsureChildrenOrder()
		{
			if (ElementController.LogicalChildren.Count == 0)
				return;

			for (var z = 0; z < ElementController.LogicalChildren.Count; z++)
			{
				var child = ElementController.LogicalChildren[z] as VisualElement;
				if (child == null)
					continue;
				IVisualElementRenderer childRenderer = Platform.GetRenderer(child);

				if (childRenderer == null)
					continue;

				NSView nativeControl = childRenderer.NativeView;

				nativeControl.RemoveFromSuperview();
				Renderer.NativeView.AddSubview(nativeControl, NSWindowOrderingMode.Above, null);
				nativeControl.Layer.ZPosition = z * 1000;
			}
		}

		void OnChildAdded(object sender, ElementEventArgs e)
		{
			var view = e.Element as VisualElement;
			if (view != null)
				OnChildAdded(view);
		}

		void OnChildRemoved(object sender, ElementEventArgs e)
		{
			var view = e.Element as VisualElement;
			if (view != null)
				OnChildRemoved(view);
		}

		void OnRendererElementChanged(object sender, VisualElementChangedEventArgs args)
		{
			if (args.NewElement == _element)
				return;

			SetElement(_element, args.NewElement);
		}

		void SetElement(VisualElement oldElement, VisualElement newElement)
		{
			if (oldElement == newElement)
				return;

			if (oldElement != null)
			{
				oldElement.ChildAdded -= OnChildAdded;
				oldElement.ChildRemoved -= OnChildRemoved;
				oldElement.ChildrenReordered -= UpdateChildrenOrder;

				if (newElement != null)
				{
					var pool = new RendererPool(Renderer, oldElement);
					pool.UpdateNewElement(newElement);

					EnsureChildrenOrder();
				}
				else
				{
					var elementController = (IElementController)oldElement;

					foreach (Element element in elementController.LogicalChildren)
					{
						var child = element as VisualElement;
						if (child != null)
							OnChildRemoved(child);
					}
				}
			}

			_element = newElement;

			if (newElement == null) return;
			newElement.ChildAdded += OnChildAdded;
			newElement.ChildRemoved += OnChildRemoved;
			newElement.ChildrenReordered += UpdateChildrenOrder;
		}

		void UpdateChildrenOrder(object sender, EventArgs e)
		{
			EnsureChildrenOrder();
		}
	}
}