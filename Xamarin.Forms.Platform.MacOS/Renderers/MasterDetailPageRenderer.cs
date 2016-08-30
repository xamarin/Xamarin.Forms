using System;
using System.ComponentModel;
using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{



	public class MasterDetailPageRenderer : NSSplitViewController, IVisualElementRenderer, IEffectControlProvider
	{
		bool _disposed;
		EventTracker _events;
		VisualElementTracker _tracker;

		MasterDetailPage _masterDetailPage;

		IPageController PageController => Element as IPageController;

		IElementController ElementController => Element as IElementController;

		IMasterDetailPageController MasterDetailPageController => MasterDetailPage as IMasterDetailPageController;

		void IEffectControlProvider.RegisterEffect(Effect effect)
		{
			var platformEffect = effect as PlatformEffect;
			if (platformEffect != null)
				platformEffect.Container = View;
		}

		protected MasterDetailPage MasterDetailPage
		{
			get { return _masterDetailPage ?? (_masterDetailPage = (MasterDetailPage)Element); }
		}

		protected override void Dispose(bool disposing)
		{
			if (!_disposed && disposing)
			{
				if (Element != null)
				{
					PageController.SendDisappearing();
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


				MasterDetailPage.Master.PropertyChanged -= HandleMasterPropertyChanged;

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

			UpdateControllers();

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

		public override void ViewWillAppear()
		{

			UpdateBackground();
			_tracker = new VisualElementTracker(this);
			_events = new EventTracker(this);
			_events.LoadEvents(NativeView);

			base.ViewWillAppear();
		}

		public override void ViewDidLayout()
		{

			View.LayoutSubtreeIfNeeded();

			base.ViewDidLayout();
		}

		void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_tracker == null)
				return;

			if (e.PropertyName == "Master" || e.PropertyName == "Detail")
				UpdateControllers();
		}

		void UpdateBackground()
		{
			//if (!string.IsNullOrEmpty(((Page)Element).BackgroundImage))
			//	View.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle(((Page)Element).BackgroundImage));
			//else if (Element.BackgroundColor == Color.Default)
			//	View.BackgroundColor = UIColor.White;
			//else
			//	View.BackgroundColor = Element.BackgroundColor.ToUIColor();
		}


		void UpdateControllers()
		{
			MasterDetailPage.Master.PropertyChanged -= HandleMasterPropertyChanged;

			if (Platform.GetRenderer(MasterDetailPage.Master) == null)
				Platform.SetRenderer(MasterDetailPage.Master, Platform.CreateRenderer(MasterDetailPage.Master));
			if (Platform.GetRenderer(MasterDetailPage.Detail) == null)
				Platform.SetRenderer(MasterDetailPage.Detail, Platform.CreateRenderer(MasterDetailPage.Detail));

			ClearControllers();

			var master = Platform.GetRenderer(MasterDetailPage.Master).ViewController;
			var detail = Platform.GetRenderer(MasterDetailPage.Detail).ViewController;


			AddSplitViewItem(new NSSplitViewItem { ViewController = master, HoldingPriority = 0.2f });
			AddSplitViewItem(new NSSplitViewItem { ViewController = detail, HoldingPriority = 0.8f });


			View.AddConstraint(NSLayoutConstraint.Create(SplitViewItems[0], NSLayoutAttribute.Width, NSLayoutRelation.LessThanOrEqual, 0.2f, 100));
			View.AddConstraint(NSLayoutConstraint.Create(SplitViewItems[1], NSLayoutAttribute.Width, NSLayoutRelation.GreaterThanOrEqual, 0.8f, 100));

			MasterDetailPage.Master.PropertyChanged += HandleMasterPropertyChanged;
		}

		void ClearControllers()
		{

		}

		void HandleMasterPropertyChanged(object sender, PropertyChangedEventArgs e)
		{

		}
	}
}
