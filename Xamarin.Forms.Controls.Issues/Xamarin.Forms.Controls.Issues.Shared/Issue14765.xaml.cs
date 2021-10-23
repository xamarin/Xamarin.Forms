using Xamarin.Forms;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Shapes;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 14765, "[Bug] [Android] Changing/Replacing Path Data value does not resize the shape to fit the container", PlatformAffected.Android)]
	public partial class Issue14765 : ContentPage
	{
		public Issue14765()
		{
#if APP
			InitializeComponent();

			UpdatePathBtn.Clicked += (sender, args) =>
			{
				PathFigure pathFigure1 = new PathFigure
				{
					IsClosed = true,
					StartPoint = new Point { X = 10, Y = 100 },
					Segments = new PathSegmentCollection {
						new LineSegment { Point = new Point { X = 100, Y = 100 } },
						new LineSegment { Point = new Point { X = 20, Y = 90 } },
					}
				};

				PathFigure pathFigure2 = new PathFigure
				{
					IsClosed = true,
					StartPoint = new Point { X = 60, Y = 80 },
					Segments = new PathSegmentCollection {
						new LineSegment { Point = new Point { X = 115, Y = 70 } },
						new LineSegment { Point = new Point { X = 120, Y = 80 } },
					}
				};

				PathFigureCollection figureCollection = new PathFigureCollection() { };
				figureCollection.Add(pathFigure1);
				figureCollection.Add(pathFigure2);

				Issue14765Path.Data = new PathGeometry { Figures = figureCollection };
			};
#endif
		}
	}
}