using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

#if UITEST
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Brush)]
#endif
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11795, "[Bug] Brushes API - gradient offset does nothing on Android", PlatformAffected.Android)]
	public partial class Issue11795 : TestContentPage
	{
#if APP
		float _offset1, _offset2;
#endif
		public Issue11795()
		{
#if APP
			Title = "Issue 11795";
			InitializeComponent();

			_offset1 = 0.1f;
			_offset2 = 0.9f;
#endif
		}

		protected override void Init()
		{

		}

#if APP
		void OnOffset1SliderValueChanged(object sender, ValueChangedEventArgs e)
		{
			_offset1 = (float)e.NewValue;
			UpdateBackground(_offset1, _offset2);
		}

		void OnOffset2SliderValueChanged(object sender, ValueChangedEventArgs e)
		{
			_offset2 = (float)e.NewValue;
			UpdateBackground(_offset1, _offset2);
		}

		void UpdateBackground(float offset1, float offset2)
		{
			LinearGradientBrush linearGradient = new LinearGradientBrush
			{
				StartPoint = new Point(0, 0),
				EndPoint = new Point(1, 0),
				GradientStops = new GradientStopCollection
				{
					new GradientStop { Color = Color.YellowGreen, Offset = offset1 },
					new GradientStop { Color = Color.Green, Offset = offset2 }
				}
			};

			GradientBoxView.Background = linearGradient;
			GradientFrame.Background = linearGradient;
		}
#endif
	}
}