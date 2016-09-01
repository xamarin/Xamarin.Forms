using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using AppKit;
using Xamarin.Forms.Internals;
using CoreImage;

namespace Xamarin.Forms.Platform.MacOS
{
	public class TabbedPageRenderer : NSTabViewController, IVisualElementRenderer, IEffectControlProvider
	{
		bool _disposed;
		bool _updatingControllers;
		bool _barBackgroundColorWasSet;
		bool _barTextColorWasSet;
		bool _defaultBarTextColorSet;
		bool _defaultBarColorSet;
		NSColor _defaultBarTextColor;
		NSColor _defaultBarColor;

		public VisualElement Element { get; private set; }

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return NativeView.GetSizeRequest(widthConstraint, heightConstraint);
		}

		public NSView NativeView
		{
			get { return View; }
		}

		public void SetElement(VisualElement element)
		{
			var oldElement = Element;
			Element = element;

			OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));

			OnPagesChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));



			TabView.DrawsBackground = false;
			TabStyle = NSTabViewControllerTabStyle.Toolbar;
			TabView.TabViewType = NSTabViewType.NSNoTabsNoBorder;

			Tabbed.PropertyChanged += OnPropertyChanged;
			Tabbed.PagesChanged += OnPagesChanged;

			UpdateBarBackgroundColor();

			UpdateBarTextColor();

			EffectUtilities.RegisterEffectControlProvider(this, oldElement, element);
		}

		IPageController PageController => Element as IPageController;

		IElementController ElementController => Element as IElementController;

		void IEffectControlProvider.RegisterEffect(Effect effect)
		{
			var platformEffect = effect as PlatformEffect;
			if (platformEffect != null)
				platformEffect.Container = View;
		}

		public void SetElementSize(Size size)
		{
			Element.Layout(new Rectangle(Element.X, Element.Y, size.Width, size.Height));
		}

		public NSViewController ViewController
		{
			get { return this; }
		}

		public override nint SelectedTabViewItemIndex
		{
			get
			{
				return base.SelectedTabViewItemIndex;
			}
			set
			{
				base.SelectedTabViewItemIndex = value;
				if (!_updatingControllers)
					UpdateCurrentPage();
			}
		}

		public override void ViewDidAppear()
		{
			PageController.SendAppearing();
			base.ViewDidAppear();
		}

		public override void ViewDidDisappear()
		{
			base.ViewDidDisappear();
			PageController.SendDisappearing();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				_disposed = true;
				PageController.SendDisappearing();
				Tabbed.PropertyChanged -= OnPropertyChanged;
				Tabbed.PagesChanged -= OnPagesChanged;
			}

			base.Dispose(disposing);
		}

		protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
		{
			var changed = ElementChanged;
			if (changed != null)
				changed(this, e);
		}

		protected TabbedPage Tabbed
		{
			get { return (TabbedPage)Element; }
		}

		void OnPagePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Page.TitleProperty.PropertyName)
			{
				var page = (Page)sender;
				var index = TabbedPage.GetIndex(page);
				TabViewItems[index].Label = page.Title;
			}
			else if (e.PropertyName == Page.IconProperty.PropertyName)
			{
				var page = (Page)sender;

				var index = TabbedPage.GetIndex(page);
				TabViewItems[index].Label = page.Title;

				if (!string.IsNullOrEmpty(page.Icon))
				{

					TabViewItems[index].Image = new NSImage(page.Icon);
				}
				else if (TabViewItems[index].Image != null)
				{
					TabViewItems[index].Image = new NSImage();
				}

			}
		}

		void OnPagesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			e.Apply((o, i, c) => SetupPage((Page)o, i), (o, i) => TeardownPage((Page)o, i), Reset);

			SetControllers();

			UpdateChildrenOrderIndex();

			SetSelectedTabViewItem();
		}

		void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(TabbedPage.CurrentPage))
			{
				var current = Tabbed.CurrentPage;
				if (current == null)
					return;

				SetSelectedTabViewItem();
			}
			else if (e.PropertyName == TabbedPage.BarBackgroundColorProperty.PropertyName)
				UpdateBarBackgroundColor();
			else if (e.PropertyName == TabbedPage.BarTextColorProperty.PropertyName)
				UpdateBarTextColor();
		}

		void Reset()
		{
			var i = 0;
			foreach (var page in Tabbed.Children)
				SetupPage(page, i++);
		}

		void SetControllers()
		{
			_updatingControllers = true;
			for (var i = 0; i < ElementController.LogicalChildren.Count; i++)
			{
				var child = ElementController.LogicalChildren[i];
				var page = child as Page;
				if (page == null)
					continue;

				var pageRenderer = Platform.GetRenderer(page);
				if (pageRenderer != null)
				{
					pageRenderer.ViewController.Identifier = i.ToString();
					var newTVI = new NSTabViewItem { ViewController = pageRenderer.ViewController, Color = NSColor.Purple };
					if (page.Icon != null)
						newTVI.Image = new NSImage(page.Icon);

					AddTabViewItem(newTVI);
				}
			}
			_updatingControllers = false;
		}

		void SetupPage(Page page, int index)
		{
			var renderer = Platform.GetRenderer(page);
			if (renderer == null)
			{
				renderer = Platform.CreateRenderer(page);
				Platform.SetRenderer(page, renderer);
			}

			renderer.ViewController.Identifier = index.ToString();

			page.PropertyChanged += OnPagePropertyChanged;

		}

		void TeardownPage(Page page, int index)
		{
			page.PropertyChanged -= OnPagePropertyChanged;

			Platform.SetRenderer(page, null);
		}

		void SetSelectedTabViewItem()
		{
			if (Tabbed.CurrentPage == null)
				return;
			var selectedIndex = TabbedPage.GetIndex(Tabbed.CurrentPage);
			SelectedTabViewItemIndex = selectedIndex;
		}

		void UpdateBarBackgroundColor()
		{
			if (Tabbed == null || TabView == null)
				return;

			var barBackgroundColor = Tabbed.BarBackgroundColor;
			var isDefaultColor = barBackgroundColor.IsDefault;

			if (isDefaultColor && !_barBackgroundColorWasSet)
				return;

			if (!_defaultBarColorSet)
			{

				//	_defaultBarColor = TabView.color;

				_defaultBarColorSet = true;
			}

			if (!isDefaultColor)
				_barBackgroundColorWasSet = true;

			System.Diagnostics.Debug.WriteLine("UpdateBarBackgroundColor not implemented");
		}

		void UpdateBarTextColor()
		{
			if (Tabbed == null || TabView == null)
				return;

			var barTextColor = Tabbed.BarTextColor;
			var isDefaultColor = barTextColor.IsDefault;

			if (isDefaultColor && !_barTextColorWasSet)
				return;

			if (!_defaultBarTextColorSet)
			{
				//	_defaultBarTextColor = TabBar.TintColor;
				_defaultBarTextColorSet = true;
			}

			if (!isDefaultColor)
				_barTextColorWasSet = true;


			System.Diagnostics.Debug.WriteLine("UpdateBarTextColor not implemented");
		}

		void UpdateChildrenOrderIndex()
		{
			for (var i = 0; i < TabViewItems.Length; i++)
			{
				var originalIndex = -1;
				if (int.TryParse(TabViewItems[i].ViewController.Identifier, out originalIndex))
				{
					var page = ((IPageController)Tabbed).InternalChildren[originalIndex];
					TabbedPage.SetIndex(page as Page, i);
				}
			}
		}

		void UpdateCurrentPage()
		{
			var count = ((IPageController)Tabbed).InternalChildren.Count;
			((TabbedPage)Element).CurrentPage = SelectedTabViewItemIndex >= 0 && SelectedTabViewItemIndex < count ? Tabbed.GetPageByIndex((int)SelectedTabViewItemIndex) : null;
		}
	}
}
