using System;
using NUnit.Framework;


namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class ViewCellTests : BaseTestFixture
	{
		
		public void SetParentBeforeView ()
		{
			var parent = new View { Platform = new UnitPlatform () };
			var child = new View ();
			var viewCell = new ViewCell ();

			Assert.Null (viewCell.View);
			Assert.DoesNotThrow (() => viewCell.Parent = parent);

			viewCell.View = child;
			Assert.AreSame (parent, viewCell.Parent);
			Assert.AreSame (viewCell, child.Parent);
			Assert.AreSame (parent.Platform, child.Platform);
		}

		
		//issue 550
		public void SetBindingContextBeforeParent ()
		{
			var parent = new View { 
				Platform = new UnitPlatform (),
				BindingContext = new object (),
			};

			var itemcontext = new object ();
			var cell = new ViewCell { View = new Label ()};
			cell.BindingContext = itemcontext;
			cell.Parent = parent;

			Assert.AreSame (itemcontext, cell.View.BindingContext);
		}

		
		public void SetBindingContextBeforeView ()
		{
			var context = new object ();
			var view = new View ();
			var cell = new ViewCell ();
			cell.BindingContext = context;
			cell.View = view;
			Assert.AreSame (context, view.BindingContext);
		}

		
		public void SetViewBeforeBindingContext ()
		{
			var context = new object ();
			var view = new View ();
			var cell = new ViewCell ();
			cell.View = view;
			cell.BindingContext = context;
			Assert.AreSame (context, view.BindingContext);
		}
	}
}
