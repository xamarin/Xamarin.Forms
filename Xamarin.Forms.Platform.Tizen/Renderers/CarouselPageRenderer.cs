using System;
using ElmSharp;
using EColor = ElmSharp.Color;
using ERectangle = ElmSharp.Rectangle;

namespace Xamarin.Forms.Platform.Tizen
{
	/// <summary>
	/// Renderer of a CarouselPage widget.
	/// </summary>
	public class CarouselPageRenderer : VisualElementRenderer<CarouselPage>, IVisualElementRenderer
	{
		/// <summary>
		/// The minimum length of a swipe to be recognized as a page switching command, in screen pixels unit.
		/// </summary>
		public static readonly double s_minimumSwipeLengthX = 200.0;

		// Different levels of "difficulty" in making a valid swipe gesture, determined by a maximum absolute value
		// of an angle between a line formed by the swipe gesture and the horizontal axis, in arc degrees:
		public static readonly double s_challengeEasyArcDegrees = 25.0;
		public static readonly double s_challengeComfortableArcDegrees = 20.0;
		public static readonly double s_challengeStandardArcDegrees = 15.0;
		public static readonly double s_challengeHardArcDegrees = 10.0;

		/// <summary>
		/// The maximum allowed angle between a line formed by the swipe gesture and the horizontal axis, in arc degrees.
		/// The gesture will be recognized as a page switching command if its angle does not exceed this value.
		/// </summary>
		public static readonly double s_thresholdSwipeArcDegrees = s_challengeComfortableArcDegrees;

		/// <summary>
		/// The tangent of a maximum allowed angle between the swipe line and the horizontal axis.
		/// </summary>
		public static readonly double s_thresholdSwipeTangent = Math.Tan(s_thresholdSwipeArcDegrees * (Math.PI / 180.0));

		// A master container for the entire widget:
		protected Box _box;

		// Used for grabbing gestures over the entire screen, even if Page is smaller than it:
		protected ERectangle _filler;

		protected GestureLayer _gestureLayer;
		protected EvasObject _page;

		/// <summary>
		/// The default constructor.
		/// </summary>
		public CarouselPageRenderer()
		{
		}

		/// <summary>
		/// Invoked whenever the CarouselPage element has been changed in Xamarin.
		/// </summary>
		/// <param name="e">Event parameters.</param>
		protected override void OnElementChanged(ElementChangedEventArgs<CarouselPage> e)
		{
			if (NativeView == null)
			{
				// Creates an overlaying box which serves as a container
				// for both page and a gesture handling layer:
				_box = new Box(Forms.Context.MainWindow)
				{
					IsHorizontal = false,
				};
				_box.SetAlignment(-1, -1);
				_box.SetWeight(1, 1);
				_box.Show();

				// Disallows the Box to lay out its contents. They will be laid out manually,
				// because the page has to overlay the conformant rectangle. By default
				// Box will lay its contents in a stack. Applying an empty method disables it:
				_box.SetLayoutCallback(() => {
					ResizeContentsToFullScreen();
				});

				// Creates a Rectangle used for ensuring that the gestures will get recognized:
				_filler = new ERectangle(Forms.Context.MainWindow)
				{
					Color = EColor.Transparent,
				};
				_filler.SetAlignment(-1, -1);
				_filler.SetWeight(1, 1);
				_filler.Show();
				_box.PackEnd(_filler);

				// Creates a GestureLayer used for swipe gestures recognition and attaches it to the Box:
				_gestureLayer = new GestureLayer(_box);
				_gestureLayer.Attach(_box);
				AddLineGestureHandler();

				SetNativeControl(_box);
			}

			if (e.OldElement != null)
			{
				Element.CurrentPageChanged -= OnCurrentPageChanged;
			}

			if (e.NewElement != null)
			{
				Element.CurrentPageChanged += OnCurrentPageChanged;
			}

			// If pages have been added to the Xamarin widget and the user has not explicitly
			// marked one of them to be displayed, displays the first one:
			if (_page == null && Element.Children.Count > 0)
			{
				DisplayPage(Element.Children[0]);
			}

			base.OnElementChanged(e);
		}

