using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class FrameUnitTests : BaseTestFixture
	{
		
		public void TestConstructor ()
		{
			Frame frame = new Frame ();

			Assert.Null (frame.Content);
			Assert.AreEqual (new Thickness (20, 20, 20, 20), frame.Padding);
		}

		
		public void TestPackWithoutChild ()
		{
			Frame frame = new Frame ();

			var parent = new NaiveLayout ();

			bool thrown = false;
			try {
				parent.Children.Add (frame);
			} catch {
				thrown = true;
			}

			Assert.False (thrown);
		}

		
		public void TestPackWithChild ()
		{
			Frame frame = new Frame {
				Content = new View ()
			};

			var parent = new NaiveLayout ();

			bool thrown = false;
			try {
				parent.Children.Add (frame);
			} catch {
				thrown = true;
			}

			Assert.False (thrown);
		}

		
		public void TestSetChild ()
		{
			Frame frame = new Frame ();
			
			var child1 = new Label ();
			
			bool added = false;

			frame.ChildAdded += (sender, e) => added = true;

			frame.Content = child1;

			Assert.True (added);
			Assert.AreEqual (child1, frame.Content);

			added = false;
			frame.Content = child1;

			Assert.False (added);
		}

		
		public void TestReplaceChild ()
		{
			Frame frame = new Frame ();

			var child1 = new Label ();
			var child2 = new Label ();

			frame.Content = child1;

			bool removed = false;
			bool added = false;

			frame.ChildRemoved += (sender, e) => removed = true;
			frame.ChildAdded += (sender, e) => added = true;

			frame.Content = child2;

			Assert.True (removed);
			Assert.True (added);
			Assert.AreEqual (child2, frame.Content);
		}

		
		public void TestFrameLayout ()
		{
			View child;

			var frame = new Frame {
				Content = child = new View {
					WidthRequest = 100,
					HeightRequest = 200,
					IsPlatformEnabled = true
				},
				IsPlatformEnabled = true,
				Platform = new UnitPlatform ()
			};

			Assert.AreEqual (new Size (140, 240), frame.GetSizeRequest (double.PositiveInfinity, double.PositiveInfinity).Request);

			frame.Layout (new Rectangle (0, 0, 300, 300));

			Assert.AreEqual (new Rectangle (20, 20, 260, 260), child.Bounds);
		}

		
		public void TestDoesNotThrowOnSetNullChild ()
		{
			Assert.DoesNotThrow (() => new Frame {Content = null});
		}

		
		public void WidthRequest ()
		{
			var frame = new Frame {
				Content = new View {
					WidthRequest = 100,
					HeightRequest = 200,
					IsPlatformEnabled = true
				},
				IsPlatformEnabled = true,
				Platform = new UnitPlatform (),
				WidthRequest = 20
			};

			Assert.AreEqual (new Size (60, 240), frame.GetSizeRequest (double.PositiveInfinity, double.PositiveInfinity).Request);
		}

		
		public void HeightRequest ()
		{
			var frame = new Frame {
				Content = new View {
					WidthRequest = 100,
					HeightRequest = 200,
					IsPlatformEnabled = true,
					Platform = new UnitPlatform ()
				},
				IsPlatformEnabled = true,
				Platform = new UnitPlatform (),
				HeightRequest = 20
			};

			Assert.AreEqual (new Size (140, 60), frame.GetSizeRequest (double.PositiveInfinity, double.PositiveInfinity).Request);
		}

		
		public void LayoutVerticallyCenter ()
		{
			View child;

			var frame = new Frame {
				Content = child = new View {
					WidthRequest = 100,
					HeightRequest = 100,
					IsPlatformEnabled = true,
					VerticalOptions = LayoutOptions.Center
				},
				IsPlatformEnabled = true,
				Platform = new UnitPlatform ()
			};

			frame.Layout (new Rectangle (0, 0, 200, 200));

			Assert.AreEqual (new Rectangle (20, 50, 160, 100), child.Bounds);
		}

		
		public void LayoutVerticallyBegin()
		{
			View child;

			var frame = new Frame {
				Content = child = new View {
					WidthRequest = 100,
					HeightRequest = 100,
					IsPlatformEnabled = true,
					VerticalOptions = LayoutOptions.Start
				},
				IsPlatformEnabled = true,
				Platform = new UnitPlatform ()
			};

			frame.Layout (new Rectangle (0, 0, 200, 200));

			Assert.AreEqual (new Rectangle (20, 20, 160, 100), child.Bounds);
		}

		
		public void LayoutVerticallyEnd()
		{
			View child;

			var frame = new Frame {
				Content = child = new View {
					WidthRequest = 100,
					HeightRequest = 100,
					IsPlatformEnabled = true,
					VerticalOptions = LayoutOptions.End
				},
				IsPlatformEnabled = true,
				Platform = new UnitPlatform ()
			};

			frame.Layout (new Rectangle (0, 0, 200, 200));

			Assert.AreEqual (new Rectangle (20, 80, 160, 100), child.Bounds);
		}

		
		public void LayoutHorizontallyCenter()
		{
			View child;

			var frame = new Frame {
				Content = child = new View {
					WidthRequest = 100,
					HeightRequest = 100,
					IsPlatformEnabled = true,
					HorizontalOptions = LayoutOptions.Center
				},
				IsPlatformEnabled = true,
				Platform = new UnitPlatform ()
			};

			frame.Layout (new Rectangle (0, 0, 200, 200));

			Assert.AreEqual (new Rectangle (50, 20, 100, 160), child.Bounds);
		}

		
		public void LayoutHorizontallyBegin()
		{
			View child;

			var frame = new Frame {
				Content = child = new View {
					WidthRequest = 100,
					HeightRequest = 100,
					IsPlatformEnabled = true,
					HorizontalOptions = LayoutOptions.Start
				},
				IsPlatformEnabled = true,
				Platform = new UnitPlatform ()
			};

			frame.Layout (new Rectangle (0, 0, 200, 200));

			Assert.AreEqual (new Rectangle (20, 20, 100, 160), child.Bounds);
		}

		
		public void LayoutHorizontallyEnd()
		{
			View child;

			var frame = new Frame {
				Content = child = new View {
					WidthRequest = 100,
					HeightRequest = 100,
					IsPlatformEnabled = true,
					HorizontalOptions = LayoutOptions.End
				},
				IsPlatformEnabled = true,
				Platform = new UnitPlatform ()
			};

			frame.Layout (new Rectangle (0, 0, 200, 200));

			Assert.AreEqual (new Rectangle (80, 20, 100, 160), child.Bounds);
		}

		
		public void SettingPaddingThroughStyle()
		{
			var frame = new Frame {
				Style = new Style(typeof(Frame)) {
					Setters = {
						new Setter {Property = Layout.PaddingProperty, Value = 0}
					}
				}
			};

			Assert.AreEqual(new Thickness(0), frame.Padding);

		}
	}
}