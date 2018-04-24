using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Xamarin.Forms.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class NavigationProxy : INavigation
	{
		INavigation _inner;
		Lazy<List<Page>> _modalStack = new Lazy<List<Page>>(() => new List<Page>());

		Lazy<List<Page>> _pushStack = new Lazy<List<Page>>(() => new List<Page>());

		public INavigation Inner
		{
			get { return _inner; }
			set
			{
				if (_inner == value)
					return;
				_inner = value;
				// reverse so that things go into the new stack in the same order
				// null out to release memory that will likely never be needed again

				if (ReferenceEquals(_inner, null))
				{
					_pushStack = new Lazy<List<Page>>(() => new List<Page>());
					_modalStack = new Lazy<List<Page>>(() => new List<Page>());
				}
				else
				{
					if (_pushStack != null && _pushStack.IsValueCreated)
					{
						foreach (Page page in _pushStack.Value)
						{
							_inner.PushAsync(page);
						}
					}

					if (_modalStack != null && _modalStack.IsValueCreated)
					{
						foreach (Page page in _modalStack.Value)
						{
							_inner.PushModalAsync(page);
						}
					}

					_pushStack = null;
					_modalStack = null;
				}
			}
		}

		public void InsertPageBefore(Page page, Page before)
		{
			OnInsertPageBefore(page, before);
		}

		public IReadOnlyList<Page> ModalStack
		{
			get { return GetModalStack(); }
		}

		public IReadOnlyList<Page> NavigationStack
		{
			get { return GetNavigationStack(); }
		}

		public Task<Page> PopAsync()
		{
			return PopAsync(true);
		}

		public Task<Page> PopAsync(bool animated)
		{
			return (Task<Page>)OnSegue(new ValueSegue(NavigationAction.PopPushed, animated), null);
		}

		public Task<Page> PopModalAsync()
		{
			return PopModalAsync(true);
		}

		public Task<Page> PopModalAsync(bool animated)
		{
			return (Task<Page>)OnSegue(new ValueSegue(NavigationAction.PopModal, animated), null);
		}

		public Task PopToRootAsync()
		{
			return PopToRootAsync(true);
		}

		public Task PopToRootAsync(bool animated)
		{
			return OnSegue(new ValueSegue(NavigationAction.PopToRoot, animated), null);
		}

		public Task PushAsync(Page root)
		{
			return PushAsync(root, true);
		}

		public Task PushAsync(Page root, bool animated)
		{
			if (root.RealParent != null)
				throw new InvalidOperationException("Page must not already have a parent.");
			return OnSegue(new ValueSegue(NavigationAction.Push, animated), (SegueTarget)root);
		}

		public Task PushModalAsync(Page modal)
		{
			return PushModalAsync(modal, true);
		}

		public Task PushModalAsync(Page modal, bool animated)
		{
			if (modal.RealParent != null)
				throw new InvalidOperationException("Page must not already have a parent.");
			return OnSegue(new ValueSegue(NavigationAction.Modal, animated), (SegueTarget)modal);
		}

		public Task ShowAsync(Page page)
		{
			return ShowAsync(page, true);
		}

		public Task ShowAsync(Page page, bool animated)
		{
			if (page.RealParent != null)
				throw new InvalidOperationException("Page must not already have a parent.");
			return OnSegue(new ValueSegue(NavigationAction.Show, animated), (SegueTarget)page);
		}

		public Task SegueAsync(Segue segue, SegueTarget target)
		{
			return OnSegue(segue, target);
		}

		public void RemovePage(Page page)
		{
			OnRemovePage(page);
		}

		protected virtual IReadOnlyList<Page> GetModalStack()
		{
			INavigation currentInner = Inner;
			return currentInner == null ? _modalStack.Value : currentInner.ModalStack;
		}

		protected virtual IReadOnlyList<Page> GetNavigationStack()
		{
			INavigation currentInner = Inner;
			return currentInner == null ? _pushStack.Value : currentInner.NavigationStack;
		}

		protected virtual void OnInsertPageBefore(Page page, Page before)
		{
			INavigation currentInner = Inner;
			if (currentInner == null)
			{
				int index = _pushStack.Value.IndexOf(before);
				if (index == -1)
					throw new ArgumentException("before must be in the pushed stack of the current context");
				_pushStack.Value.Insert(index, page);
			}
			else
			{
				currentInner.InsertPageBefore(page, before);
			}
		}

		protected internal virtual Task OnSegue(ValueSegue segue, SegueTarget target)
		{
			INavigation currentInner = Inner;
			if (currentInner == null)
			{
				Page page = null;
				if (segue.Action.RequiresTarget())
				{
					if (target == null)
						throw new ArgumentNullException(nameof(target));
					page = (Page)target.TryCreatePage();
					if (page == null)
						throw new InvalidOperationException($"Navigation must be rooted for non-Page target");
				}
				switch (segue.Action)
				{
					case NavigationAction.Show:
						throw new InvalidOperationException($"Navigation must be rooted to use {nameof(NavigationAction.Show)}");

					case NavigationAction.Push:
						_pushStack.Value.Add(page);
						return Task.CompletedTask;
					case NavigationAction.Modal:
						_modalStack.Value.Add(page);
						return Task.CompletedTask;

					// It's important these Pop* cases (except PopToRoot) return Task<Page>
					case NavigationAction.Pop:
						return Task.FromResult(this.ShouldPopModal() ? PopModal() : Pop());
					case NavigationAction.PopPushed:
						return Task.FromResult(Pop());
					case NavigationAction.PopModal:
						return Task.FromResult(PopModal());

					case NavigationAction.PopToRoot:
						Page root = _pushStack.Value.Last();
						_pushStack.Value.Clear();
						_pushStack.Value.Add(root);
						return Task.CompletedTask;
				}
				return this.NavigateAsync(segue.Action, page, segue.IsAnimated);
			}
			return currentInner.SegueAsync(segue, target);
		}

		protected virtual void OnRemovePage(Page page)
		{
			INavigation currentInner = Inner;
			if (currentInner == null)
			{
				_pushStack.Value.Remove(page);
			}
			else
			{
				currentInner.RemovePage(page);
			}
		}

		Page Pop()
		{
			List<Page> list = _pushStack.Value;
			Page result = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			return result;
		}

		Page PopModal()
		{
			List<Page> list = _modalStack.Value;
			Page result = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			return result;
		}
	}
}