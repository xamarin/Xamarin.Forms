using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace Microsoft.Maui.Controls.Core.UnitTests.Layouts
{
	public class LayoutCompatTests : BaseTestFixture
	{
		[Test]
		public void BasicContentPage()
		{
			var page = new ContentPage() { IsPlatformEnabled = true };
			var layout = new VerticalStackLayout() { IsPlatformEnabled = true };
			var button = new Button() { IsPlatformEnabled = true };
			var expectedSize = new Size(100, 100);

			var view = Substitute.For<IViewHandler>();
			view.GetDesiredSize(default, default).ReturnsForAnyArgs(expectedSize);
			button.Handler = view;

			layout.Add(button);
			page.Content = layout;
			(page as IFrameworkElement).Arrange(new Rectangle(0, 0, 100, 100));

			Assert.AreEqual(expectedSize, button.Bounds.Size);
		}

		[Test]
		public void VerticalStackLayoutInsideStackLayout()
		{
			var stackLayout = new StackLayout() { IsPlatformEnabled = true };
			var verticalStackLayout = new VerticalStackLayout() { IsPlatformEnabled = true };
			var button = new Button() { IsPlatformEnabled = true, HeightRequest = 100, WidthRequest = 100 };
			var expectedSize = new Size(100, 100);

			var view = Substitute.For<IViewHandler>();
			view.GetDesiredSize(default, default).ReturnsForAnyArgs(expectedSize);
			button.Handler = view;

			stackLayout.Children.Add(verticalStackLayout);
			verticalStackLayout.Add(button);

			Layout(verticalStackLayout, 100, 100);
			Assert.AreEqual(expectedSize, button.Bounds.Size);
		}


		[Test]
		public void StackLayoutInsideVerticalStackLayout()
		{
			var stackLayout = new StackLayout() { IsPlatformEnabled = true };
			var verticalStackLayout = new VerticalStackLayout() { IsPlatformEnabled = true };
			var button = new Button() { IsPlatformEnabled = true, HeightRequest = 100, WidthRequest = 100 };
			var expectedSize = new Size(100, 100);

			var view = Substitute.For<IViewHandler>();
			view.GetDesiredSize(default, default).ReturnsForAnyArgs(expectedSize);
			button.Handler = view;

			verticalStackLayout.Add(stackLayout);
			stackLayout.Add(button);

			Layout(verticalStackLayout, 100, 100);
			Assert.AreEqual(expectedSize, button.Bounds.Size);
		}


		void Layout(IFrameworkElement frameworkElement, double width, double height)
		{
			var size = frameworkElement.Measure(100, 100);
			frameworkElement.Arrange(new Rectangle(0, 0, size.Width, size.Height));
		}
	}
}
