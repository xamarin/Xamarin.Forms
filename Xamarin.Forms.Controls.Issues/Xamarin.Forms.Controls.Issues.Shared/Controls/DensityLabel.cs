using System;
using System.Windows.Input;

namespace Xamarin.Forms.Controls
{
	public class DensityLabel : Label
	{
		public DensityLabel() =>
			AutomationId = "DensityLabel";

		public void RefreshValue() =>
			GetCurrentDensityValue?.Execute(null);

		public ICommand GetCurrentDensityValue { get; set; }
	}
}