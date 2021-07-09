using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Shapes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13159, "[Bug] +Fix. Path.Data crashing when geometry has a PolyLineSegment with 0 point",
		PlatformAffected.iOS)]
#if UITEST
	[Category(UITestCategories.Shape)]
#endif
	public class Issue13159 : TestContentPage
	{
		const string TestReady = "TestReadyId";

		public Issue13159()
		{

		}

		protected override void Init()
		{
			var layout = new StackLayout();

			var instructions = new Label
			{
				AutomationId = TestReady,
				Padding = 12,
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "Without exceptions, the test has passed."
			};

			layout.Children.Add(instructions);
			layout.Children.Add(CreateNoPointsPolyLineSegmentPath());
			layout.Children.Add(CreateNoPointsPolyBezierSegmentPath());
			layout.Children.Add(CreateNoPointsPolyQuadraticBezierSegmentPath());
			layout.Children.Add(CreateNoPointsArcSegmentPath());

			Content = layout;
		}

		Path CreateNoPointsPolyLineSegmentPath()
		{
			var path = new Path();

			PathFigure pathFigure = new PathFigure();

			pathFigure.Segments.Add(new PolyLineSegment());

			PathGeometry geometry = new PathGeometry();

			geometry.Figures.Add(pathFigure);

			path.Data = geometry;

			return path;
		}

		Path CreateNoPointsPolyBezierSegmentPath()
		{
			var path = new Path();

			PathFigure pathFigure = new PathFigure();

			pathFigure.Segments.Add(new PolyBezierSegment());

			PathGeometry geometry = new PathGeometry();

			geometry.Figures.Add(pathFigure);

			path.Data = geometry;

			return path;
		}

		Path CreateNoPointsPolyQuadraticBezierSegmentPath()
		{
			var path = new Path();

			PathFigure pathFigure = new PathFigure();

			pathFigure.Segments.Add(new PolyQuadraticBezierSegment());

			PathGeometry geometry = new PathGeometry();

			geometry.Figures.Add(pathFigure);

			path.Data = geometry;

			return path;
		}

		Path CreateNoPointsArcSegmentPath()
		{
			var path = new Path();

			PathFigure pathFigure = new PathFigure();

			pathFigure.Segments.Add(new ArcSegment());

			PathGeometry geometry = new PathGeometry();

			geometry.Figures.Add(pathFigure);

			path.Data = geometry;

			return path;
		}

#if UITEST && __IOS__
		[Test]
		public void Issue13159NoPointsPathTest()
		{
			RunningApp.WaitForElement(TestReady);
		}
#endif
	}
}
