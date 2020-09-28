using System.Linq;
using System.Runtime.InteropServices;

namespace Xamarin.Platform
{
	public abstract class StackLayout : Layout, IStackLayout
	{
		public int Spacing { get; set; }

		bool _isMeasureValid;
		public override bool IsMeasureValid
		{
			get
			{
				return _isMeasureValid
					&& Children.All(child => child.IsMeasureValid);
			}

			protected set
			{
				_isMeasureValid = value;
			}
		}

		public static int MeasureSpacing(int spacing, int childCount) 
		{
			return childCount > 1 ? (childCount - 1) * spacing : 0;
		}
	}
}
