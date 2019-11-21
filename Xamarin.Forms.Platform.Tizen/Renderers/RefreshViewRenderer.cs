using ElmSharp;
using System.Reflection;
using System.Threading.Tasks;

namespace Xamarin.Forms.Platform.Tizen
{
	class RefreshIcon : ContentView
	{
		public static readonly int IconSize = 48;
		static readonly Color DefaultColor = Color.FromHex("#6200EE");
		static readonly string IconPath = "Xamarin.Forms.Platform.Tizen.Resource.refresh_48dp.png";
		Image _icon;

		public RefreshIcon()
		{
			HeightRequest = IconSize;
			WidthRequest = IconSize;
			var layout = new AbsoluteLayout()
			{
				HeightRequest = IconSize,
				WidthRequest = IconSize,
			};

			layout.Children.Add(new BoxView
			{
				Color = Color.White,
				CornerRadius = new CornerRadius(IconSize),
			}, new Rectangle(0.5, 0.5, IconSize, IconSize), AbsoluteLayoutFlags.PositionProportional);

			_icon = new Image
			{
				Source = ImageSource.FromResource(IconPath, typeof(ShellItemRenderer).Assembly),
			};

			layout.Children.Add(_icon, new Rectangle(0.5, 0.5, IconSize - 8, IconSize - 8), AbsoluteLayoutFlags.PositionProportional);
			Content = layout;

			IconColor = DefaultColor;
		}

		bool IsPlaying { get; set; }

		public Color IconColor
		{
			get
			{
				return PlatformConfiguration.TizenSpecific.Image.GetBlendColor(_icon);
			}
			set
			{
				PlatformConfiguration.TizenSpecific.Image.SetBlendColor(_icon, value == Color.Default ? DefaultColor : value);
			}
		}

		public double IconRotation
		{
			get
			{
				return _icon.Rotation;
			}
			set
			{
				_icon.Rotation = value;
			}
		}

		public void Start()
		{
			Stop();
			IsPlaying = true;
			TurnInternal();
		}

		public void Stop()
		{
			IsPlaying = false;
			_icon.AbortAnimation("RotateTo");
		}

		async void TurnInternal()
		{
			await _icon.RelRotateTo(360, 1000);
			if (IsPlaying)
				TurnInternal();
		}
	}

	class RefreshLayout : StackLayout
	{
		static readonly int MaximumDistance = 100;

		public RefreshLayout()
		{
			HeightRequest = 200;
			HorizontalOptions = LayoutOptions.FillAndExpand;

			RefreshIcon = new RefreshIcon
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				TranslationY = -RefreshIcon.IconSize, 
				Opacity = 0.5,
			};
			Children.Add(RefreshIcon);
		}

		public void SetDistance(double distance)
		{
			var calculated = -RefreshIcon.IconSize + distance;
			if (calculated > MaximumDistance)
				calculated = MaximumDistance;
			RefreshIcon.TranslationY = calculated;
			RefreshIcon.IconRotation = 180 * (calculated / (float)MaximumDistance);
			RefreshIcon.Opacity = 0.5 + (calculated / (float)MaximumDistance);
		}

		public void Start()
		{
			_ = RefreshIcon.TranslateTo(0, MaximumDistance / 2.0, length:200);
			RefreshIcon.Start();
		}

		public bool ShouldRefresh()
		{
			return RefreshIcon.TranslationY > (MaximumDistance - 30);
		}

		async public Task Stop()
		{
			_ = RefreshIcon.FadeTo(0);
			await RefreshIcon.ScaleTo(0.2);
			RefreshIcon.Stop();
		}

		public async Task ResetRefreshIcon()
		{

			new Animation((r) =>
			{
				RefreshIcon.IconRotation = 180 * (RefreshIcon.TranslationY / (float)MaximumDistance);
			}).Commit(RefreshIcon, "reset", length: 250);
			_ = RefreshIcon.FadeTo(0.5, length: 250);
			await RefreshIcon.TranslateTo(0, -RefreshIcon.IconSize, length: 250);
		}

