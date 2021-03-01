﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Maui.Controls
{
	// TODO: We don't currently have any concept of a page in Maui
	// so this just treats it as a layout for now
	public partial class ContentPage : Microsoft.Maui.ILayout
	{
		IReadOnlyList<Microsoft.Maui.IView> Microsoft.Maui.ILayout.Children =>
			new List<IView>() { Content };

		ILayoutHandler Maui.ILayout.LayoutHandler => Handler as ILayoutHandler;

		Thickness Maui.IView.Margin => new Thickness();

		void Maui.ILayout.Add(IView child)
		{
			Content = (View)child;
		}

		void Maui.ILayout.Remove(IView child)
		{
			Content = null;
		}

		protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
		{
			IsMeasureValid = true;
			return new Size(widthConstraint, heightConstraint);
		}

		protected override void ArrangeOverride(Rectangle bounds)
		{
			if (IsArrangeValid)
			{
				return;
			}

			Arrange(bounds);
			IsArrangeValid = true;
			IsMeasureValid = true;
			Handler?.SetFrame(Frame);

			if (Content is IFrameworkElement fe)
			{
				fe.Measure(Frame.Width, Frame.Height);
				fe.Arrange(Frame);
			}
		}
	}
}
