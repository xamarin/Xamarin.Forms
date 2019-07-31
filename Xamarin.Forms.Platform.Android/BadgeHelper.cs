using System;
using System.Linq;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using LP = Android.Views.ViewGroup.LayoutParams;
#if __ANDROID_29__
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Tabs;
#else
using Android.Support.Design.Internal;
using Android.Support.Design.Widget;
#endif

namespace Xamarin.Forms.Platform.Android
{
	internal static class BadgeHelper
	{
		public static void ApplyBadge(this BottomNavigationItemView bottomNavigationView, Color color, string text,
			Color textColor)
		{
			if (!bottomNavigationView.GetChildrenOfType<BadgeFrameLayout>().Any())
			{
				bottomNavigationView.SetClipChildren(false);
				bottomNavigationView.SetClipToPadding(false);

				ImageView imageView = bottomNavigationView.GetChildrenOfType<ImageView>().Single();
				bottomNavigationView.RemoveView(imageView);

				FrameLayout badgeContainerFrameLayout = new FrameLayout(bottomNavigationView.Context)
				{
					LayoutParameters = new FrameLayout.LayoutParams(LP.WrapContent, LP.WrapContent)
					{
						Gravity = GravityFlags.CenterHorizontal
					}
				};

				badgeContainerFrameLayout.AddView(imageView);

				BadgeFrameLayout badgeContainer = CreateBadgeContainer(bottomNavigationView.Context);
				badgeContainer.TopMargin += 10;

				badgeContainer.Visibility = !string.IsNullOrEmpty(text) ? ViewStates.Visible : ViewStates.Invisible;

				badgeContainer.Background = CreateBadgeBackground(bottomNavigationView.Context, color);
				badgeContainer.AddView(CreateBadgeText(bottomNavigationView.Context, text, textColor));

				badgeContainerFrameLayout.AddView(badgeContainer);

				bottomNavigationView.AddView(badgeContainerFrameLayout);

			}
			else
			{
				BadgeFrameLayout badgeContainer = bottomNavigationView.GetChildrenOfType<BadgeFrameLayout>().Single();
				badgeContainer.Visibility = !string.IsNullOrEmpty(text) ? ViewStates.Visible : ViewStates.Invisible;

				((PaintDrawable)badgeContainer.Background).Paint.Color = color.IsDefault ? Color.FromRgb(255, 59, 48).ToAndroid() : color.ToAndroid();

				TextView textView = (TextView)badgeContainer.GetChildAt(0);
				textView.Text = text;
				textView.SetTextColor(textColor.IsDefault ? Color.White.ToAndroid() : textColor.ToAndroid());
			}
		}

		public static void ApplyBadge(this TabLayout.TabView tabView, Color color, string text,
			Color textColor)
		{
			if (!tabView.GetChildrenOfType<BadgeFrameLayout>().Any())
			{
				tabView.SetClipChildren(false);
				tabView.SetClipToPadding(false);

				TextView tabTextView = tabView.GetChildrenOfType<TextView>().Single();
				tabView.RemoveView(tabTextView);

				FrameLayout badgeContainerFrameLayout = new FrameLayout(tabView.Context)
				{
					LayoutParameters = new FrameLayout.LayoutParams(LP.WrapContent, LP.WrapContent)
				};

				badgeContainerFrameLayout.AddView(tabTextView);

				BadgeFrameLayout badgeContainer = CreateBadgeContainer(tabView.Context);
				badgeContainer.Visibility = !string.IsNullOrEmpty(text) ? ViewStates.Visible : ViewStates.Invisible;

				badgeContainer.Background = CreateBadgeBackground(tabView.Context, color);
				badgeContainer.AddView(CreateBadgeText(tabView.Context, text, textColor));

				badgeContainerFrameLayout.AddView(badgeContainer);

				tabView.AddView(badgeContainerFrameLayout);
			}
			else
			{
				BadgeFrameLayout badgeContainer = tabView.GetChildrenOfType<BadgeFrameLayout>().Single();
				badgeContainer.Visibility = !string.IsNullOrEmpty(text) ? ViewStates.Visible : ViewStates.Invisible;

				((PaintDrawable)badgeContainer.Background).Paint.Color = color.IsDefault ? Color.FromRgb(255, 59, 48).ToAndroid() : color.ToAndroid();

				TextView textView = (TextView)badgeContainer.GetChildAt(0);
				textView.Text = text;
				textView.SetTextColor(textColor.IsDefault ? Color.White.ToAndroid() : textColor.ToAndroid());
			}
		}

		static BadgeFrameLayout CreateBadgeContainer(Context context)
		{
			BadgeFrameLayout badgeFrameLayout = new BadgeFrameLayout(context)
			{
				LayoutParameters = new FrameLayout.LayoutParams(LP.WrapContent, LP.WrapContent)
				{
					Gravity = GravityFlags.Top | GravityFlags.Right
				}
			};

			badgeFrameLayout.SetPadding(
				(int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 6, context.Resources.DisplayMetrics),
				(int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 2, context.Resources.DisplayMetrics),
				(int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 6, context.Resources.DisplayMetrics),
				(int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 2, context.Resources.DisplayMetrics));

			return badgeFrameLayout;
		}

		static TextView CreateBadgeText(Context context, string text, Color textColor)
		{
			var textView = new TextView(context)
			{
				Text = text,
				LayoutParameters = new FrameLayout.LayoutParams(LP.WrapContent, LP.WrapContent)
				{
					Gravity = GravityFlags.Center
				}
			};

			textView.SetTextColor(textColor.IsDefault ? Color.White.ToAndroid() : textColor.ToAndroid());
			textView.SetTextSize(ComplexUnitType.Sp, 10);

			return textView;
		}

		static PaintDrawable CreateBadgeBackground(Context context, Color color)
		{
			var badgeFrameLayoutBackground = new PaintDrawable();

			badgeFrameLayoutBackground.Paint.Color =
				color.IsDefault ? Color.FromRgb(255, 59, 48).ToAndroid() : color.ToAndroid();

			badgeFrameLayoutBackground.Shape = new RectShape();
			badgeFrameLayoutBackground.SetCornerRadius(TypedValue.ApplyDimension(ComplexUnitType.Dip, 8,
				context.Resources.DisplayMetrics));

			return badgeFrameLayoutBackground;
		}

		internal class BadgeFrameLayout : FrameLayout
		{
			public int TopMargin { get; set; } = 10;

			public int RightMargin { get; set; } = 10;

			protected BadgeFrameLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
			{
			}

			public BadgeFrameLayout(Context context) : base(context)
			{
			}

			public BadgeFrameLayout(Context context, IAttributeSet attrs) : base(context, attrs)
			{
			}

			public BadgeFrameLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
			{
			}

			public BadgeFrameLayout(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
			{
			}

			protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
			{
				base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

				if (this.LayoutParameters is MarginLayoutParams layoutParameters)
				{
					int rightMargin = -(this.MeasuredWidth - (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, RightMargin, Context.Resources.DisplayMetrics));
					int topMargin = -(this.MeasuredHeight - (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, TopMargin, Context.Resources.DisplayMetrics));

					layoutParameters.SetMargins(layoutParameters.LeftMargin, topMargin, rightMargin, layoutParameters.BottomMargin);
				}
			}
		}
	}
}