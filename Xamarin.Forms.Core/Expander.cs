using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using static System.Math;

namespace Xamarin.Forms
{
	[ContentProperty(nameof(Content))]
	public class Expander : TemplatedView
	{
		const string ExpandAnimationName = nameof(ExpandAnimationName);
		const uint DefaultAnimationLength = 250;

		public event EventHandler Tapped;

		public static readonly BindableProperty HeaderProperty = BindableProperty.Create(nameof(Header), typeof(View), typeof(Expander), default(View), propertyChanged: (bindable, oldValue, newValue)
			=> ((Expander)bindable).SetHeader((View)oldValue));

		public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(View), typeof(Expander), default(View), propertyChanged: (bindable, oldValue, newValue)
			=> ((Expander)bindable).SetContent());

		public static readonly BindableProperty ContentTemplateProperty = BindableProperty.Create(nameof(ContentTemplate), typeof(DataTemplate), typeof(Expander), default(DataTemplate), propertyChanged: (bindable, oldValue, newValue)
			=> ((Expander)bindable).SetContent(true));

		public static readonly BindableProperty IsExpandedProperty = BindableProperty.Create(nameof(IsExpanded), typeof(bool), typeof(Expander), default(bool), BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue)
			=> ((Expander)bindable).SetContent(false));

		public static readonly BindableProperty DirectionProperty = BindableProperty.Create(nameof(Direction), typeof(ExpandDirection), typeof(Expander), default(ExpandDirection), propertyChanged: (bindable, oldValue, newValue)
			=> ((Expander)bindable).SetDirection((ExpandDirection)oldValue));

		public static readonly BindableProperty ExpandAnimationLengthProperty = BindableProperty.Create(nameof(ExpandAnimationLength), typeof(uint), typeof(Expander), DefaultAnimationLength);

		public static readonly BindableProperty CollapseAnimationLengthProperty = BindableProperty.Create(nameof(CollapseAnimationLength), typeof(uint), typeof(Expander), DefaultAnimationLength);

		public static readonly BindableProperty ExpandAnimationEasingProperty = BindableProperty.Create(nameof(ExpandAnimationEasing), typeof(Easing), typeof(Expander), default(Easing));

		public static readonly BindableProperty CollapseAnimationEasingProperty = BindableProperty.Create(nameof(CollapseAnimationEasing), typeof(Easing), typeof(Expander), default(Easing));

		public static readonly BindableProperty StateProperty = BindableProperty.Create(nameof(State), typeof(ExpanderState), typeof(Expander), default(ExpanderState), BindingMode.OneWayToSource);

		public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(Expander), default(object));

		public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(Expander), default(ICommand));

		public static readonly BindableProperty ForceUpdateSizeCommandProperty = BindableProperty.Create(nameof(ForceUpdateSizeCommand), typeof(ICommand), typeof(Expander), default(ICommand), BindingMode.OneWayToSource);

		DataTemplate _previousTemplate;
		double _lastVisibleSize = -1;
		Size _previousSize = new Size(-1, -1);
		bool _shouldIgnoreContentSetting;
		readonly object _contentSetLocker = new object();
		static bool isExperimentalFlagSet;

		public Expander()
		{
			ExpanderLayout = new StackLayout { Spacing = 0 };
			ForceUpdateSizeCommand = new Command(ForceUpdateSize);
			InternalChildren.Add(ExpanderLayout);
			HeaderTapGestureRecognizer = new TapGestureRecognizer
			{
				CommandParameter = this,
				Command = new Command(parameter =>
				{
					var parent = (parameter as View).Parent;
					while (parent != null && !(parent is Page))
					{
						if (parent is Expander ancestorExpander)
							ancestorExpander.ContentSizeRequest = -1;

						parent = parent.Parent;
					}
					IsExpanded = !IsExpanded;
					Command?.Execute(CommandParameter);
					Tapped?.Invoke(this, EventArgs.Empty);
				})
			};
		}

		internal static void VerifyExperimental([CallerMemberName] string memberName = "", string constructorHint = null)
		{
			if (isExperimentalFlagSet)
				return;

			ExperimentalFlags.VerifyFlagEnabled(nameof(Expander), ExperimentalFlags.ExpanderExperimental, constructorHint, memberName);

			isExperimentalFlagSet = true;
		}

		protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
		{
			VerifyExperimental(nameof(Expander));
			return base.OnMeasure(widthConstraint, heightConstraint);
		}

		StackLayout ExpanderLayout { get; }

		ContentView ContentHolder { get; set; }

		GestureRecognizer HeaderTapGestureRecognizer { get; }

		double Size => Direction.IsVertical()
			? Height
			: Width;

		double ContentSize => Direction.IsVertical()
			? ContentHolder.Height
			: ContentHolder.Width;

		double ContentSizeRequest
		{
			get
			{
				var sizeRequest = Direction.IsVertical()
					? Content.HeightRequest
					: Content.WidthRequest;

				if (sizeRequest < 0 || !(Content is Layout layout))
					return sizeRequest;

				return sizeRequest + (Direction.IsVertical()
					? layout.Padding.VerticalThickness
					: layout.Padding.HorizontalThickness);
			}
			set
			{
				if (Direction.IsVertical())
				{
					ContentHolder.HeightRequest = value;
					return;
				}
				ContentHolder.WidthRequest = value;
			}
		}

		double MeasuredContentSize => Direction.IsVertical()
			? ContentHolder.Measure(Width, double.PositiveInfinity).Request.Height
			: ContentHolder.Measure(double.PositiveInfinity, Height).Request.Width;

		public View Header
		{
			get => (View)GetValue(HeaderProperty);
			set => SetValue(HeaderProperty, value);
		}

		public View Content
		{
			get => (View)GetValue(ContentProperty);
			set => SetValue(ContentProperty, value);
		}

		public DataTemplate ContentTemplate
		{
			get => (DataTemplate)GetValue(ContentTemplateProperty);
			set => SetValue(ContentTemplateProperty, value);
		}

		public bool IsExpanded
		{
			get => (bool)GetValue(IsExpandedProperty);
			set => SetValue(IsExpandedProperty, value);
		}

		public ExpandDirection Direction
		{
			get => (ExpandDirection)GetValue(DirectionProperty);
			set => SetValue(DirectionProperty, value);
		}

		public uint ExpandAnimationLength
		{
			get => (uint)GetValue(ExpandAnimationLengthProperty);
			set => SetValue(ExpandAnimationLengthProperty, value);
		}

		public uint CollapseAnimationLength
		{
			get => (uint)GetValue(CollapseAnimationLengthProperty);
			set => SetValue(CollapseAnimationLengthProperty, value);
		}

		public Easing ExpandAnimationEasing
		{
			get => (Easing)GetValue(ExpandAnimationEasingProperty);
			set => SetValue(ExpandAnimationEasingProperty, value);
		}

		public Easing CollapseAnimationEasing
		{
			get => (Easing)GetValue(CollapseAnimationEasingProperty);
			set => SetValue(CollapseAnimationEasingProperty, value);
		}

		public ExpanderState State
		{
			get => (ExpanderState)GetValue(StateProperty);
			set => SetValue(StateProperty, value);
		}

		public object CommandParameter
		{
			get => GetValue(CommandParameterProperty);
			set => SetValue(CommandParameterProperty, value);
		}

		public ICommand Command
		{
			get => (ICommand)GetValue(CommandProperty);
			set => SetValue(CommandProperty, value);
		}

		public ICommand ForceUpdateSizeCommand
		{
			get => (ICommand)GetValue(ForceUpdateSizeCommandProperty);
			set => SetValue(ForceUpdateSizeCommandProperty, value);
		}

		public void ForceUpdateSize()
		{
			_lastVisibleSize = -1;
			OnIsExpandedChanged();
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();
			_lastVisibleSize = -1;
			SetContent(true, true);
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);
			if ((Abs(width - _previousSize.Width) >= double.Epsilon && Direction.IsVertical()) ||
				(Abs(height - _previousSize.Height) >= double.Epsilon && !Direction.IsVertical()))
				ForceUpdateSize();

			_previousSize = new Size(width, height);
		}

		void OnIsExpandedChanged(bool shouldIgnoreAnimation = false)
		{
			if (ContentHolder == null || (!IsExpanded && !ContentHolder.IsVisible))
				return;

			var isAnimationRunning = ContentHolder.AnimationIsRunning(ExpandAnimationName);
			ContentHolder.AbortAnimation(ExpandAnimationName);

			var startSize = ContentHolder.IsVisible
				? Max(ContentSize, 0)
				: 0;

			if (IsExpanded)
				ContentHolder.IsVisible = true;

			var endSize = ContentSizeRequest >= 0
				? ContentSizeRequest
				: _lastVisibleSize;

			if (IsExpanded)
			{
				if (endSize <= 0)
				{
					ContentSizeRequest = -1;
					endSize = MeasuredContentSize;
					ContentSizeRequest = 0;
				}
			}
			else
			{
				_lastVisibleSize = startSize = ContentSizeRequest >= 0
						? ContentSizeRequest
						: !isAnimationRunning
							? ContentSize
							: _lastVisibleSize;
				endSize = 0;
			}

			InvokeAnimation(startSize, endSize, shouldIgnoreAnimation);
		}

		void SetHeader(View oldHeader)
		{
			if (oldHeader != null)
			{
				oldHeader.GestureRecognizers.Remove(HeaderTapGestureRecognizer);
				ExpanderLayout.Children.Remove(oldHeader);
			}

			if (Header != null)
			{
				if (Direction.IsRegularOrder())
					ExpanderLayout.Children.Insert(0, Header);
				else
					ExpanderLayout.Children.Add(Header);

				Header.GestureRecognizers.Add(HeaderTapGestureRecognizer);
			}
		}

		void SetContent(bool isForceUpdate, bool shouldIgnoreAnimation = false, bool isForceContentReset = false)
		{
			if (IsExpanded && (Content == null || isForceUpdate || isForceContentReset))
			{
				lock (_contentSetLocker)
				{
					_shouldIgnoreContentSetting = true;

					var contentFromTemplate = CreateContent();
					if (contentFromTemplate != null)
						Content = contentFromTemplate;
					else if (isForceContentReset)
						SetContent();

					_shouldIgnoreContentSetting = false;
				}
			}
			OnIsExpandedChanged(shouldIgnoreAnimation);
		}

		void SetContent()
		{
			if (ContentHolder != null)
			{
				ContentHolder.AbortAnimation(ExpandAnimationName);
				ExpanderLayout.Children.Remove(ContentHolder);
				ContentHolder = null;
			}
			if (Content != null)
			{
				ContentHolder = new ContentView
				{
					IsClippedToBounds = true,
					IsVisible = false,
					Content = Content
				};
				ContentSizeRequest = 0;

				if (Direction.IsRegularOrder())
					ExpanderLayout.Children.Add(ContentHolder);
				else
					ExpanderLayout.Children.Insert(0, ContentHolder);
			}

			if (!_shouldIgnoreContentSetting)
				SetContent(true);
		}

		View CreateContent()
		{
			var template = ContentTemplate;
			while (template is DataTemplateSelector selector)
				template = selector.SelectTemplate(BindingContext, this);

			if (template == _previousTemplate && Content != null)
				return null;

			_previousTemplate = template;
			return (View)template?.CreateContent();
		}

		void SetDirection(ExpandDirection oldDirection)
		{
			if(oldDirection.IsVertical() == Direction.IsVertical())
			{
				SetHeader(Header);
				return;
			}

			ExpanderLayout.Orientation = Direction.IsVertical()
				? StackOrientation.Vertical
				: StackOrientation.Horizontal;
			_lastVisibleSize = -1;
			SetHeader(Header);
			SetContent(true, true, true);
		}

		void InvokeAnimation(double startSize, double endSize, bool shouldIgnoreAnimation)
		{
			State = IsExpanded ? ExpanderState.Expanding : ExpanderState.Collapsing;

			if (shouldIgnoreAnimation || Size < 0)
			{
				State = IsExpanded ? ExpanderState.Expanded : ExpanderState.Collapsed;
				ContentSizeRequest = endSize;
				ContentHolder.IsVisible = IsExpanded;
				return;
			}

			var length = ExpandAnimationLength;
			var easing = ExpandAnimationEasing;
			if (!IsExpanded)
			{
				length = CollapseAnimationLength;
				easing = CollapseAnimationEasing;
			}

			if (_lastVisibleSize > 0)
				length = Max((uint)(length * (Abs(endSize - startSize) / _lastVisibleSize)), 1);

			new Animation(v => ContentSizeRequest = v, startSize, endSize)
				.Commit(ContentHolder, ExpandAnimationName, 16, length, easing, (value, isInterrupted) =>
				{
					if (isInterrupted)
						return;

					if (!IsExpanded)
					{
						ContentHolder.IsVisible = false;
						State = ExpanderState.Collapsed;
						return;
					}
					State = ExpanderState.Expanded;
				});
		}
	}
}
