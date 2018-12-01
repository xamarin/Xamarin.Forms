﻿using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class ShellSectionRootRenderer : UIViewController, IShellSectionRootRenderer
	{
		#region IShellSectionRootRenderer

		bool IShellSectionRootRenderer.ShowNavBar => Shell.GetNavBarIsVisible(((IShellContentController)ShellSection.CurrentItem).GetOrCreateContent());

		UIViewController IShellSectionRootRenderer.ViewController => this;

		#endregion IShellSectionRootRenderer

		const int HeaderHeight = 35;
		readonly IShellContext _shellContext;
		UIView _blurView;
		UIView _containerArea;
		int _currentIndex;
		ShellSectionRootHeader _header;
		bool _isAnimating;
		Dictionary<ShellContent, IVisualElementRenderer> _renderers = new Dictionary<ShellContent, IVisualElementRenderer>();
		IShellPageRendererTracker _tracker;

		ShellSection ShellSection { get; set; }

		public ShellSectionRootRenderer(ShellSection shellSection, IShellContext shellContext)
		{
			ShellSection = shellSection ?? throw new ArgumentNullException(nameof(shellSection));
			_shellContext = shellContext;
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();

			_containerArea.Frame = View.Bounds;

			LayoutRenderers();

			LayoutHeader();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			_containerArea = new UIView();
			if (Forms.IsiOS11OrNewer)
				_containerArea.InsetsLayoutMarginsFromSafeArea = false;

			View.AddSubview(_containerArea);

			LoadRenderers();

			ShellSection.PropertyChanged += OnShellSectionPropertyChanged;
			((INotifyCollectionChanged)ShellSection.Items).CollectionChanged += OnShellSectionItemsChanged;

			_blurView = new UIView();
			UIVisualEffect blurEffect = UIBlurEffect.FromStyle(UIBlurEffectStyle.ExtraLight);
			_blurView = new UIVisualEffectView(blurEffect);

			View.AddSubview(_blurView);		

			UpdateHeaderVisibility();

			var tracker = _shellContext.CreatePageRendererTracker();
			tracker.IsRootPage = true;
			tracker.ViewController = this;
			tracker.Page = ((IShellContentController)ShellSection.CurrentItem).GetOrCreateContent();
			_tracker = tracker;
		}

		public override void ViewSafeAreaInsetsDidChange()
		{
			base.ViewSafeAreaInsetsDidChange();

			LayoutHeader();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing && ShellSection != null)
			{
				ShellSection.PropertyChanged -= OnShellSectionPropertyChanged;
				((INotifyCollectionChanged)ShellSection.Items).CollectionChanged -= OnShellSectionItemsChanged;

				_header?.Dispose();
				_tracker?.Dispose();

				foreach (var shellContent in ShellSection.Items)
				{
					if (_renderers.TryGetValue(shellContent, out var oldRenderer))
					{
						_renderers.Remove(shellContent);
						oldRenderer.NativeView.RemoveFromSuperview();
						oldRenderer.ViewController.RemoveFromParentViewController();
						oldRenderer.Dispose();
					}
				}
			}

			ShellSection = null;
			_header = null;
			_tracker = null;
		}

		protected virtual void LayoutRenderers()
		{
			if (_isAnimating)
				return;

			var items = ShellSection.Items;
			for (int i = 0; i < items.Count; i++)
			{
				var shellContent = items[i];
				if (_renderers.TryGetValue(shellContent, out var renderer))
				{
					var view = renderer.NativeView;
					view.Frame = new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height);
				}
			}
		}

		protected virtual void LoadRenderers()
		{
			var currentItem = ShellSection.CurrentItem;
			for (int i = 0; i < ShellSection.Items.Count; i++)
			{
				ShellContent item = ShellSection.Items[i];
				var page = ((IShellContentController)item).GetOrCreateContent();
				var renderer = Platform.CreateRenderer(page);
				Platform.SetRenderer(page, renderer);

				AddChildViewController(renderer.ViewController);

				if (item == currentItem)
				{
					_containerArea.AddSubview(renderer.NativeView);
					_currentIndex = i;
				}

				_renderers[item] = renderer;
			}
		}

		protected virtual void OnShellSectionPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == ShellSection.CurrentItemProperty.PropertyName)
			{
				var items = ShellSection.Items;
				var currentItem = ShellSection.CurrentItem;

				var oldIndex = _currentIndex;
				var oldItem = items[oldIndex];

				_currentIndex = items.IndexOf(currentItem);

				var oldRenderer = _renderers[oldItem];
				var currentRenderer = _renderers[currentItem];

				// -1 == slide left, 1 ==  slide right
				int motionDirection = _currentIndex > oldIndex ? -1 : 1;

				_containerArea.AddSubview(currentRenderer.NativeView);

				_isAnimating = true;

				currentRenderer.NativeView.Frame = new CGRect(-motionDirection * View.Bounds.Width, 0, View.Bounds.Width, View.Bounds.Height);
				oldRenderer.NativeView.Frame = _containerArea.Bounds;

				UIView.Animate(.25, 0, UIViewAnimationOptions.CurveEaseOut, () =>
				{
					currentRenderer.NativeView.Frame = _containerArea.Bounds;
					oldRenderer.NativeView.Frame = new CGRect(motionDirection * View.Bounds.Width, 0, View.Bounds.Width, View.Bounds.Height);
				},
				() =>
				{
					oldRenderer.NativeView.RemoveFromSuperview();
					_isAnimating = false;

					_tracker.Page = ((IShellContentController)currentItem).Page;
				});
			}
		}

		protected virtual void UpdateHeaderVisibility()
		{
			bool visible = ShellSection.Items.Count > 1;

			if (visible)
			{
				if (_header == null)
				{
					_header = new ShellSectionRootHeader(_shellContext);
					_header.ShellSection = ShellSection;

					AddChildViewController(_header);
					View.AddSubview(_header.View);
				}
				_blurView.Hidden = false;
				LayoutHeader();
			}
			else
			{
				if (_header != null)
				{
					_header.View.RemoveFromSuperview();
					_header.RemoveFromParentViewController();
					_header.Dispose();
					_header = null;
				}
				_blurView.Hidden = true;
			}
		}

		void OnShellSectionItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			// Make sure we do this after the header has a chance to react
			Device.BeginInvokeOnMainThread(UpdateHeaderVisibility);

			if (e.OldItems != null)
			{
				foreach (ShellContent oldItem in e.OldItems)
				{
					var oldRenderer = _renderers[oldItem];
					_renderers.Remove(oldItem);
					oldRenderer.NativeView.RemoveFromSuperview();
					oldRenderer.ViewController.RemoveFromParentViewController();
					oldRenderer.Dispose();
				}
			}

			if (e.NewItems != null)
			{
				foreach (ShellContent newItem in e.NewItems)
				{
					var page = ((IShellContentController)newItem).GetOrCreateContent();
					var renderer = Platform.CreateRenderer(page);
					Platform.SetRenderer(page, renderer);

					AddChildViewController(renderer.ViewController);
					_renderers[newItem] = renderer;
				}
			}
		}

		void LayoutHeader()
		{
			if (_header == null)
				return;

			CGRect frame;
			if (Forms.IsiOS11OrNewer)
				frame = new CGRect(View.Bounds.X, View.SafeAreaInsets.Top, View.Bounds.Width, HeaderHeight);
			else
				frame = new CGRect(View.Bounds.X, TopLayoutGuide.Length, View.Bounds.Width, HeaderHeight);
			_blurView.Frame = frame;
			_header.View.Frame = frame;

			nfloat left;
			nfloat top;
			nfloat right;
			nfloat bottom;
			if (Forms.IsiOS11OrNewer)
			{
				left = View.SafeAreaInsets.Left;
				top = View.SafeAreaInsets.Top;
				right = View.SafeAreaInsets.Right;
				bottom = View.SafeAreaInsets.Bottom;
			}
			else
			{
				left = 0;
				top = TopLayoutGuide.Length;
				right = 0;
				bottom = BottomLayoutGuide.Length;
			}

			((IShellSectionController)ShellSection).SendInsetChanged(new Thickness(left, top, right, bottom), HeaderHeight);
		}
	}
}