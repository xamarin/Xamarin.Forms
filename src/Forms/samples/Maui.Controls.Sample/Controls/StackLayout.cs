using System.Linq;
using Xamarin.Platform;

namespace Maui.Controls.Sample.Controls
{
	public abstract class StackLayout : Maui.Controls.Sample.Controls.Layout, IStackLayout
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
	}
}
