using ElmSharp;

namespace Xamarin.Forms.Platform.Tizen
{
	public class MasterDetailPageRenderer : VisualElementRenderer<MasterDetailPage>
	{
		Native.MasterDetailPage _mdpage;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public MasterDetailPageRenderer()
		{
			RegisterPropertyHandler("Master", UpdateMasterPage);
			RegisterPropertyHandler("Detail", UpdateDetailPage);
			RegisterPropertyHandler(MasterDetailPage.IsPresentedProperty,
				UpdateIsPresented);
			RegisterPropertyHandler(MasterDetailPage.MasterBehaviorProperty,
				UpdateMasterBehavior);
			RegisterPropertyHandler(MasterDetailPage.IsGestureEnabledProperty,
				UpdateIsGestureEnabled);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<MasterDetailPage> e)
		{
			if (_mdpage == null)
			{
				_mdpage = new Native.MasterDetailPage(Forms.NativeParent)
				{
					IsPresented = e.NewElement.IsPresented,
				};

				_mdpage.IsPresentedChanged += (sender, ev) =>
				{
					Element.IsPresented = ev.IsPresent;
				};
				_mdpage.UpdateIsPresentChangeable += (sender, ev) =>
				{
					(Element as IMasterDetailPageController).CanChangeIsPresented = ev.CanChange;
				};
				SetNativeView(_mdpage);
			}

			if (e.OldElement != null)
			{
				(e.OldElement as IMasterDetailPageController).BackButtonPressed -= OnBackButtonPressed;
			}

			if (e.NewElement != null)
			{
				(e.NewElement as IMasterDetailPageController).BackButtonPressed += OnBackButtonPressed;
			}

			UpdateMasterBehavior();
			base.OnElementChanged(e);
		}

		protected override void OnElementReady()
		{
			base.OnElementReady();
			UpdateMasterPage(false);
			UpdateDetailPage(false);
		}

		void OnBackButtonPressed(object sender, BackButtonPressedEventArgs e)
		{
			if ((Element != null) && Element.IsPresented)
			{
				Element.IsPresented = false;
				e.Handled = true;
			}
		}

		EvasObject GetNativePage(Page page)
		{
			var pageRenderer = Platform.GetOrCreateRenderer(page);
			return pageRenderer.NativeView;
		}

		void UpdateMasterBehavior() {
			_mdpage.MasterBehavior = Element.MasterBehavior;
		}

		void UpdateMasterPage(bool isInit)
		{
			if(!isInit)
				_mdpage.Master = GetNativePage(Element.Master);
		}

		void UpdateDetailPage(bool isInit)
		{
			if (!isInit)
				_mdpage.Detail = GetNativePage(Element.Detail);
		}

		void UpdateIsPresented()
		{
			_mdpage.IsPresented = Element.IsPresented;
		}

		void UpdateIsGestureEnabled()
		{
			_mdpage.IsGestureEnabled = Element.IsGestureEnabled;
		}
	}
}