		public RefreshIcon RefreshIcon { get; set;}
	}

	public enum RefreshState
	{
		Idle,
		Drag,
		Loading,
	}

	public class RefreshViewRenderer : LayoutRenderer
	{
		GestureLayer _gestureLayer;

		RefreshLayout _refreshLayout;
		IVisualElementRenderer _refreshIconRenderer;

		public RefreshViewRenderer()
		{
			RegisterPropertyHandler(RefreshView.RefreshColorProperty, UpdateRefreshColor);
			RegisterPropertyHandler(RefreshView.IsRefreshingProperty, UpdateIsRefreshing);
		}


		RefreshView RefreshView => Element as RefreshView;
		RefreshState RefreshState { get; set; }


		protected override void OnElementChanged(ElementChangedEventArgs<Layout> e)
		{
			base.OnElementChanged(e);
			Initialize();
		}

		void Initialize()
		{
			_gestureLayer?.Unrealize();
			_gestureLayer = new GestureLayer(NativeView);
			_gestureLayer.Attach(NativeView);

			_gestureLayer.SetMomentumCallback(GestureLayer.GestureState.Move, OnMoved);
			_gestureLayer.SetMomentumCallback(GestureLayer.GestureState.End, OnEnd);
			_gestureLayer.SetMomentumCallback(GestureLayer.GestureState.Abort, OnEnd);
		}

		void UpdateRefreshLayout()
		{
			_refreshLayout = new RefreshLayout();
			_refreshLayout.RefreshIcon.IconColor = RefreshView.RefreshColor;
			_refreshIconRenderer = Platform.GetOrCreateRenderer(_refreshLayout);
			(_refreshIconRenderer as LayoutRenderer).RegisterOnLayoutUpdated();

			Control.Children.Add(_refreshIconRenderer.NativeView);
			var measured = _refreshLayout.Measure(Element.Width, Element.Height);
			var parentBound = NativeView.Geometry;
			var bound = new Rect
			{
				X = parentBound.X,
				Y = parentBound.Y,
				Width = parentBound.Width,
				Height = Forms.ConvertToScaledPixel(measured.Request.Height)
			};

			_refreshIconRenderer.NativeView.Geometry = bound;
			RefreshState = RefreshState.Drag;
		}

		bool IsEdgeScrolling()
		{
			if (RefreshView.Content is ScrollView scrollview)
			{
				if (scrollview.ScrollY == 0)
				{
					return true;
				}
			}

			if (Platform.GetRenderer(RefreshView.Content) is ItemsViewRenderer itemsViewRenderer)
			{
				var collectionView = itemsViewRenderer.NativeView;

				var scroller = collectionView.GetType().GetProperty("Scroller", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(collectionView);

				if (scroller != null)
				{
					if ((scroller as Scroller)?.CurrentRegion.Y == 0)
					{
						return true;
					}
				}
			}

			return false;
		}

		void OnMoved(GestureLayer.MomentumData moment)
		{
			if (RefreshState == RefreshState.Idle)
			{
				if (IsEdgeScrolling())
				{
					UpdateRefreshLayout();
				}
			}

			if (RefreshState == RefreshState.Drag)
			{
				var dy = moment.Y2 - moment.Y1;
				_refreshLayout?.SetDistance(Forms.ConvertToScaledDP(dy));
			}
		}

		void OnEnd(GestureLayer.MomentumData moment)
		{
			if (RefreshState == RefreshState.Drag && _refreshLayout != null && _refreshIconRenderer != null)
			{
				if (_refreshLayout.ShouldRefresh())
				{
					_refreshLayout.Start();
					RefreshState = RefreshState.Loading;
					RefreshView.SetValueFromRenderer(RefreshView.IsRefreshingProperty, true);
				}
				else
				{
					ResetRefresh();
				}

			}
		}

		async void ResetRefresh()
		{
			var refreshLayout = _refreshLayout;
			var refreshIconRenderer = _refreshIconRenderer;
			_refreshLayout = null;
			_refreshIconRenderer = null;
			await refreshLayout.ResetRefreshIcon();
			refreshIconRenderer?.Dispose();
			RefreshState = RefreshState.Idle;
		}

		void UpdateRefreshColor()
		{
			if (_refreshLayout != null)
			{
				_refreshLayout.RefreshIcon.IconColor = RefreshView.RefreshColor;
			}
		}

		async void UpdateIsRefreshing(bool init)
		{
			if (init)
				return;

			if (!RefreshView.IsRefreshing && RefreshState == RefreshState.Loading)
			{
				var refreshLayout = _refreshLayout;
				var refreshIconRenderer = _refreshIconRenderer;
				_refreshLayout = null;
				_refreshIconRenderer = null;
				await refreshLayout?.Stop();
				refreshIconRenderer?.Dispose();

				RefreshState = RefreshState.Idle;
			}
		}
	}
}
