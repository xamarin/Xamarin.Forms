using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ElmSharp;
using EToolbarItem = ElmSharp.ToolbarItem;
using EToolbarItemEventArgs = ElmSharp.ToolbarItemEventArgs;

using Xamarin.Forms.PlatformConfiguration.TizenSpecific;

namespace Xamarin.Forms.Platform.Tizen
{
	public class TabbedPageRenderer : VisualElementRenderer<TabbedPage>, IVisualElementRenderer
	{
		Box _box;
		Toolbar _tpage;
		EvasObject _tcontent;
		Dictionary<EToolbarItem, Page> _itemToItemPage = new Dictionary<EToolbarItem, Page>();

		public TabbedPageRenderer()
		{
			//Register for title change property
			RegisterPropertyHandler(TabbedPage.TitleProperty, UpdateTitle);
			//Register for current page change property
			RegisterPropertyHandler("CurrentPage", CurrentPageChanged);
			//TODO renderer should add item on EFL toolbar when new Page is added to TabbedPage
		}

		protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
		{
			if (_tpage == null)
			{
				//Create box that holds toolbar and selected content
				_box = new Box(Forms.Context.MainWindow)
				{
					AlignmentX = -1,
					AlignmentY = -1,
					WeightX = 1,
					WeightY = 1,
					IsHorizontal = false,
				};
				_box.Show();

				//Create toolbar that is placed inside the _box
				_tpage = new Toolbar(Forms.Context.MainWindow)
				{
					AlignmentX = -1,
					WeightX = 1,
					SelectionMode = ToolbarSelectionMode.Always,
					Style = "tabbar_with_title"
				};
				_tpage.Show();
				//Add callback for item selection
				_tpage.Selected += OnCurrentPageChanged;
				_box.PackEnd(_tpage);

				SetNativeControl(_box);
				UpdateTitle();
			}

			base.OnElementChanged(e);
		}

		protected override void Dispose(bool disposing)
		{
			if (_box != null)
			{
				_box.Unrealize();
				_box = null;
			}
			if (_tpage != null)
			{
				_tpage.Selected -= OnCurrentPageChanged;

				_tpage.Unrealize();
				_tpage = null;
			}
			base.Dispose(disposing);
		}

		protected override void OnElementReady()
		{
			FillToolbar();
			base.OnElementReady();
		}

		protected override void UpdateThemeStyle()
		{
			_tpage.Style = Element.OnThisPlatform().GetStyle();
			((IVisualElementController)Element).NativeSizeChanged();
		}

		void UpdateTitle()
		{
			_tpage.Text = Element.Title;
		}

		void UpdateTitle(Page page)
		{
			if (_itemToItemPage.ContainsValue(page))
			{
				var pair = _itemToItemPage.FirstOrDefault(x => x.Value == page);
				pair.Key.SetPartText(null, pair.Value.Title);
			}
		}

		void OnPageTitleChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Page.TitleProperty.PropertyName)
			{
				UpdateTitle(sender as Page);
			}
		}

		void FillToolbar()
		{
			var logicalChildren = (Element as IElementController).LogicalChildren;

			//add items to toolbar
			foreach (Page child in logicalChildren)
			{
				var childRenderer = Platform.GetRenderer(child);
				if (childRenderer != null)
				{
					childRenderer.NativeView.Hide();
				}

				EToolbarItem toolbarItem = _tpage.Append(child.Title, string.IsNullOrEmpty(child.Icon) ? null : ResourcePath.GetPath(child.Icon));

				_itemToItemPage.Add(toolbarItem, child);
				if (Element.CurrentPage == child)
				{
					//select item on the toolbar and fill content
					toolbarItem.IsSelected = true;
					OnCurrentPageChanged(null, null);
				}
				child.PropertyChanged += OnPageTitleChanged;
			}
		}

		void OnCurrentPageChanged(object sender, EToolbarItemEventArgs e)
		{
			if (_tpage.SelectedItem == null)
				return;
			//detach content from view without EvasObject changes
			if (_tcontent != null)
			{
				//hide content that should not be visible
				_tcontent.Hide();
				//unpack content that is hiden an prepare for new content
				_box.UnPack(_tcontent);
				(Element.CurrentPage as IPageController)?.SendDisappearing();
			}
			Element.CurrentPage = _itemToItemPage[_tpage.SelectedItem];

			//create EvasObject using renderer and remember to not destroy
			//it for better performance (create once)
			_tcontent = Platform.GetOrCreateRenderer(Element.CurrentPage).NativeView;
			_tcontent.SetAlignment(-1, -1);
			_tcontent.SetWeight(1, 1);
			_tcontent.Show();
			_box.PackEnd(_tcontent);
			(Element.CurrentPage as IPageController)?.SendAppearing();
		}

		void CurrentPageChanged()
		{
			foreach (KeyValuePair<EToolbarItem, Page> pair in _itemToItemPage)
			{
				if (pair.Value == Element.CurrentPage)
				{
					pair.Key.IsSelected = true;
					return;
				}
			}
		}
	}
}
