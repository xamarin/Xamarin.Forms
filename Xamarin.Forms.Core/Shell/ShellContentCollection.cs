using System;

namespace Xamarin.Forms
{
	internal sealed class ShellContentCollection : ShellAbstractCollection<ShellContent>
	{
		public ShellContentCollection() : base() { }

		void OnIsPageVisibleChanged(object sender, EventArgs e)
		{
			CheckVisibility((ShellContent)sender);
		}

		protected override void CheckVisibility(ShellContent shellContent)
		{
			if (shellContent is IShellContentController controller)
			{
				// Assume incoming page will be visible
				if (controller.Page == null || controller.Page.IsVisible)
				{
					if (_visibleContents.Contains(shellContent))
						return;

					int visibleIndex = 0;
					for (var i = 0; i < _inner.Count; i++)
					{
						var item = _inner[i];

						if (item == shellContent)
						{
							_visibleContents.Insert(visibleIndex, shellContent);
							break;
						}

						visibleIndex++;
					}
				}
				else
				{
					_visibleContents.Remove(shellContent);
				}
			}
			else if (_visibleContents.Contains(shellContent))
			{
				_visibleContents.Remove(shellContent);
			}
		}

		protected override void OnElementControllerInserting(IElementController element)
		{
			if (element is IShellContentController controller)
				controller.IsPageVisibleChanged += OnIsPageVisibleChanged;
		}

		protected override void OnElementControllerRemoving(IElementController element)
		{
			if (element is IShellContentController controller)
				controller.IsPageVisibleChanged -= OnIsPageVisibleChanged;
		}
	}
}