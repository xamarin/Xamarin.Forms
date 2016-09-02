using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AppKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.MacOS
{
	public class NavigationPageRenderer : NSViewController, IVisualElementRenderer, IEffectControlProvider
	{
		bool _disposed;
		bool _appeared;
		EventTracker _events;
		VisualElementTracker _tracker;
		Stack<PageWrapper> _currentStack = new Stack<PageWrapper>();

		IPageController PageController => Element as IPageController;

		IElementController ElementController => Element as IElementController;

		void IEffectControlProvider.RegisterEffect(Effect effect)
		{
			var platformEffect = effect as PlatformEffect;
			if (platformEffect != null)
				platformEffect.Container = View;
		}

		public NavigationPageRenderer() : this(IntPtr.Zero) { }
		public NavigationPageRenderer(IntPtr coder)
		{
			View = new FormsNSView { WantsLayer = true, BackgroundColor = Color.Pink.ToNSColor() };

			var transiton = new CATransition();
			transiton.Type = CAAnimation.TransitionPush;
			transiton.Subtype = CAAnimation.TransitionFromLeft;

			View.Animations = NSDictionary.FromObjectAndKey(transiton, new NSString("subviews"));
			ConfigurePageRenderer();
		}



		protected virtual void ConfigurePageRenderer()
		{
			Title = "hello";
			//TransitionStyle = NSPageControllerTransitionStyle.StackHistory;
			//Delegate = new PageDelagete(GetPageIdentifier, GetPageViewController);

		}

		protected override void Dispose(bool disposing)
		{
			if (!_disposed && disposing)
			{
				if (Element != null)
				{
					PageController.SendDisappearing();
					((Element as IPageContainer<Page>).CurrentPage as IPageController).SendDisappearing();
					Element.PropertyChanged -= HandlePropertyChanged;
					Element = null;
				}

				if (_tracker != null)
				{
					_tracker.Dispose();
					_tracker = null;
				}

				if (_events != null)
				{
					_events.Dispose();
					_events = null;
				}

				_disposed = true;
			}
			base.Dispose(disposing);
		}

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

			EffectUtilities.RegisterEffectControlProvider(this, oldElement, element);

		}

		public void SetElementSize(Size size)
		{
			Element.Layout(new Rectangle(Element.X, Element.Y, size.Width, size.Height));

		}

		public NSViewController ViewController
		{
			get { return this; }
		}

		public Task<bool> PopToRootAsync(Page page, bool animated = true)
		{
			return OnPopToRoot(page, animated);
		}

		public Task<bool> PopViewAsync(Page page, bool animated = true)
		{
			return OnPopViewAsync(page, animated);
		}

		public Task<bool> PushPageAsync(Page page, bool animated = true)
		{
			return OnPushAsync(page, animated);
		}

		protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
		{
			if (e.OldElement != null)
				e.OldElement.PropertyChanged -= HandlePropertyChanged;

			if (e.NewElement != null)
				e.NewElement.PropertyChanged += HandlePropertyChanged;

			var changed = ElementChanged;
			if (changed != null)
				changed(this, e);
		}

		public override void ViewDidDisappear()
		{
			base.ViewDidDisappear();

			if (!_appeared)
				return;

			_appeared = false;
			PageController?.SendDisappearing();
		}

		public override void ViewDidAppear()
		{
			base.ViewDidAppear();

			if (_appeared)
				return;

			_appeared = true;
			PageController?.SendAppearing();
		}

		public override void ViewWillAppear()
		{
			Init();

			base.ViewWillAppear();
		}

		public override void ViewDidLayout()
		{
			base.ViewDidLayout();
		}

		void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_tracker == null)
				return;

			if (e.PropertyName == NavigationPage.BarBackgroundColorProperty.PropertyName)
				UpdateBarBackgroundColor();
			else if (e.PropertyName == NavigationPage.BarTextColorProperty.PropertyName)
				UpdateBarTextColor();
			else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
				UpdateBackgroundColor();
			else if (e.PropertyName == NavigationPage.CurrentPageProperty.PropertyName)
				UpdateCurrentPage();

		}

		protected virtual async Task<bool> OnPopToRoot(Page page, bool animated)
		{
			var renderer = Platform.GetRenderer(page);
			if (renderer == null || renderer.ViewController == null)
				return false;

			var success = false;

			UpdateToolBarVisible();
			return success;
		}

		protected virtual async Task<bool> OnPopViewAsync(Page page, bool animated)
		{

			var wrapper = _currentStack.Peek();
			if (page != wrapper.Page)
				throw new NotSupportedException("Popped page does not appear on top of current navigation stack, please file a bug.");

			View.Subviews.Last().RemoveFromSuperview();
			(page as IPageController)?.SendDisappearing();

			_currentStack.Pop();

			UpdateToolBarVisible();
			return true;
		}

		protected virtual async Task<bool> OnPushAsync(Page page, bool animated)
		{
			var shown = true;
			var wrapper = new PageWrapper(page);

			_currentStack.Push(wrapper);
			var newIndex = _currentStack.Count - 1;

			var vc = CreateViewControllerForPage(page);

			View.AddSubview(vc.NativeView);
			vc.NativeView.LayoutSubtreeIfNeeded();
			(page as IPageController)?.SendAppearing();

			UpdateToolBarVisible();
			return shown;
		}


		NSViewController GetPageViewController(string identifier)
		{
			var pageWrapper = _currentStack.FirstOrDefault((arg) => arg.Identifier == identifier);
			if (pageWrapper == null)
				throw new Exception(nameof(identifier) + " not found ");
			return CreateViewControllerForPage(pageWrapper.Page).ViewController;
		}

		string GetPageIdentifier(NSObject obj)
		{
			if (obj == null)
			{
			}
			var ns = obj as PageWrapper;
			return ns?.Identifier ?? "";
		}


		void Init()
		{
			var navPage = (NavigationPage)Element;

			if (navPage.CurrentPage == null)
			{
				throw new InvalidOperationException(
					"NavigationPage must have a root Page before being used. Either call PushAsync with a valid Page, or pass a Page to the constructor before usage.");
			}

			var navController = ((INavigationPageController)navPage);

			navController.PushRequested += OnPushRequested;
			navController.PopRequested += OnPopRequested;
			navController.PopToRootRequested += OnPopToRootRequested;
			navController.RemovePageRequested += OnRemovedPageRequested;
			navController.InsertPageBeforeRequested += OnInsertPageBeforeRequested;

			UpdateBarBackgroundColor();
			UpdateBarTextColor();

			// If there is already stuff on the stack we need to push it
			((INavigationPageController)navPage).StackCopy.Reverse().ForEach(async p => await PushPageAsync(p, false));

			_events = new EventTracker(this);
			_events.LoadEvents(NativeView);
			_tracker = new VisualElementTracker(this);

			Element.PropertyChanged += HandlePropertyChanged;

			UpdateToolBarVisible();
			UpdateBackgroundColor();
		}

		IVisualElementRenderer CreateViewControllerForPage(Page page)
		{
			if (Platform.GetRenderer(page) == null)
				Platform.SetRenderer(page, Platform.CreateRenderer(page));

			var pageRenderer = Platform.GetRenderer(page);
			return pageRenderer;
		}

		void InsertPageBefore(Page page, Page before)
		{
			if (before == null)
				throw new ArgumentNullException("before");
			if (page == null)
				throw new ArgumentNullException("page");

		}

		void OnInsertPageBeforeRequested(object sender, NavigationRequestedEventArgs e)
		{
			InsertPageBefore(e.Page, e.BeforePage);
		}

		void OnPopRequested(object sender, NavigationRequestedEventArgs e)
		{
			e.Task = PopViewAsync(e.Page, e.Animated);
		}

		void OnPopToRootRequested(object sender, NavigationRequestedEventArgs e)
		{
			e.Task = PopToRootAsync(e.Page, e.Animated);
		}

		void OnPushRequested(object sender, NavigationRequestedEventArgs e)
		{
			e.Task = PushPageAsync(e.Page, e.Animated);
		}

		void OnRemovedPageRequested(object sender, NavigationRequestedEventArgs e)
		{
			RemovePage(e.Page);
		}

		void RemovePage(Page page)
		{
			if (page == null)
				throw new ArgumentNullException(nameof(page));
			var target = Platform.GetRenderer(page).ViewController.ParentViewController;
		}


		void UpdateBarTextColor()
		{

		}

		void UpdateBackgroundColor()
		{

		}

		void UpdateBarBackgroundColor()
		{

		}

		void UpdateCurrentPage()
		{

		}

		void UpdateToolBarVisible()
		{

		}
	}
}
