using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Platform;

namespace Sample
{
	public class ProgressBar : Xamarin.Forms.View, IProgress
	{
		public double Progress { get; set; }
		public Color ProgressColor { get; set; }

		public Task<bool> ProgressTo(double value, uint length, Easing easing)
		{
			var tcs = new TaskCompletionSource<bool>();

			this.Animate("Progress", d => Progress = d, Progress, value, length: length, easing: easing, finished: (d, finished) => tcs.SetResult(finished));

			return tcs.Task;
		}
	}
}