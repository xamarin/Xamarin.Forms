using System;
namespace Xamarin.Platform.Layouts
{
	public abstract class StackLayoutManager : LayoutManager
	{
		public StackLayoutManager(IStackLayout stack) : base(stack)
		{
			Stack = stack;
		}

		public IStackLayout Stack { get; }

		public static int MeasureSpacing(int spacing, int childCount)
		{
			return childCount > 1 ? (childCount - 1) * spacing : 0;
		}
	}
}
