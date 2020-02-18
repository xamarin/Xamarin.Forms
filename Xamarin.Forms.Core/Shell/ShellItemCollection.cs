using System.Collections;
using System.Collections.Specialized;

namespace Xamarin.Forms
{
	internal sealed class ShellItemCollection : ShellAbstractCollection<ShellItem>
	{
		public ShellItemCollection() : base() { }

		void OnShellItemControllerItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			foreach (ShellSection section in (e.NewItems ?? e.OldItems ?? (IList)Inner))
			{
				if (section.Parent == null)
					section.ParentSet += OnParentSet;
				else
					CheckVisibility(section.Parent as ShellItem);
			}

			void OnParentSet(object s, System.EventArgs __)
			{
				var shellSection = (ShellSection)s;
				shellSection.ParentSet -= OnParentSet;
				CheckVisibility(shellSection.Parent as ShellItem);
			}
		}

		protected override void CheckVisibility(ShellItem shellItem)
		{
			if (IsShellItemVisible(shellItem))
			{
				if (_visibleContents.Contains(shellItem))
					return;

				int visibleIndex = 0;
				for (var i = 0; i < Inner.Count; i++)
				{
					var item = Inner[i];

					if (!IsShellItemVisible(item))
						continue;

					if (item == shellItem)
					{
						_visibleContents.Insert(visibleIndex, shellItem);
						break;
					}

					visibleIndex++;
				}
			}
			else if (_visibleContents.Contains(shellItem))
			{
				_visibleContents.Remove(shellItem);
			}

			bool IsShellItemVisible(ShellItem item)
			{
				return (item is IShellItemController itemController && itemController.GetItems().Count > 0) ||
					item is IMenuItemController;
			}
		}
		
		public override void Add(ShellItem item)
		{
			/*
			 * This is purely for the case where a user is only specifying Tabs at the highest level
			 * <shell>
			 * <tab></tab>
			 * <tab></tab>
			 * </shell>
			 * */
			if (Routing.IsImplicit(item) &&
				item is TabBar
				)
			{
				int i = Count - 1;
				if (i >= 0 && this[i] is TabBar && Routing.IsImplicit(this[i]))
				{
					this[i].Items.Add(item.Items[0]);
					return;
				}
			}

			Inner.Add(item);
		}

		protected override void OnElementControllerInserting(IElementController element)
		{
			if (element is IShellItemController controller)
				controller.ItemsCollectionChanged += OnShellItemControllerItemsCollectionChanged;
		}

		protected override void OnElementControllerRemoving(IElementController element)
		{
			if (element is IShellSectionController controller)
				controller.ItemsCollectionChanged -= OnShellItemControllerItemsCollectionChanged;
		}
	}
}
