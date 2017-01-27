using System.Diagnostics;
using System.Threading.Tasks;

namespace Xamarin.Forms.Controls
{
	public partial class LayoutAddPerformance : ContentPage
	{
		public LayoutAddPerformance()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			layout.Children.Clear();

			await Task.Delay(2000);

			var sw = new Stopwatch();
			sw.Start();
			for (var i = 0; i < 500; i++)
			{
				layout.Children.Add(new Label { Text = i.ToString() });
			}
			sw.Stop();
			timingLabel.Text = sw.ElapsedMilliseconds.ToString();
		}
	}
}