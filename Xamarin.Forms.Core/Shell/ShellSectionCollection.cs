using System.Collections;
using System.Collections.Specialized;

namespace Xamarin.Forms
{
	internal sealed class ShellSectionCollection : ShellAbstractCollection<ShellSection>
	{
		public ShellSectionCollection() : base() {}

		void OnShellSectionControllerItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			foreach (ShellContent content in (e.NewItems ?? e.OldItems ?? (IList)Inner))
			{
				if(content.Parent == null)
					content.ParentSet += OnParentSet;
				else	
					CheckVisibility(content.Parent as ShellSection);
			}

			void OnParentSet(object s, System.EventArgs __)
			{
				var shellContent = (ShellContent)s;
				shellContent.ParentSet -= OnParentSet;
				CheckVisibility(shellContent.Parent as ShellSection);
			}
		}

		protected override void CheckVisibility(ShellSection section)
		{
			if (section is IShellSectionController controller && controller.GetItems().Count > 0)
			{
				if (_visibleContents.Contains(section))
					return;

				int visibleIndex = 0;
				for (var i = 0; i < Inner.Count; i++)
				{
					var item = Inner[i];

					if (item == section)
					{
						_visibleContents.Insert(visibleIndex, section);
						break;
					}

					visibleIndex++;
				}
			}
			else if (_visibleContents.Contains(section))
			{
				_visibleContents.Remove(section);
			}
		}

		protected override void OnElementControllerInserting(IElementController element)
		{
			if (element is IShellSectionController controller)
				controller.ItemsCollectionChanged += OnShellSectionControllerItemsCollectionChanged;
		}

		protected override void OnElementControllerRemoving(IElementController element)
		{
			if (element is IShellSectionController controller)
				controller.ItemsCollectionChanged -= OnShellSectionControllerItemsCollectionChanged;
		}
	}
}