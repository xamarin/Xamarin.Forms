using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	public class VisualElementPackager : IDisposable
	{
		readonly EventHandler<ElementEventArgs> _childAddedHandler;
		readonly EventHandler<ElementEventArgs> _childRemovedHandler;
		readonly EventHandler _childReorderedHandler;
		List<IVisualElementRenderer> _childViews;

		bool _disposed;

		IVisualElementRenderer _renderer;

		public VisualElementPackager(IVisualElementRenderer renderer)
		{
			if (renderer == null)
				throw new ArgumentNullException("renderer");

			_childAddedHandler = OnChildAdded;
			_childRemovedHandler = OnChildRemoved;
			_childReorderedHandler = OnChildrenReordered;

			_renderer = renderer;
			_renderer.ElementChanged += (sender, args) => SetElement(args.OldElement, args.NewElement);
		}

		IElementController ElementController => _renderer.Element as IElementController;

		public void Dispose()
		{
			if (_disposed)
				return;
			_disposed = true;

			if (_renderer != null)
			{
				if (_childViews != null)
				{
					_childViews.Clear();
					_childViews = null;
				}

				_renderer.Element.ChildAdded -= _childAddedHandler;
				_renderer.Element.ChildRemoved -= _childRemovedHandler;

				_renderer.Element.ChildrenReordered -= _childReorderedHandler;
				_renderer = null;
			}
		}

		public void Load()
		{
			SetElement(null, _renderer.Element);
		}

		void AddChild(VisualElement view, IVisualElementRenderer oldRenderer = null, RendererPool pool = null, bool sameChildren = false)
		{
			Performance.Start();

			if (_childViews == null)
				_childViews = new List<IVisualElementRenderer>();

			IVisualElementRenderer renderer = oldRenderer;
			if (pool != null)
				renderer = pool.GetFreeRenderer(view);
			if (renderer == null)
			{
				Performance.Start("New renderer");
				renderer = Platform.CreateRenderer(view);
				Performance.Stop("New renderer");
			}

			if (renderer == oldRenderer)
			{
				Platform.SetRenderer(renderer.Element, null);
				renderer.SetElement(view);
			}

			Performance.Start("Set renderer");
			Platform.SetRenderer(view, renderer);
			Performance.Stop("Set renderer");

			Performance.Start("Add view");
			if (!sameChildren)
			{
				_renderer.ViewGroup.AddView(renderer.ViewGroup);
				_childViews.Add(renderer);
			}
			Performance.Stop("Add view");

			Performance.Stop();
		}

		void EnsureChildOrder()
		{
			for (var i = 0; i < ElementController.LogicalChildren.Count; i++)
			{
				Element child = ElementController.LogicalChildren[i];
				var element = (VisualElement)child;
				if (element != null)
				{
					IVisualElementRenderer r = Platform.GetRenderer(element);
					_renderer.ViewGroup.BringChildToFront(r.ViewGroup);
				}
			}
		}

		void OnChildAdded(object sender, ElementEventArgs e)
		{
			var view = e.Element as VisualElement;
			if (view != null)
				AddChild(view);

			if (ElementController.LogicalChildren[ElementController.LogicalChildren.Count - 1] != view)
				EnsureChildOrder();
		}

		void OnChildRemoved(object sender, ElementEventArgs e)
		{
			Performance.Start();
			var view = e.Element as VisualElement;
			if (view != null)
				RemoveChild(view);

			Performance.Stop();
		}

		void OnChildrenReordered(object sender, EventArgs e)
		{
			EnsureChildOrder();
		}

		void RemoveChild(VisualElement view)
		{
			IVisualElementRenderer renderer = Platform.GetRenderer(view);
			_childViews.Remove(renderer);
			renderer.ViewGroup.RemoveFromParent();
			renderer.Dispose();
		}

		void SetElement(VisualElement oldElement, VisualElement newElement)
		{
			Performance.Start();

			var sameChildrenTypes = false;

			ReadOnlyCollection<Element> newChildren = null, oldChildren = null;

			RendererPool pool = null;
			if (oldElement != null)
			{
				if (newElement != null)
				{
					sameChildrenTypes = true;

					oldChildren = ((IElementController)oldElement).LogicalChildren;
					newChildren = ((IElementController)newElement).LogicalChildren;
					if (oldChildren.Count == newChildren.Count)
					{
						for (var i = 0; i < oldChildren.Count; i++)
						{
							if (oldChildren[i].GetType() != newChildren[i].GetType())
							{
								sameChildrenTypes = false;
								break;
							}
						}
					}
					else
						sameChildrenTypes = false;
				}

				oldElement.ChildAdded -= _childAddedHandler;
				oldElement.ChildRemoved -= _childRemovedHandler;

				oldElement.ChildrenReordered -= _childReorderedHandler;

				if (!sameChildrenTypes)
				{
					_childViews = new List<IVisualElementRenderer>();
					pool = new RendererPool(_renderer, oldElement);
					pool.ClearChildrenRenderers();
				}
			}

			if (newElement != null)
			{
				Performance.Start("Setup");
				newElement.ChildAdded += _childAddedHandler;
				newElement.ChildRemoved += _childRemovedHandler;

				newElement.ChildrenReordered += _childReorderedHandler;

				newChildren = newChildren ?? ((IElementController)newElement).LogicalChildren;

				for (var i = 0; i < newChildren.Count; i++)
				{
					IVisualElementRenderer oldRenderer = null;
					if (oldChildren != null && sameChildrenTypes)
						oldRenderer = _childViews[i];

					AddChild((VisualElement)newChildren[i], oldRenderer, pool, sameChildrenTypes);
				}

#if DEBUG
				//if (renderer.Element.LogicalChildren.Any() && renderer.ViewGroup.ChildCount != renderer.Element.LogicalChildren.Count)
				//	throw new InvalidOperationException ("SetElement did not create the correct number of children");
#endif
				Performance.Stop("Setup");
			}

			Performance.Stop();
		}
	}
}