		/// <summary>
		/// Called just before the associated element is deleted.
		/// </summary>
		/// <param name="disposing">True if the memory release was requested on demand.</param>
		protected override void Dispose(bool disposing)
		{
			if (_box != null)
			{
				Element.CurrentPageChanged -= OnCurrentPageChanged;

				// Unpacks the page from the box to prevent it from being disposed of prematurely:
				_box.UnPack(_page);

				_box.Unrealize();
				_box = null;
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Handles the process of switching between the displayed pages.
		/// </summary>
		/// <param name="sender">An object originating the request</param>
		/// <param name="ea">Additional arguments to the event handler</param>
		void OnCurrentPageChanged(object sender, EventArgs ea)
		{
			if (_page != null)
			{
				_page.Hide();
				_box.UnPack(_page);
			}

			DisplayPage(Element.CurrentPage);
			ResizeContentsToFullScreen();
		}

		/// <summary>
		/// Gets the index of the currently displayed page in Element.Children collection.
		/// </summary>
		/// <returns>An int value representing the index of the page currently displayed,
		/// or -1 if no page is being displayed currently.</returns>
		int GetCurrentPageIndex()
		{
			int index = -1;
			for (int k = 0; k < Element.Children.Count; ++k)
			{
				if (Element.Children[k] == Element.CurrentPage)
				{
					index = k;
					break;
				}
			}

			return index;
		}

		/// <summary>
		/// Resizes the widget's contents to utilize all the available screen space.
		/// </summary>
		void ResizeContentsToFullScreen()
		{
			// Box's geometry should match Forms.Context.MainWindow's geometry
			// minus the space occupied by the top toolbar.
			// Applies Box's geometry to both displayed page and conformant rectangle:
			_filler.Geometry = _page.Geometry = _box.Geometry;
		}

		/// <summary>
		/// Adds the feature of recognizing swipes to the GestureLayer.
		/// </summary>
		void AddLineGestureHandler()
		{
			_gestureLayer.SetLineCallback(GestureLayer.GestureState.End, (line) => {
				double horizontalDistance = line.X2 - line.X1;
				double verticalDistance = line.Y2 - line.Y1;

				// Determines whether the movement is long enough to be considered a swipe:
				bool isLongEnough = (Math.Abs(horizontalDistance) >= s_minimumSwipeLengthX);

				// Determines whether the movement is horizontal enough to be considered as a swipe:
				// The swipe arc's tangent value (v/h) needs to be lesser than or equal to the threshold value.
				// This approach allows for getting rid of computationally heavier atan2() function.
				double angleTangent = Math.Abs(verticalDistance) / horizontalDistance;
				bool isDirectionForward = (angleTangent < 0);

				// Determines whether the movement has been recognized as a valid swipe:
				bool isSwipeMatching = (isLongEnough && (Math.Abs(angleTangent) <= s_thresholdSwipeTangent));

				if (isSwipeMatching)
				{
					// TODO: Unsure whether changes made via ItemsSource/ItemTemplate properties will be handled correctly this way.
					// If not, it should be implemented in another method.
					if (isDirectionForward)
					{
						// Tries to switch the page to the next one:
						int currentPageIndex = GetCurrentPageIndex();
						if (currentPageIndex < Element.Children.Count - 1)
						{
							// Sets the current page to the next one:
							Element.CurrentPage = Element.Children[currentPageIndex + 1];
						}
						else
						{
							// Reacts to the case of forward-swiping when the last page is already being displayed:
							Log.Debug("CarouselPage: Displaying the last page already - can not revolve further.");

							// Note (TODO): Once we have a more sophisticated renderer able to e.g. display the animation
							// of revolving Pages or at least indicate current overall position, some visual feedback
							// should be provided here for the user who has haplessly tried to access a nonexistent page.
						}
					}
					else
					{
						// Tries to switch the page to the previous one:
						int currentPageIndex = GetCurrentPageIndex();
						if (currentPageIndex > 0)
						{
							// Sets the current page to the previous one:
							Element.CurrentPage = Element.Children[currentPageIndex - 1];
						}
						else
						{
							// Reacts to the case of backward-swiping when the first page is already being displayed:
							Log.Debug("CarouselPage: The first page is already being displayed - can not revolve further.");

							// Note (TODO): (The same as in case of scrolling forwards)
						}
					}
				}
			});
		}

		void DisplayPage(ContentPage p)
		{
			_page = Platform.GetOrCreateRenderer(p).NativeView;
			_page.SetAlignment(-1, -1);
			_page.SetWeight(1, 1);
			_page.Show();
			_box.PackEnd(_page);
		}

	}
}

